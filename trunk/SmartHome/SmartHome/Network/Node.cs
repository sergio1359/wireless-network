using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartHome.Plugins;
using SmartHome.Products;
using SmartHome.Tools;
using SmartHome.Memory;
using System.IO;
using SmartHome.Network.HomeDevices;

namespace SmartHome.Network
{
    public class Node
    {
        public int Id { get; set; }
        public uint Mac { get; set; }
        public string Name { get; set; }
        public byte NetworkRetries = 3;
        public ushort Address { get; set; }
        public List<Connector> Connectors { get; set; }
        public Position Position { get; set; }
        public BaseType Base { get; set; }
        public ShieldType Shield { get; set; }
        public Security Security { get; set; }

        public List<HomeDevice> HomeDevices 
        { 
            get 
            {
                throw new NotImplementedException();
            }
        } 

        public Node() { }

        public Node(uint Mac, BaseType baseType, ShieldType shieldType)
        {
            Base = baseType;
            Shield = shieldType;

            this.Mac = Mac;

            Connectors = new List<Connector>();

            foreach (var item in ProductConfiguration.GetShieldDictionary(shieldType))
            {
                Connectors.Add(new Connector(item.Key, item.Value.Item1, this));
            }

            Position = new Position();

            Security = new Security();
        }


        public Base GetBaseConfiguration()
        {
            return ProductConfiguration.GetBaseConfiguration(Base);
        }

        public SortedDictionary<DateTime, List<Operation>> GetTimeActions()
        {
            //Puke, quiero hacer esto con linQ!!
            SortedDictionary<DateTime, List<Operation>> res = new SortedDictionary<DateTime, List<Operation>>();
            foreach (var item in Sheduler.TimeActions)
            {
                foreach (var operation in item.Value)
                {
                    if (operation.DestionationHomeDevice.Connector.Node.Address == Address)
                    {
                        if (res.ContainsKey(item.Key))
                            res[item.Key].Add(operation);
                        else
                            res.Add(item.Key, new List<Operation>() { operation });
                    }
                }
            }

            return res;
        }

        public void GetEEPROM()
        {
            FirmwareUno fw = new FirmwareUno(this, 0x00); //TODO: Ojo
            byte[] memoryEEPROM = fw.GenerateEEPROM();
            //guardamos el bin
            File.WriteAllBytes(Mac.ToString()+".bin", memoryEEPROM);
            //guardamos el hex
            Hex.SaveBin2Hex(memoryEEPROM, Mac.ToString());
        }


    }
}
