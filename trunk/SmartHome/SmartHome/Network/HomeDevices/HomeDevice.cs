using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Comunications;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public Connector Connector { get; set; }

        public bool InUse
        {
            get
            {
                if (Connector != null)
                    return true;
                else
                    return false;
            }
        }

        private static string[] homeDeviceTypes = null;
        public static string[] HomeDeviceTypes
        {
            get
            {
                if(homeDeviceTypes == null)
                    homeDeviceTypes = typeof(HomeDevice).Assembly.GetTypes().Where(t => t != typeof(HomeDevice).GetType() && typeof(HomeDevice).GetType().IsAssignableFrom(t)).Select(t => t.Name).ToArray();

                return homeDeviceTypes;
            }
        }

        public string HomeDeviceType
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public Position Position { get; set; }
        public List<Operation> Operations { get; set; }

        public HomeDevice() { }

        public HomeDevice(string name)
        {
            Name = name;
        }

        public virtual void RefreshState()
        {

        }

        //LOW LEVEL PROTOCOL

        private Operation GetDefaultOperation(OPCode operationCode)
        {
            return GetDefaultOperation(operationCode, new byte[] { });
        }

        private Operation GetDefaultOperation(OPCode operationCode, byte[] args)
        {
            return new Operation() { DestionationHomeDevice = this, ConditionalRestriction = new List<ConditionalRestriction>(), TimeRestrictions = new List<TimeRestriction>(), OPCode = operationCode ,Args = args};
        }

        public Operation Reset() { return GetDefaultOperation(OPCode.Reset); }

        public Operation FirmwareVersionRead() { return GetDefaultOperation(OPCode.FirmwareVersionRead); }

        public Operation ShieldModelRead() { return GetDefaultOperation(OPCode.ShieldModelRead); }

        public Operation ConfigWrite(byte fragmentTotal, byte fragment, byte length, byte[] content) 
        {
            List<byte> args = new List<byte>();
            args.Add((byte)(fragmentTotal<<4|fragment));
            args.Add(length);
            args.AddRange(content);
            return GetDefaultOperation(OPCode.ConfigWrite, args.ToArray());
        }

        public Operation ConfigRead() { return GetDefaultOperation(OPCode.ConfigRead); }

        public Operation ConfigReadConfirmation(byte fragmentTotal, byte fragment, byte length, StatusCode statusCode)
        {
            return GetDefaultOperation(OPCode.ConfigWrite, new byte[] { (byte)(fragmentTotal << 4 | fragment), length, (byte)statusCode });
        }

        public Operation ConfigChecksumRead() { return GetDefaultOperation(OPCode.ConfigChecksumRead); }

        public Operation MACRead() { return GetDefaultOperation(OPCode.MacRead); }

        public Operation NextHopRead(ushort nodeAddress) 
        { 
            bool littleEndian = Connector.Node.GetBaseConfiguration().LittleEndian;

            return GetDefaultOperation(OPCode.NextHopRead); 
        }
    }
}
