#region Using Statements
using System;
using System.Collections.Generic;

#endregion

namespace DataLayer.Entities.HomeDevices.Status
{
    public static class StatusProvider
    {
        private static Dictionary<string, object> statusMap;

        static StatusProvider()
        {
            statusMap = new Dictionary<string, object>();
        }

        public static void StoreProperty(this HomeDevice homeDevice, string propertyName, object value)
        {
            string propKey = homeDevice.GetPropertyKey(propertyName);

            if (statusMap.ContainsKey(propKey))
            {
                if(value == null)
                    statusMap.Remove(propKey);
                else
                    statusMap[propKey] = value;
            }
            else if (value != null)
            {
                statusMap.Add(propKey, value);
            }
        }

        public static Nullable<T> ReadProperty<T>(this HomeDevice homeDevice, string propertyName) where T : struct
        {
            string propKey = homeDevice.GetPropertyKey(propertyName);

            if (statusMap.ContainsKey(propKey))
            {
                return (T)statusMap[propKey];
            }

            return null;
        }

        private static string GetPropertyKey(this HomeDevice homeDevice, string propertyName)
        {
            return homeDevice.Id + propertyName;
        }
    }
}
