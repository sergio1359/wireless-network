#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class LogService
    {
        public void AddLog(string logText)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                Log log = new Log()
                {
                    Category = LogTypes.App,
                    Date = DateTime.Now,
                    Message = logText
                };
                repository.LogRepository.Insert(log);
                repository.Commit();
            }
        }

        public IEnumerable<LogDTO> GetLog(string category)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                LogTypes type;
                if (Enum.TryParse(category, out type))
                {
                    var logs = repository.LogRepository.GetLogByCategory(type);
                    return Mapper.Map<IEnumerable<LogDTO>>(logs);
                }

                return null;
            }
        }

        public IEnumerable<LogDTO> GetLog(string category, int from, int to)
        {
            if (from >= to)
                return null;

            var logs = GetLog(category).Skip(from).Take(to - from);

            if (logs == null)
                return null;

            return logs;
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(LogTypes));
        }
    }
}
