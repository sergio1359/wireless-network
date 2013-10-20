#region Using Statements
using DataLayer.Entities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using SmartHome.Communications.Messages;
using System.Linq;
using System.Reflection;
using SmartHome.Communications;
using SmartHome.Communications.Modules;
using System.Threading.Tasks;

#endregion

namespace SmartHome.BusinessEntities
{
    public static class BusinessOperation
    {
        public static async Task<bool> Execute(this Operation operation)
        {
            return await CommunicationManager.Instance.FindModule<UserModule>().SendMessage(operation.GetOperationMessage());
        }

        public static OperationMessage GetOperationMessage(this Operation operation)
        {
            MethodInfo method = operation.DestionationHomeDevice.GetType().GetMethods().First(m => m.Name == operation.OperationName 
                && m.ReturnType == typeof(OperationMessage) 
                && m.GetCustomAttributes(typeof(OperationAttribute)).Any());

            return (OperationMessage)method.Invoke(operation.DestionationHomeDevice, operation.Args);
        }

        public static byte[] ToBinaryOperation(this Operation operation)
        {
            return operation.GetOperationMessage().ToBinary();
        }
    }
}
