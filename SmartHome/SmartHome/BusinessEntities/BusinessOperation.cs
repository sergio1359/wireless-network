using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using SmartHome.Comunications.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessEntities
{
    public class BusinessOperation
    {
        public static void Execute(this Operation operation)
        {

        }

        public static byte[] ToBinaryOperation(this Operation operation)
        {
            return operation.GetOperationMessage().ToBinary();
        }

        public static OperationMessage GetOperationMessage(this Operation operation)
        {
            MethodInfo method = operation.DestionationHomeDevice.GetType().GetMethods().First(m => m.Name == operation.OperationName 
                && m.ReturnType == typeof(OperationMessage) 
                && m.GetCustomAttribute() == OperationAttribute.GetCustomAttribute());

            return (OperationMessage)method.Invoke(operation.DestionationHomeDevice, operation.Params);
        }
    }
}
