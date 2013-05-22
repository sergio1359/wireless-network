using DataLayer.Repositories;
using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class ConnectorService
    {
        private ConnectorRepository _connectorRepository;

        public ConnectorService(ConnectorRepository connectorRepository)
        {
            _connectorRepository = connectorRepository;
        }

        public Connector AddConnector(Connector c)
        {
            throw new NotImplementedException();
        }
    }
}
