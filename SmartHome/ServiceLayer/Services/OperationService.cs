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
using System.Linq;
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
            using (UnitOfWork repository = new UnitOfWork())
            {
                Operation operation = repository.OperationRepository.GetById(idOperation);

                if (operation == null)
                    return;

                repository.OperationRepository.Delete(operation);
                repository.Commit();
            }
        }

        public void ExecuteOperation(int idOperation)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Operation operation = repository.OperationRepository.GetById(idOperation);

                if (operation != null)
                    operation.Execute();
            }
        }

        /// <summary>
        /// Devuelve las operaciones que un home device puede hacer
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public string[] GetHomeDeviceOperation(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return null;

                return homeDevice.GetHomeDeviceOperations();
            }
        }

        /// <summary>
        /// Devuelve las operaciones programadas en el homeDevice
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public OperationDTO[] GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return null;

                return Mapper.Map<OperationDTO[]>(homeDevice.Operations);
            }
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
            int idRes = -1;
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevDestino = repository.HomeDeviceRespository.GetById(idHomeDeviceDestination);
                HomeDevice homeDev = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDev == null || homeDevDestino == null)
                    return -1;

                Operation op = new Operation()
                {
                    DestionationHomeDevice = homeDevDestino,
                    OperationName = operation,
                    Args = args
                };

                homeDev.Operations.Add(op);

                idRes = repository.OperationRepository.Insert(op).Id;

                repository.Commit();
            }

            return idRes;
        }
        #endregion

        #region Theme Operation

        public ThemeDTO[] GetThemes()
        {
            IQueryable<Theme> themes;
            using (UnitOfWork repository = new UnitOfWork())
            {
                themes = repository.ThemesRespository.GetAll();
            }

            return Mapper.Map<ThemeDTO[]>(themes);
        }

        public void ExecuteTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme != null)
                    theme.ExecuteTheme();
            }
        }

        public OperationDTO[] GetOperationsOfTheme(int idTheme)
        {
            Theme theme;
            using (UnitOfWork repository = new UnitOfWork())
            {
                theme = repository.ThemesRespository.GetById(idTheme);
            }

            if (theme == null)
                return null;

            return Mapper.Map<OperationDTO[]>(theme.Operations);
        }

        public void RemoveTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme == null)
                    return;

                foreach (var item in theme.Operations)
                {
                    Services.OperationService.RemoveOperation(item.Id);
                }

                repository.ThemesRespository.Delete(theme);

                repository.Commit();
            }
        }
        #endregion

        #region SchedulerOperation

        public TimeOperationDTO[] GetScheduler()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                IEnumerable<TimeOperation> timeOps;

                timeOps = repository.TimeOperationRepository.GetAll();

                return Mapper.Map<TimeOperationDTO[]>(timeOps);
            }
        }

        public int AddScheduler(byte weekDays, TimeSpan time, string name, int idHomeDeviceDestination, string operation, object[] args = null)
        {
            int idRes = -1;
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDeviceDestination);

                if (homeDevice == null)
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
                idRes = repository.TimeOperationRepository.Insert(timeOperation).Id;
            }
            return idRes;
        }

        public void RemoveTimeOperation(int idTimeOperation)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                TimeOperation timeOp = repository.TimeOperationRepository.GetById(idTimeOperation);

                if (timeOp == null)
                    return;

                repository.TimeOperationRepository.Delete(timeOp);
                repository.Commit();
            }
        }

        #endregion

    }
}
