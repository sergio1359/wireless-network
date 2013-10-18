#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.HomeDevices;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace ServiceLayer
{
    public class SchedulerServices
    {
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
                    throw new ArgumentException("HomeDevice id doesn't exist");

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
                TimeOperation timeOperation = repository.TimeOperationRepository.GetById(idTimeOperation);

                if (timeOperation == null)
                    throw new ArgumentException("TimeOperation id doesn't exist");

                if (timeOperation.Operation.DestionationHomeDevice.InUse)
                {
                    //UPDATE CHECKSUM
                    timeOperation.Operation.DestionationHomeDevice.Connector.Node.UpdateChecksum(null);
                }

                repository.TimeOperationRepository.Delete(timeOperation);
                repository.Commit();
            }
        }
    }
}
