﻿#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.Communications.Messages;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmartHome.BusinessEntities;
using SmartHome.Communications.Modules;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public static class BusinessHomeDevice
    {
        private static Dictionary<Type, string[]> homeDeviceOperations = null;

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
            if(!homeDevice.InUse)
                return null;
            if (!homeDevice.ProductTag.HasValue)//no es un producto
                return homeDevice.Connector.GetPinPort();
            else //es un producto
            {
                List<PinPort> pinPorts = new List<PinPort>();
                //TODO
                return pinPorts.ToArray();
            }
        }

        public static OperationMessage RefreshState(this HomeDevice homeDevice)
        {
            if (homeDevice is WallPlug)
                return (homeDevice as WallPlug).RefreshState();
            else if (homeDevice is Light)
                return (homeDevice as Light).RefreshState();
            else if (homeDevice is Dimmable)
                return (homeDevice as Dimmable).RefreshState();
            else if (homeDevice is HumiditySensor)
                return (homeDevice as HumiditySensor).RefreshState();
            else if (homeDevice is PowerSensor)
                return (homeDevice as PowerSensor).RefreshState();
            else if (homeDevice is PresenceSensor)
                return (homeDevice as PresenceSensor).RefreshState();
            else if (homeDevice is RGBLight)
                return (homeDevice as RGBLight).RefreshState();
            else if (homeDevice is SwitchButton)
                return (homeDevice as SwitchButton).RefreshState();
            else if (homeDevice is TemperatureSensor)
                return (homeDevice as TemperatureSensor).RefreshState();
            else
                throw new ArgumentException("HomeDevice not valid");
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

        public static OperationMessage GetAddressableOperation(this HomeDevice homeDevice, OperationMessage message)
        {
            if (homeDevice.InUse)
                message.DestinationAddress = (ushort)homeDevice.Connector.Node.Address;

            return message;
        }

        

        public static PropertyInfo[] GetStateValue(this HomeDevice homeDevice)
        {
            return homeDevice.HomeDeviceType.GetProperties().Where(p => p.GetCustomAttributes(true)
                                            .OfType<PropertyAttribute>()
                                            .Where(a => !a.Internal).Count() > 0)
                                            .ToArray();
        }
    }
}
