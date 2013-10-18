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
            using (UnitOfWork repository = new UnitOfWork())
            {
                var themes = repository.ThemesRespository.GetAll();
                return Mapper.Map<IEnumerable<ThemeDTO>>(themes);
            }
        }

        public void ExecuteTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme == null)
                    throw new ArgumentException("Theme id doesn't exist");

                theme.ExecuteTheme();
            }
        }

        public IEnumerable<OperationDTO> GetOperationsOfTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Theme theme = repository.ThemesRespository.GetById(idTheme);

                if (theme == null)
                    throw new ArgumentException("Theme id doesn't exist");

                return Mapper.Map<IEnumerable<OperationDTO>>(theme.Operations);
            }
        }

        public void RemoveTheme(int idTheme)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
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
}
