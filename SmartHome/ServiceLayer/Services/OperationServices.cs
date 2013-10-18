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

                if (operation == null)
                    throw new ArgumentException("Operation id doesn't exist");

                if (operation.SourceHomeDevice.InUse)
                {
                    //UPDATE CHECKSUM
                    operation.SourceHomeDevice.Connector.Node.UpdateChecksum(null);
                }

                repository.OperationRepository.Delete(operation);
                repository.Commit();
            }
        }

        public void ExecuteOperation(int idOperation)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Operation operation = repository.OperationRepository.GetById(idOperation);

                if (operation == null)
                    throw new ArgumentException("Operation id doesn't exist");

                operation.Execute();
            }
        }

        public string[] GetHomeDeviceOperation(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                return homeDevice.GetHomeDeviceOperations();
            }
        }

        public IEnumerable<OperationDTO> GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                return Mapper.Map<IEnumerable<OperationDTO>>(homeDevice.Operations);
            }
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDeviceDestination = repository.HomeDeviceRespository.GetById(idHomeDeviceDestination);
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                if (homeDeviceDestination == null)
                    throw new ArgumentException("HomeDevice destination id doesn't exist");

                Operation op = new Operation()
                {
                    DestionationHomeDevice = homeDeviceDestination,
                    OperationName = operation,
                    Args = args
                };

                homeDevice.Operations.Add(op);

                int idRes = repository.OperationRepository.Insert(op).Id;

                repository.Commit();

                return idRes;
            }
        }
    }
}
