#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessHomeDevice
    {
        private static Dictionary<Type, string[]> homeDeviceOperations = null;

        public static HomeDevice CreateHomeDevice(string homeDeviceType)
        {
            Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

            return (HomeDevice)Activator.CreateInstance(deviceType);
        }

        public static List<PinPort> GetPinPorts(this HomeDevice homeDevice)
        {
            if(!homeDevice.InUse)
                return null;
            if (!homeDevice.ProductTag.HasValue)//no es un producto
                return homeDevice.Connector.GetPinPort();
            else //es un producto
            {
                List<PinPort> pinPorts = new List<PinPort>();

                return pinPorts;
            }
        }

        public static string[] GetHomeDeviceOperations(this HomeDevice homeDevice)
        {
            Type HomeDeviceType = homeDevice.GetType();
            if (HomeDeviceType == null || !typeof(HomeDevice).IsAssignableFrom(HomeDeviceType))
                return null;

            if (homeDeviceOperations == null)
                homeDeviceOperations = new Dictionary<Type, string[]>();

            if (!homeDeviceOperations.ContainsKey(HomeDeviceType))
            {
                homeDeviceOperations.Add(HomeDeviceType,
                                        HomeDeviceType.GetMethods()
                                        .Where(m => m.GetCustomAttributes(true)
                                            .OfType<OperationAttribute>()
                                            .Where(a => !a.Internal).Count() > 0)
                                        .Select(m => m.Name)
                                        .ToArray());
            }

            return homeDeviceOperations[HomeDeviceType];
        }

        public static void GetStateOperation(this HomeDevice homeDevice)
        {
            MethodInfo method = homeDevice.GetType().GetMethods().First(m => m.ReturnType == typeof(OperationMessage)
                && m.GetCustomAttributes(typeof(OperationAttribute)).Any());

            OperationMessage op = (OperationMessage)method.Invoke(homeDevice, null);
            //TODO WHEN WE HAVE THE SENDER METHOD
        }
    }
}
