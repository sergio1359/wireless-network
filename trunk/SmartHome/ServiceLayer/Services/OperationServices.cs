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

        public void ExecuteOperation(int idHomeDeviceDestination, string operationName, object[] args)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDeviceDestination = repository.HomeDeviceRespository.GetById(idHomeDeviceDestination);

                if (homeDeviceDestination == null)
                    throw new ArgumentException("HomeDevice destination id doesn't exist");

                if (!homeDeviceDestination.GetHomeDeviceOperations().Contains(operationName))
                    throw new ArgumentException("OperationName is not available on this HomeDevice");

                Operation operation = new Operation()
                {
                    DestionationHomeDevice = homeDeviceDestination,
                    OperationName = operationName,
                    Args = args
                };                

                operation.Execute();
            }
        }

        public string[] GetExecutableHomeDeviceNameOperations(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                return homeDevice.GetHomeDeviceOperations();
            }
        }

        public IEnumerable<OperationDTO> GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                return Mapper.Map<IEnumerable<OperationDTO>>(homeDevice.Operations);
            }
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operationName, object[] args)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                HomeDevice homeDeviceDestination = repository.HomeDeviceRespository.GetById(idHomeDeviceDestination);
                HomeDevice homeDevice = repository.HomeDeviceRespository.GetById(idHomeDevice);

                if (homeDevice == null)
                    throw new ArgumentException("HomeDevice id doesn't exist");

                if (homeDeviceDestination == null)
                    throw new ArgumentException("HomeDevice destination id doesn't exist");

                Operation operation = new Operation()
                {
                    DestionationHomeDevice = homeDeviceDestination,
                    OperationName = operationName,
                    Args = args
                };

                homeDevice.Operations.Add(operation);

                int idRes = repository.OperationRepository.Insert(operation).Id;

                repository.Commit();

                return idRes;
            }
        }
    }
}
