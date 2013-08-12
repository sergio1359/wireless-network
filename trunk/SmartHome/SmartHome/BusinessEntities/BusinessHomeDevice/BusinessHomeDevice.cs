#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums;
using DataLayer.Entities.HomeDevices;
using DataLayer.Entities;
#endregion

namespace SmartHome.BusinessEntities.BusinessHomeDevice
{
    public class BusinessHomeDevice
    {
        public static void LinkConnector(this HomeDevice homeDevice, Connector connector)
        {
            homeDevice.Connector = connector;
        }

        public static void UnlinkConnector(this HomeDevice homeDevice)
        {
            homeDevice.Connector = null;
        }

        public static string[] GetHomeDeviceOperations(this HomeDevice homeDevice, Type HomeDeviceType)
        {
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

        public virtual void RefreshState()
        {

        }
    }
}
