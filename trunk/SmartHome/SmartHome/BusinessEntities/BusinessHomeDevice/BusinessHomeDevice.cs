#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using SmartHome.Communications.Modules;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessHomeDevice
    {
        private static Dictionary<Type, MethodInfo[]> _homeDeviceOperations;
        private static Dictionary<Type, MethodInfo[]> HomeDeviceOperations
        {
            get
            {
                if (_homeDeviceOperations == null)
                    LoadExecutableMethods();
                return _homeDeviceOperations;
            }
        }

        public static HomeDevice CreateHomeDevice(string homeDeviceType)
        {
            try
            {
                Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

                return (HomeDevice)Activator.CreateInstance(deviceType);
            }
            catch (Exception)
            {
                throw new ArgumentException("The type " + homeDeviceType + " not exist in the system");
            }
        }

        public static PinPort[] GetPinPorts(this HomeDevice homeDevice)
        {
            if (!homeDevice.InUse)
                return null;
            if (!homeDevice.ProductTag.HasValue)//no es un producto
                return homeDevice.Connector.GetPinPort();

            //Is Product
            List<PinPort> pinPorts = new List<PinPort>();
            //TODO
            return pinPorts.ToArray();
        }

        public static OperationMessage RefreshState(this HomeDevice homeDevice)
        {
            if (homeDevice is WallPlug)
                return (homeDevice as WallPlug).RefreshState();
            if (homeDevice is Light)
                return (homeDevice as Light).RefreshState();
            if (homeDevice is Dimmable)
                return (homeDevice as Dimmable).RefreshState();
            if (homeDevice is HumiditySensor)
                return (homeDevice as HumiditySensor).RefreshState();
            if (homeDevice is PowerSensor)
                return (homeDevice as PowerSensor).RefreshState();
            if (homeDevice is PresenceSensor)
                return (homeDevice as PresenceSensor).RefreshState();
            if (homeDevice is RGBLight)
                return (homeDevice as RGBLight).RefreshState();
            if (homeDevice is SwitchButton)
                return (homeDevice as SwitchButton).RefreshState();
            if (homeDevice is TemperatureSensor)
                return (homeDevice as TemperatureSensor).RefreshState();

            throw new ArgumentException("HomeDevice not valid");
        }

        public static MethodInfo[] GetHomeDeviceMethodOperations(this HomeDevice homeDevice)
        {
            Type homeDeviceType = homeDevice.HomeDeviceType;
            if (homeDeviceType == null || !typeof(HomeDevice).IsAssignableFrom(homeDeviceType))
                throw new ArgumentException("This homeDevice is not a valid home Device");

            return HomeDeviceOperations[homeDeviceType];
        }

        public static string[] GetHomeDeviceNameOperations(this HomeDevice homeDevice)
        {
            return GetHomeDeviceMethodOperations(homeDevice).Select(m => m.Name).ToArray();
        }

        public static MethodInfo GetArgsOperation(this HomeDevice homeDevice, string nameOperation)
        {
            return GetHomeDeviceMethodOperations(homeDevice).First(m => m.Name == nameOperation);
        }

        public static OperationMessage GetAddressableOperation(this HomeDevice homeDevice, OperationMessage message)
        {
            if (homeDevice.InUse)
                message.DestinationAddress = (ushort)homeDevice.Connector.Node.Address;

            return message;
        }

        public static List<PropertyParam> GetStateValue(this HomeDevice homeDevice)
        {
            List<PropertyParam> propiertyValues = new List<PropertyParam>();

            var filterProperties = homeDevice.HomeDeviceType.GetProperties().Where(p => p.GetCustomAttributes(true)
                                            .OfType<PropertyAttribute>()
                                            .Where(a => !a.Internal).Count() > 0);

            foreach (var item in filterProperties)
            {
                Type type = item.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                }

                propiertyValues.Add(new PropertyParam
                {
                    Name = item.Name,
                    Type = type,
                    Value = item.GetValue(homeDevice, null)
                });
            }

            return propiertyValues;
        }

        private static void LoadExecutableMethods()
        {
            _homeDeviceOperations = new Dictionary<Type, MethodInfo[]>();
            foreach (var homeDeviceType in HomeDevice.HomeDeviceTypes)
            {
                var methods = homeDeviceType.GetEvenExtensionMethods(Assembly.GetAssembly(homeDeviceType))
                    .Where(m => m.GetCustomAttributes(true)
                                            .OfType<OperationAttribute>()
                                            .Where(a => !a.Internal).Count() > 0)
                    .ToArray();

                _homeDeviceOperations.Add(homeDeviceType, methods);
            }
        }

        private static IEnumerable<MethodInfo> GetEvenExtensionMethods(this Type type, Assembly extensionsAssembly)
        {
            var query = from t in extensionsAssembly.GetTypes()
                        where !t.IsGenericType && !t.IsNested
                        from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                        where m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where m.GetParameters()[0].ParameterType == type
                        select m;

            return query;
        }
    }

    public class PropertyParam
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
    }
}
