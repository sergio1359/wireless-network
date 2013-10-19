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
    public class NodeRepository : Repository<Node>
    {
        public NodeRepository(SmartHomeDBContext context) : base(context) { }

        public Node GetByMacAddress(string macAddress)
        {
            return this._Collection.FirstOrDefault(n => n.Mac == macAddress);
        }

        public Node GetByNetworkAddress(int nodeAddress)
        {
            return this._Collection.FirstOrDefault(n => n.Address == nodeAddress);
        }

        public Node GetByNetworkAddressWithConnectedHomeDevices(int nodeAddress)
        {
            return this.GetAllWithConnectedHomeDevices().FirstOrDefault(n => n.Address == nodeAddress);
        }

        public IQueryable<Node> GetAllWithConnectedHomeDevices()
        {
            return this._Collection.Include("Connectors.HomeDevices.Operations");
        }

        public override void Delete(Node entityNode)
        {
            UnitOfWork repository = new UnitOfWork(this._context);
            for (int i = entityNode.Connectors.Count - 1; i <= 0; i--)
            {
                repository.ConnectorRepository.Delete(entityNode.Connectors.ElementAt(i));
            }

            if (entityNode.Location != null)
                repository.LocationRepository.Delete(entityNode.Location);

            base.Delete(entityNode);
        }

        public int GetNewAddress()
        {
            var existingAddress = this._Collection.Select(n => n.Address);

            for (int i = 2; i < 0xFFFF; i++)
            {
                if (!existingAddress.Contains(i))
                    return i;
            }

            throw new IndexOutOfRangeException("A valid address can't be found");
        }
    }
}
