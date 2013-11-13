#region Using Statements
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities;
#endregion

namespace DataLayer
{
    public class SmartHomeDBInitializer : DropCreateDatabaseIfModelChanges<SmartHomeDBContext>
    {
        protected override void Seed(SmartHomeDBContext context)
        {
            context.Home.Add(new Home()
                {
                    Name = "TestHome",
                });
            /*IList<Home> defaultStandards = new List<Standard>();

            defaultStandards.Add(new Standard() { StandardName = "Standard 1", Description = "First Standard" });
            defaultStandards.Add(new Standard() { StandardName = "Standard 2", Description = "Second Standard" });
            defaultStandards.Add(new Standard() { StandardName = "Standard 3", Description = "Third Standard" });

            foreach (Standard std in defaultStandards)
                context.Standards.Add(std);*/

            //All standards will
            base.Seed(context);
        }
    }
}
