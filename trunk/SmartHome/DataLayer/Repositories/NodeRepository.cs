#region Using Statements
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace DataLayer
{
    public class NodeRepository: Repository<Node>
    {
        //public IQueryable<Connector> GetAllWithHomeDevice()
        //{
        //    return _Collection.Include("HomeDevice");
        //}

    }
}
