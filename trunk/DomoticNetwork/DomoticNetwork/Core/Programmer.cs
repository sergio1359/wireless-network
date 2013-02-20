using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DomoticNetwork.Core
{
    class Programmer
    {
        List<Task> tasks;
        Timer timer;

        public Programmer()
        {
            this.tasks = new List<Task>();
            //LoadTasks();
            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(CkeckTasks);
            timer.Interval = 1000; //cada segundo hacemos una comprobacion
            timer.Enabled = true;
            GC.KeepAlive(timer);
        }

        void CkeckTasks(object sender, ElapsedEventArgs e)
        {
            //buscamos tareas que hayan cumplido
            DateTime now = DateTime.Now;
            List<Task> toExecute = tasks.Where(x => (x.Date.CompareTo(now) <= 0)).ToList();
            //las eliminamos de la lista de tareas
            foreach (Task task in toExecute)
            {
                tasks.Remove(task);
            }
            //ejecutamos
            foreach (Task task in toExecute)
            {
                task.ExecuteTask();
            }
        }


        public void AddTask(string command, DateTime date)
        {
            tasks.Add(new Task(command, date));
        }

        public void AddTask(string command, int day, int month, int year, int hour, int minute)
        {
            tasks.Add(new Task(command, year, month, day, hour, minute));
        }

        public void AddTask(string command, int hour, int minute)
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            tasks.Add(new Task(command, day, month, year, hour, minute));
        }

        public void AddTask(string command, int remainMinutes)
        {
            DateTime time = DateTime.Now.AddMinutes(remainMinutes);

            tasks.Add(new Task(command, time));
        }

        public void DeleteAllTask()
        {
            tasks.Clear();
        }

        public ElapsedEventHandler ckeckTasks { get; set; }


        class Task
        {
            public String Command { get; set; }
            public DateTime Date { get; set; }

            public Task(String command, DateTime date)
            {
                Command = command;
                Date = date;
            }

            public void ExecuteTask()
            {
                
            }
        }
    }
}
