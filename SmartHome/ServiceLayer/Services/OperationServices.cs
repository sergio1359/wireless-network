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
    }
}
