#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;
using SmartHome.Products;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
#endregion

namespace SmartHome.DataLayer
{
    public class Connector
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ConnectorType ConnectorType { get; set; }

        public virtual Node Node { get; set; }

        public Dictionary<HomeDevice, List<PinPort>> MappingHomeDevice;

        [NotMapped]
        public List<HomeDevice> HomeDevices
        {
            get
            {
                return MappingHomeDevice.Keys.ToList();
            }
        }

        [NotMapped]
        public List<PinPort> PinPorts
        {
            get
            {
                return MappingHomeDevice.Values.SelectMany(v => v).ToList(); //TODO: quitar los repetidos
            }
        }

        [NotMapped]
        public bool InUse
        {
            get
            {
                return HomeDevices.Count != 0;
            }
        }

        public Connector() { }

        public Connector(string Name, ConnectorType type, Node node)
        {
            this.Name = Name;
            ConnectorType = type;
            Node = node;
        }

        /// <summary>
        /// Conecta, sin realizar ninguna comprobacion de compatibilidad, un conector con el HomeDevice pasado por parametros
        /// </summary>
        /// <param name="homeDevice"></param>
        public void LinkHomeDevice(HomeDevice homeDevice)
        {
            MappingHomeDevice.Add(homeDevice, GetPinPort());
        }

        /// <summary>
        /// Conecta, sin realizar ninguna comprobacion de compatibilidad, el conector con el producto cuyo nombre es pasado por parametros
        /// </summary>
        /// <param name="nameProduct"></param>
        public void LinkHomeDevice(string nameProduct)
        {
            
        }

        public void UnlinkHomeDevice()
        {
            if (this.ConnectorType == Network.ConnectorType.ConectorSensorBoard)
            {
                //TODO: hay que destruir los elementos uno a uno. 
                
            }

            MappingHomeDevice.Clear();
        }

        public bool IsCapable(HomeDevice homeDevice)
        {
            if (this.ConnectorType == Network.ConnectorType.DimmerPassZero)
                return false;

            return this.ConnectorType == homeDevice.ConnectorCapable;
        }

        public List<PinPort> GetPinPort()
        {
            return ProductConfiguration.GetShieldDictionary(Node.Shield)[Name].Item2;
        }

        public PinPortConfiguration GetPinPortConfiguration(HomeDevice homeDevice)
        {
            return ProductConfiguration.GetPinPortConfiguration(homeDevice);
        }

        public Operation[] GetActionsConnector()
        {
            if (!InUse)
            {
                return new Operation[0];
            }
            else
            {
                return HomeDevices.SelectMany(hd => hd.Operations).ToArray();
            }

        }
    }
}
