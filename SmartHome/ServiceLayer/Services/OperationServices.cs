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
    public class OperationServices
    {
        #region Generic Operation

        public void RemoveOperation(int idOperation)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Operation operation = repository.OperationRepository.GetById(idOperation);

                if (operation != null)
                {
                    if (operation.SourceHomeDevice.InUse)
                    {
                        //UPDATE CHECKSUM
                        operation.SourceHomeDevice.Connector.Node.UpdateChecksum(null);
                    }

                    repository.OperationRepository.Delete(operation);
                    repository.Commit();
                }
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

        public IEnumerable<OperationDTO> GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    return null;

                return Mapper.Map<IEnumerable<OperationDTO>>(homeDevice.Operations);
            }
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
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

                int idRes = repository.OperationRepository.Insert(op).Id;

                repository.Commit();

                return idRes;
            }

            
        }
        #endregion

        #region Theme Operation

        public IEnumerable<ThemeDTO> GetThemes()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var themes = repository.ThemesRespository.GetAll();
                return Mapper.Map<IEnumerable<ThemeDTO>>(themes);
            }
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

        public IEnumerable<OperationDTO> GetOperationsOfTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme == null)
                    return null;

                return Mapper.Map<IEnumerable<OperationDTO>>(theme.Operations);
            }
        }

        public void RemoveTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme != null)
                {
                    foreach (var item in theme.Operations)
                    {
                        Services.OperationService.RemoveOperation(item.Id);
                    }

                    repository.ThemesRespository.Delete(theme);

                    repository.Commit();
                }                
            }
        }
        #endregion

        #region SchedulerOperation

        public IEnumerable<TimeOperationDTO> GetScheduler()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                IEnumerable<TimeOperation> timeOps;

                timeOps = repository.TimeOperationRepository.GetAll();

                return Mapper.Map<IEnumerable<TimeOperationDTO>>(timeOps);
            }
        }

        public int AddScheduler(byte weekDays, TimeSpan time, string name, int idHomeDeviceDestination, string operation, object[] args = null)
        {
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
                int idRes = repository.TimeOperationRepository.Insert(timeOperation).Id;

                if (homeDevice.InUse)
                {
                    //UPDATE CHECKSUM
                    homeDevice.Connector.Node.UpdateChecksum(null);
                }

                repository.Commit();

                return idRes;
            }
        }

        public void RemoveTimeOperation(int idTimeOperation)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                TimeOperation timeOp = repository.TimeOperationRepository.GetById(idTimeOperation);

                if (timeOp != null)
                {
                    if (timeOp.Operation.DestionationHomeDevice.InUse)
                    {
                        //UPDATE CHECKSUM
                        timeOp.Operation.DestionationHomeDevice.Connector.Node.UpdateChecksum(null);
                    }

                    repository.TimeOperationRepository.Delete(timeOp);
                    repository.Commit();
                }
            }
        }

        #endregion

    }
}
