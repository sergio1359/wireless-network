#region Using Statement
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using SmartHome.BusinessEntities;
using SmartHome.Communications;
using SmartHome.Communications.Modules.Config;
using System.Collections.Generic;
#endregion

namespace ServiceLayer
{
    public class HomeService
    {
        public void SetHomeName(string newName)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Home home = repository.HomeRespository.GetHome();
            home.Name = newName;

            repository.Commit();
        }

        public string GetHomeName()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            return repository.HomeRespository.GetHome().Name;
        }

        public void SetHomeLocation(float latitude, float longitude)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Home home = repository.HomeRespository.GetHome();
            home.Location = new Coordenate
            {
                Latitude = latitude,
                Longitude = longitude
            };

            repository.Commit();
        }

        public Coordenate GetHomeLocation()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            return repository.HomeRespository.GetHome().Location;
        }

        public void UnlinkNode(int idNode)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Node node = repository.NodeRespository.GetById(idNode);

            if (node == null)
                throw new ArgumentException("Node Id doesn't exist");

            node.UnlinkAllConnectors();

            repository.NodeRespository.Delete(node);

            repository.Commit();
        }
    }
}
