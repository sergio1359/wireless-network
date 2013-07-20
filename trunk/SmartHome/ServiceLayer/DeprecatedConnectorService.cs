using DataLayer.Repositories;
using SmartHome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class DeprecatedConnectorService
    {
        private ConnectorRepository _connectorRepository;

        public DeprecatedConnectorService(ConnectorRepository connectorRepository)
        {
            _connectorRepository = connectorRepository;
        }

        public Connector AddConnector(Connector c)
        {
            throw new NotImplementedException();
        }
    }
}
