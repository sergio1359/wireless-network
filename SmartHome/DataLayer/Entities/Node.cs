#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataLayer.Entities.HomeDevices;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataLayer.Entities.Enums; 
#endregion

namespace DataLayer.Entities
{
    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(6), MinLength(6, ErrorMessage = "Mac must have 6 characters length")]
        public string Mac { get; set; }

        public string Name { get; set; }

        [Range(0, 30)]
        public int NetworkRetries { get; set; }

        public ushort Address { get; set; }

        public Location Location { get; set; }

        public BaseTypes Base { get; set; }

        public ShieldTypes Shield { get; set; }

        public virtual ICollection<Connector> Connectors { get; set; }

        [NotMapped]
        public IEnumerable<HomeDevice> HomeDevices
        {
            get
            {
                return this.Connectors.SelectMany(c => c.HomeDevices);
            }
        }

        public Node()
        {
            this.Connectors = new List<Connector>();
        }

        public Base GetBaseConfiguration()
        {
            return ProductConfiguration.GetBaseConfiguration(this.Base);
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
            File.WriteAllBytes(Mac.ToString() + ".bin", memoryEEPROM);
            //guardamos el hex
            Hex.SaveBin2Hex(memoryEEPROM, Mac.ToString());
        }


    }
}
