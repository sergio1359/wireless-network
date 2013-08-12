﻿using DataLayer.Entities;
using SmartHome.Memory;
using SmartHome.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.BusinessEntities
{
    public static class BusinesNode
    {
        public static Base GetBaseConfiguration(this Node node)
        {
            return ProductConfiguration.GetBaseConfiguration(node.Base);
        }

        public static SortedDictionary<DateTime, List<Operation>> GetTimeActions(this Node node)
        {
            throw new NotImplementedException();
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