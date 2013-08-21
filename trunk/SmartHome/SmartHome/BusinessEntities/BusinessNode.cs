#region Using Statements
using DataLayer.Entities;
using SmartHome.Memory;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
#endregion

namespace SmartHome.BusinessEntities
{
    public static class BusinessNode
    {
        public static Node CreateNode(BaseTypes baseType, ShieldTypes shieldType)
        {
            Node node = new Node()
            {
                Base = baseType,
                Shield = shieldType
            };

            foreach (var item in ProductConfiguration.GetShieldDictionary(shieldType))
            {
                if(item.Value.Item1 != DataLayer.Entities.Enums.ConnectorTypes.DimmerPassZero)
                    node.Connectors.Add(BusinessConnector.CreateConnector(item.Key, item.Value.Item1));
            }

            return node;
        }

        public static Base GetBaseConfiguration(this Node node)
        {
            return ProductConfiguration.GetBaseConfiguration(node.Base);
        }

        public static List<TimeOperation> GetTimeActions(this Node node)
        {
            List<TimeOperation> timeOperations = new List<TimeOperation>();

            foreach (var item in node.HomeDevices)
            {
                timeOperations.AddRange(DataLayer.Repositories.TimeOperationRepository.GetOperationInHomeDevice(item.Id));
            }

            timeOperations.Sort();

            return timeOperations;
        }

        //TOCHECK
        public static void ChangeShield(this Node node, ShieldTypes newShield)
        {
            //Unlink all home devices
            node.UnlinkAllConnectors();

            //Delete connectors
            node.Connectors.Clear();

            //Create new connectors
            foreach (var item in ProductConfiguration.GetShieldDictionary(newShield))
            {
                node.Connectors.Add(BusinessConnector.CreateConnector(item.Key, item.Value.Item1));
            }
        }

        public static void UnlinkAllConnectors(this Node node)
        {
            node.Connectors.ToList().ForEach(c => c.UnlinkHomeDevice());
        }

        public static byte[] GetBinaryConfiguration(this Node node)
        {
            FirmwareUno fw = new FirmwareUno(node, 0x00); //TODO: Ojo
            return fw.GenerateEEPROM();
        }

        public static void GetEEPROM(this Node node)
        {
            FirmwareUno fw = new FirmwareUno(node, 0x00); //TODO: Ojo
            byte[] memoryEEPROM = fw.GenerateEEPROM();
            //guardamos el bin
            File.WriteAllBytes(node.Mac.ToString() + ".bin", memoryEEPROM);
            //guardamos el hex
            Hex.SaveBin2Hex(memoryEEPROM, node.Mac.ToString());
        }
    }
}
