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
            Log log = new Log();
            log.Date = DateTime.Now;
            log.Category = LogTypes.App;
            log.Message = logText;

            Repositories.LogRepository.Insert(log);
        }

        /// <summary>
        /// Get all logs for a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns>If category don't exist then null return</returns>
        public LogDTO[] GetLog(string category)
        {
            LogTypes type;
            if (Enum.TryParse(category, out type))
            {
                var logs = Repositories.LogRepository.GetLogByCategory(type);

                return Mapper.Map<LogDTO[]>(logs);
            }
            else
                return null;
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
            
            var logs = GetLog(category).Skip(from).Take(to-from);

            if (logs == null)
                return null;

            return Mapper.Map<LogDTO[]>(logs);
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(LogTypes));
        }
    }
}
