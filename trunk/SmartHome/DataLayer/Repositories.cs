#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public static class Repositories
    {
        private static NodeRepository nodeRespository;

        public static NodeRepository NodeRespository
        {
            get
            {
                if (nodeRespository == null)
                    nodeRespository = new NodeRepository();
                return nodeRespository;
            }
        }
    }
}
