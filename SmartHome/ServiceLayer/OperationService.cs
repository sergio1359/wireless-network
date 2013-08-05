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
        public Dictionary<ushort, Tuple<string, ushort>> GetProgramOperations(ushort idHomeDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Elimina una operacion
        /// </summary>
        /// <param name="idOperation"></param>
        public void RemoveOperation(ushort idOperation)
        {
            NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation);
        }

        public void ExecuteOperation(ushort idOperation)
        {
            NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation).Execute();
        }

        public Operation[] GetHomeDeviceOperation(ushort idHomeDevice)
        {
            throw new NotImplementedException();
        }

        public Operation[] GetHomeDeviceOperation(string typeHomeDevice)
        {
            throw new NotImplementedException();
        }

        public ushort AddOperationOnHomeDeviceProgram(ushort idHomeDevice, ushort idHomeDeviceDestination, string operation, object[] param)
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

            return (ushort)op.Id;
        }
        #endregion

        //Gestiona el Themer
        #region THEME_OPERATION

        public Dictionary<ushort, string> GetThemes()
        {
            throw new NotImplementedException();
        }

        public void ExecuteTheme(ushort idTheme)
        {
            throw new NotImplementedException();
        }

        public Operation[] GetOperationOfTheme(ushort idTheme)
        {
            throw new NotImplementedException();
        }

        public void RemoveTheme(ushort idTheme)
        {
            throw new NotImplementedException();
        }
        #endregion

        //Getion del planificador de operaciones
        #region SCHEDULER_OPERATION

        //public Operation[] GetScheduler()
        //{
        //}

        //public Operation[] GetScheduler(ushort idHomeDevice)

        #endregion



    }
}
