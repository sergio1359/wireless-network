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
            UnitOfWork repository = UnitOfWork.GetInstance();

            Zone zone = repository.ZoneRepository.GetById(idZone);

            if (zone == null)
                throw new ArgumentException("Zone id doesn't exist");

            return Mapper.Map<IEnumerable<ZoneDTO>>(zone.Views);
        }

        public byte[] GetViewImage(int idView)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();
            View view = repository.ViewRepository.GetById(idView);

            if (view == null)
                throw new ArgumentException("View id doesn't exist");

            return view.ImageMap;
        }

        public void SetViewImage(int idView, byte[] newImage)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();
            View view = repository.ViewRepository.GetById(idView);

            if (view == null)
                throw new ArgumentException("View id doesn't exist");

            view.ImageMap = newImage;

            repository.Commit();
        }

        public string GetNameView(int idView)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();
            View view = repository.ViewRepository.GetById(idView);

            if (view == null)
                throw new ArgumentException("View id doesn't exist");

            return view.Name;
        }

        public void SetNameView(int idView, string newName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            View view = repository.ViewRepository.GetById(idView);

            if (view == null)
                throw new ArgumentException("View id doesn't exist");

            if (view.Id == view.Zone.MainView.Id)
                view.Zone.Name = newName;

            view.Name = newName;

            repository.Commit();
        }

        /// <summary>
        /// Add a View in a concrete Zone at Home, return the View's id
        /// </summary>
        public int AddView(int idZone, string nameView)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Zone zone = repository.ZoneRepository.GetById(idZone);

            if (zone == null)
                throw new ArgumentException("Zone id doesn't exist");

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

        public void RemoveView(int idView)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            View view = repository.ViewRepository.GetById(idView);

            if (view == null)
                throw new ArgumentException("View id doesn't exist");

            repository.ViewRepository.Delete(view);
            repository.Commit();
        }
    }
}
