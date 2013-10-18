using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class ZoneServices
    {
        public IEnumerable<ZoneDTO> GetZones()
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                var zones = repository.ZoneRepository.GetAll();

                return Mapper.Map<IEnumerable<ZoneDTO>>(zones);
            }
        }

        /// <summary>
        /// Add a Zone at Home and return it id.
        /// </summary>
        public int AddZone(string nameZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = new Zone()
                {
                    Name = nameZone,
                    Home = repository.HomeRespository.GetHome(),
                };

                zone = repository.ZoneRepository.Insert(zone);

                View view = new View()
                {
                    Name = nameZone,
                    Zone = zone
                };

                repository.ViewRepository.Insert(view);

                zone.MainView = view;

                repository.Commit();

                return zone.Id;
            }
        }

        public void RemoveZone(int idZone) //CHECK method
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);
                
                repository.ZoneRepository.Delete(zone);

                repository.Commit();
            }
        }

        public void SetNameZone(int idZone, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    throw new ArgumentException("Zone id doesn't exist");

                zone.Name = newName;
                zone.MainView.Name = newName;

                repository.Commit();
            }
        }
    }
}
