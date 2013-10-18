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
    public class LogServices
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

                throw new ArgumentException("Log cagetory doesn't exist");
            }
        }

        public IEnumerable<LogDTO> GetLog(string category, int from, int to)
        {
            if (from >= to)
                throw new ArgumentException("from is bigger than to");

            var logs = GetLog(category).Skip(from).Take(to - from);

            if (logs == null)
                throw new ArgumentException("Log Id doesn't exist");

            return logs;
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(LogTypes));
        }
    }
}
