﻿#region Using Statements
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using SmartHome.BusinessEntities;
using SmartHome.BusinessEntities.BusinessHomeDevice;
using AutoMapper;
using DataLayer.Entities.HomeDevices;
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
            var operation = Repositories.OperationRepository.GetById(idOperation);

            Repositories.OperationRepository.Delete(operation);
        }

        public void ExecuteOperation(int idOperation)
        {
            Repositories.OperationRepository.GetById(idOperation).Execute();
        }

        /// <summary>
        /// Devuelve las operaciones que un home device puede hacer
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public string[] GetHomeDeviceOperation(int idHomeDevice)
        {
            var homeDevice = Repositories.HomeDeviceRespository.GetById(idHomeDevice);

            return homeDevice.GetHomeDeviceOperations();
        }
        
        /// <summary>
        /// Devuelve las operaciones programadas en el homeDevice
        /// </summary>
        /// <param name="idHomeDevice"></param>
        /// <returns></returns>
        public OperationDTO[] GetHomeDeviceOperationProgram(int idHomeDevice)
        {
            var operations = Repositories.HomeDeviceRespository.GetById(idHomeDevice).Operations;

            return Mapper.Map<OperationDTO[]>(operations);
        }

        public int AddOperationOnHomeDeviceProgram(int idHomeDevice, int idHomeDeviceDestination, string operation, object[] args)
        {
            HomeDevice homeDevDestino = Repositories.HomeDeviceRespository.GetById(idHomeDeviceDestination);

            Operation op = new Operation()
            {
                DestionationHomeDevice = homeDevDestino,
                OperationName = operation,
                Args = args
            };

            HomeDevice homeDev = Repositories.HomeDeviceRespository.GetById(idHomeDevice);
            homeDev.Operations.Add(op);

            op = Repositories.OperationRepository.Insert(op);

            Repositories.SaveChanges();

            return op.Id;
        }
        #endregion

        #region Theme Operation

        public ThemeDTO[] GetThemes()
        {
            var themes = Repositories.ThemesRespository.GetAll();

            return Mapper.Map<ThemeDTO[]>(themes);
        }

        public void ExecuteTheme(int idTheme)
        {
            Repositories.ThemesRespository.GetById(idTheme).ExecuteTheme();
        }

        public OperationDTO[] GetOperationsOfTheme(int idTheme)
        {
            var operations = Repositories.ThemesRespository.GetById(idTheme).Operations;

            return Mapper.Map<OperationDTO[]>(operations);
        }

        public void RemoveTheme(int idTheme)
        {
            var theme = Repositories.ThemesRespository.GetById(idTheme);

            foreach (var item in theme.Operations)
            {
                Services.OperationService.RemoveOperation(item.Id);
            }

            Repositories.ThemesRespository.Delete(theme);
        }
        #endregion

        #region SchedulerOperation

        //public OperationDTO[] GetScheduler()
        //{
        //}

        //public OperationDTO[] GetScheduler(int idHomeDevice)

        #endregion

    }
}