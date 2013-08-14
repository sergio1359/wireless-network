#region Using Statements
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.Comunications.Messages;
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

        


        //Methods
        public static void LinkConnector(this HomeDevice homeDevice, Connector connector)
        {
            homeDevice.Connector = connector;
        }

        public static void UnlinkConnector(this HomeDevice homeDevice)
        {
            homeDevice.Connector = null;
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
