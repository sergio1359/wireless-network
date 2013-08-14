#region Using Statements
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public static class Repositories
    {
        private static DbContext context;
        private static NodeRepository nodeRespository;
        private static HomeDeviceRepository homeDeviceRespository;

        public static NodeRepository NodeRespository
        {
            get
            {
                if (nodeRespository == null)
                    nodeRespository = new NodeRepository(context);
                return nodeRespository;
            }
        }

        
        public static HomeDeviceRepository HomeDeviceRespository
        {
            get
            {
                if (homeDeviceRespository == null)
                    homeDeviceRespository = new HomeDeviceRepository(context);
                return homeDeviceRespository;
            }
        }

        static Repositories()
        {
            context = new SmartHomeDBContext();
        }

    }
}
