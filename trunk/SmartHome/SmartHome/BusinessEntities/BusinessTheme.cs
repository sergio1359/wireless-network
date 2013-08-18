using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessEntities
{
    public static class BusinessTheme
    {
        public static async void ExecuteTheme(this Theme theme)
        {
            List<Task<bool>> taskList = new List<Task<bool>>();

            foreach (var operation in theme.Operations)
            {
                var operationTask = operation.Execute();
                operationTask.Start();

                taskList.Add(operationTask);
            }

            await Task.WhenAll(taskList);
        }
    }
}
