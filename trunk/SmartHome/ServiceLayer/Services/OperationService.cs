#region Using Statements
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using AutoMapper;
using DataLayer.Entities.HomeDevices;
#endregion

namespace ServiceLayer
{
    public class OperationService
    {
        #region Generic Operation

        /// <summary>
        /// Elimina una operacion
        /// </summary>
        /// <param name="idOperation"></param>
        public void RemoveOperation(int idOperation)
        {
            Operation operation = Repositories.OperationRepository.GetById(idOperation);

            if (operation == null)
                return;

            Repositories.OperationRepository.Delete(operation);
        }

        public void ExecuteOperation(int idOperation)
        {
            Operation operation = Repositories.OperationRepository.GetById(idOperation);

            if (operation != null)
                operation.Execute();
        }

        /// <summary>
        /// Devuelve las operaciones que un home device puede hacer
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public string[] GetHomeDeviceOperation(int idHomeDevice)
        {
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                return null;

            return homeDevice.GetHomeDeviceOperations();
        }

        /// <summary>
        /// Devuelve las operaciones programadas en el homeDevice
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public OperationDTO[] GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDevice == null)
                return null;

            return Mapper.Map<OperationDTO[]>(homeDevice.Operations);
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
            HomeDevice homeDevDestino = Repositories.HomeDeviceRespository.GetById(idHomeDeviceDestination);
            HomeDevice homeDev = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            if (homeDev == null || homeDevDestino == null)
                return -1;

            Operation op = new Operation()
            {
                DestionationHomeDevice = homeDevDestino,
                OperationName = operation,
                Args = args
            };

            homeDev.Operations.Add(op);

            op = Repositories.OperationRepository.Insert(op);

            Repositories.SaveChanges();

            return op.Id;
        }
        #endregion

        #region Theme Operation

        public ThemeDTO[] GetThemes()
        {
            var themes = Repositories.ThemesRespository.GetAll();

            return Mapper.Map<ThemeDTO[]>(themes);
        }

        public void ExecuteTheme(int idTheme)
        {
            Theme theme = Repositories.ThemesRespository.GetById(idTheme);

            if (theme != null)
                theme.ExecuteTheme();
        }

        public OperationDTO[] GetOperationsOfTheme(int idTheme)
        {
            Theme theme = Repositories.ThemesRespository.GetById(idTheme);

            if (theme == null)
                return null;

            return Mapper.Map<OperationDTO[]>(theme.Operations);
        }

        public void RemoveTheme(int idTheme)
        {
            Theme theme = Repositories.ThemesRespository.GetById(idTheme);

            if (theme == null)
                return;

            foreach (var item in theme.Operations)
            {
                Services.OperationService.RemoveOperation(item.Id);
            }

            Repositories.ThemesRespository.Delete(theme);
        }
        #endregion

        #region SchedulerOperation

        public TimeOperationDTO[] GetScheduler()
        {
            var timeOps = Repositories.TimeOperationRepository.GetAll();

            return Mapper.Map<TimeOperationDTO[]>(timeOps);
        }

        public int AddScheduler(byte weekDays, TimeSpan time, string name, int idHomeDeviceDestination, string operation, object[] args = null)
        {
            HomeDevice homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDeviceDestination);

            if(homeDevice == null)
                return -1;

            Operation operationInternal = new Operation()
            {
                Args = args,
                Name = name,
                OperationName = operation,
                DestionationHomeDevice = homeDevice
            };

            TimeOperation timeOperation = new TimeOperation()
            {
                MaskWeekDays = weekDays,
                Time = time,
                Operation = operationInternal
            };

            return Repositories.TimeOperationRepository.Insert(timeOperation).Id;
        }

        public void RemoveTimeOperation(int idTimeOperation)
        {
            TimeOperation timeOp = Repositories.TimeOperationRepository.GetById(idTimeOperation);

            if (timeOp == null)
                return;

            Repositories.TimeOperationRepository.Delete(timeOp);
        }

        #endregion

    }
}
