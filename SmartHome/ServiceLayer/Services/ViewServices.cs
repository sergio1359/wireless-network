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
    public class ViewServices
    {
        public IEnumerable<ZoneDTO> GetViews(int idZone)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return null;

                return Mapper.Map<IEnumerable<ZoneDTO>>(zone.Views);
            }
        }

        public byte[] GetViewImage(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return null;

                return view.ImageMap;
            }
        }

        public void SetViewImage(int idView, byte[] newImage)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return;

                view.ImageMap = newImage;

                repository.Commit();
            }
        }

        public string GetNameView(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return null;

                return view.Name;
            }
        }

        public void SetNameView(int idView, string newName)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view == null)
                    return;

                if (view.Id == view.Zone.MainView.Id)
                    view.Zone.Name = newName;

                view.Name = newName;

                repository.Commit();
            }
        }

        /// <summary>
        /// Add a View in a concrete Zone at Home, return the View's id
        /// </summary>
        public int AddView(int idZone, string nameView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Zone zone = repository.ZoneRepository.GetById(idZone);

                if (zone == null)
                    return -1;

                View view = new View()
                {
                    Name = nameView,
                    Zone = zone,
                };
                view = repository.ViewRepository.Insert(view);

                zone.Views.Add(view);

                repository.Commit();

                return view.Id;
            }
        }

        public void RemoveView(int idView)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                View view = repository.ViewRepository.GetById(idView);

                if (view != null)
                {
                    repository.ViewRepository.Delete(view);
                    repository.Commit();
                }
            }
        }
    }
}
