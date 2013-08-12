using DataLayer.Entities.Enums;
using ServiceLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class LogService
    {
        /// <summary>
        /// Add a log from the client of this service
        /// </summary>
        /// <param name="log"></param>
        public void AddClientLog(string log)
        {

        }


        /// <summary>
        /// Get all logs for a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public LogDTO[] GetLog(string category)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the categories of the logs.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public LogDTO[] GetLogCategory(string category)
        {
            throw new NotImplementedException();
        }

        public string[] GetCategories()
        {
            return Enum.GetNames(typeof(LogTypes));
        }
    }
}
