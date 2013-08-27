#region Using Statements
using AutoMapper;
using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using ServiceLayer.DTO;
using System;
using System.Linq;
#endregion

namespace ServiceLayer
{
    public class LogService
    {
        /// <summary>
        /// Add a log from the client of this service
        /// </summary>
        /// <param name="log"></param>
        public void AddClientLog(string logText)
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

        /// <summary>
        /// Get all logs for a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns>If category don't exist then null return</returns>
        public LogDTO[] GetLog(string category)
        {
            using (UnitOfWork repository = new UnitOfWork())
            {
                LogTypes type;
                if (Enum.TryParse(category, out type))
                {
                    var logs = repository.LogRepository.GetLogByCategory(type);
                    return Mapper.Map<LogDTO[]>(logs);
                }

                return null;
            }
        }

        /// <summary>
        /// Get logs for a concrete category.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public LogDTO[] GetLog(string category, int from, int to)
        {
            if (from >= to)
                return null;

            var logs = GetLog(category).Skip(from).Take(to - from);

            if (logs == null)
                return null;

            return logs.ToArray();
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(LogTypes));
        }
    }
}
