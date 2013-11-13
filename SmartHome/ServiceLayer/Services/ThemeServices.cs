#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.BusinessEntities;
#endregion

namespace ServiceLayer
{
    public class ThemeServices
    {
        public IEnumerable<ThemeDTO> GetThemes()
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            var themes = repository.ThemesRespository.GetAll();
            return Mapper.Map<IEnumerable<ThemeDTO>>(themes);
        }

        public void ExecuteTheme(int idTheme)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Theme theme = repository.ThemesRespository.GetById(idTheme);

            if (theme == null)
                throw new ArgumentException("Theme id doesn't exist");

            theme.ExecuteTheme();
        }

        public IEnumerable<OperationProgrammedDTO> GetOperationsOfTheme(int idTheme)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Theme theme = repository.ThemesRespository.GetById(idTheme);

            if (theme == null)
                throw new ArgumentException("Theme id doesn't exist");

            return Mapper.Map<IEnumerable<OperationProgrammedDTO>>(theme.Operations);
        }

        public void RemoveTheme(int idTheme)
        {
            UnitOfWork repository = UnitOfWork.GetInstance();

            Theme theme = repository.ThemesRespository.GetById(idTheme);

            if (theme == null)
                throw new ArgumentException("Theme id doesn't exist");

            foreach (var item in theme.Operations)
            {
                Services.OperationService.RemoveOperation(item.Id);
            }

            repository.ThemesRespository.Delete(theme);
            repository.Commit();
        }
    }
}
