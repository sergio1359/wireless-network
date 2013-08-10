using ServiceLayer.DTO;
using SmartHome.Network;
using SmartHome.Network.HomeDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class OperationService
    {

        //ENCARGADA DE OPERACIONES BASICAS SOBRE TODAS LAS OEPRACIONES
        #region GENERIC_OPERATION_REGION

        /// <summary>
        /// Devuelve las operaciones 
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public Dictionary<int, Tuple<string, int>> GetProgramOperations(int idHomeDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Elimina una operacion
        /// </summary>
        /// <param name="idOperation"></param>
        public void RemoveOperation(int idOperation)
        {
            NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation);
        }

        public void ExecuteOperation(int idOperation)
        {
            NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation).Execute();
        }

        public OperationDTO[] GetHomeDeviceOperation(int idHomeDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the HomeDevice's operation types of the system.
        /// </summary>
        /// <returns>Array with the types names</returns>
        public string[] GetHomeDeviceOperationTypes(string homeDeviceType)
        {
            Type deviceType = typeof(HomeDevice).Assembly.GetTypes().First(t => t.Name == homeDeviceType);

            return HomeDevice.GetHomeDeviceOperations(deviceType);
        }
        /*
        public OperationDTO[] GetHomeDeviceOperation(string homeDeviceType)
        {
            throw new NotImplementedException();
        }*/

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] param)
        {
            HomeDevice homeDevDestino = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDeviceDestination);

            Operation op = new Operation()
            {
                DestionationHomeDevice = homeDevDestino,
                OperationName = operation,
                Args = param
            };

            HomeDevice homeDev = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            homeDev.Operations.Add(op);

            return op.Id;
        }
        #endregion

        //Gestiona el Themer
        #region THEME_OPERATION

        public Dictionary<int, string> GetThemes()
        {
            throw new NotImplementedException();
        }

        public void ExecuteTheme(int idTheme)
        {
            throw new NotImplementedException();
        }

        public Operation[] GetOperationOfTheme(int idTheme)
        {
            throw new NotImplementedException();
        }

        public void RemoveTheme(int idTheme)
        {
            throw new NotImplementedException();
        }
        #endregion

        //Getion del planificador de operaciones
        #region SCHEDULER_OPERATION

        //public Operation[] GetScheduler()
        //{
        //}

        //public Operation[] GetScheduler(int idHomeDevice)

        #endregion



    }
}
