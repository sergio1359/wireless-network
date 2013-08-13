#region Using Statements
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic; 
#endregion

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
        public OperationDTO[] GetProgramOperations(int idHomeDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Elimina una operacion
        /// </summary>
        /// <param name="idOperation"></param>
        public void RemoveOperation(int idOperation)
        {
            throw new NotImplementedException();
            //NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation);
        }

        public void ExecuteOperation(int idOperation)
        {
            throw new NotImplementedException();
            //NetworkManager.HomeDevices.SelectMany(hd => hd.Operations).First(op => op.Id == idOperation).Execute();
        }

        public OperationDTO[] GetHomeDeviceOperation(int idHomeDevice)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the HomeDevice's operation types of the system.
        /// </summary>
        /// <returns>Array with the types names</returns>
        public string[] GetHomeDeviceOperationTypes(int idHomeDevice)
        {
            throw new NotImplementedException();
        }
        /*
        public OperationDTO[] GetHomeDeviceOperation(string homeDeviceType)
        {
            throw new NotImplementedException();
        }*/

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
            throw new NotImplementedException();
            //HomeDevice homeDevDestino = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDeviceDestination);

            //Operation op = new Operation()
            //{
            //    DestionationHomeDevice = homeDevDestino,
            //    OperationName = operation,
            //    Args = args
            //};

            //HomeDevice homeDev = NetworkManager.HomeDevices.First(hd => hd.Id == idHomeDevice);
            //homeDev.Operations.Add(op);

            //return op.Id;
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
