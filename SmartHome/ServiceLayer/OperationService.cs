using SmartHome.Network;
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
            
        }

        /// <summary>
        /// Elimina una operacion
        /// </summary>
        /// <param name="idOperation"></param>
        public void RemoveOperation(ushort idOperation)
        {

        }


        public void ExecuteOperation(ushort idOperation)
        {

        }

        public Operation[] GetHomeDeviceOperation(ushort idHomeDevice)
        {
        }

        public Operation[] GetHomeDeviceOperation(string typeHomeDevice)
        {

        }

        public ushort AddOperationOnHomeDevideProgram(ushort idHomeDevice, ushort idHomeDeviceDestination, string operation, object[] param)
        {

        }



        #endregion

        //Gestiona el Themer
        #region THEME_OPERATION

        public Dictionary<ushort, string> GetThemes()
        {

        }

        public void ExecuteTheme(ushort idTheme)
        {

        }

        public Operation[] GetOperationOfTheme(ushort idTheme)
        {

        }

        public void RemoveTheme(ushort idTheme)
        {

        }

        #endregion

        //Getion del planificador de operaciones
        #region SCHEDULER_OPERATION

        //public Operation[] GetScheduler()

        //public Operation[] GetScheduler(ushort idHomeDevice)

        #endregion



    }
}
