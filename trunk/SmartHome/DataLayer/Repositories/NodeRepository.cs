#region Using Statements
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public class NodeRepository: Repository<Node>
    {
        public NodeRepository(SmartHomeDBContext context) : base(context) { }



    }
}
