using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Plugins;
using SmartHome.Comunications;
using SmartHome.Memory;
using System.Drawing;

namespace SmartHome.Network.HomeDevices
{
    public abstract class HomeDevice
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public Connector Connector { get; set; }
        public ConnectorType ConnectorCapable { get; set; }

        public bool InUse
        {
            get
            {
                return Connector != null;
            }
        }

        private static string[] homeDeviceTypes = null;
        public static string[] HomeDeviceTypes
        {
            get
            {
                if (homeDeviceTypes == null)
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

        public void LinkConnector(Connector connector)
        {
            this.Connector = connector;
        }

        public void UnlinkConnector()
        {
            this.Connector = null;
        }

        public virtual void RefreshState()
        {

        }



        //LOW LEVEL COMUNICATION PROTOCOL
        #region ComunicationProtocol
        public enum LogicWriteValues : byte
        {
            Clear = 0x00,
            Set = 0x01,
            SetRelative = 0x80,
            SetRelativeInverse = 0x81
        }

        private Operation GetDefaultOperation(OPCode operationCode)
        {
            return GetDefaultOperation(operationCode, new byte[] { });
        }

        private Operation GetDefaultOperation(OPCode operationCode, byte[] args)
        {
            return new Operation() { DestionationHomeDevice = this, ConditionalRestriction = new List<ConditionalRestriction>(), TimeRestrictions = new List<TimeRestriction>(), OPCode = operationCode, Args = args };
        }

        private Operation GetDefaultOperationAddr(OPCode operationCode, byte[] args)
        {
            List<byte> argaux = new List<byte>();
            argaux.AddRange(Id.UshortToByte(Connector.Node.GetBaseConfiguration().LittleEndian));
            argaux.AddRange(args);

            return GetDefaultOperation(operationCode, argaux.ToArray());
        }

        public Operation Reset() { return GetDefaultOperation(OPCode.Reset); }

        public Operation FirmwareVersionRead() { return GetDefaultOperation(OPCode.FirmwareVersionRead); }

        public Operation ShieldModelRead() { return GetDefaultOperation(OPCode.ShieldModelRead); }

        public Operation ConfigWrite(byte fragmentTotal, byte fragment, byte length, byte[] content)
        {
            List<byte> args = new List<byte>();
            args.Add((byte)(fragmentTotal << 4 | fragment));
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

        public Operation NextHopRead(ushort nodeAddress) { return GetDefaultOperation(OPCode.NextHopRead, nodeAddress.UshortToByte(Connector.Node.GetBaseConfiguration().LittleEndian)); }

        public Operation RouteTableRead() { return GetDefaultOperation(OPCode.RouteTableRead); }

        public Operation RouteTableReadConfirmation(byte fragmentTotal, byte fragment, byte length, StatusCode statusCode)
        {
            return GetDefaultOperation(OPCode.RouteTableReadConfirmation, new byte[] { (byte)(fragmentTotal << 4 | fragment), length, (byte)statusCode });
        }

        public Operation DateTimeWrite(WeekDays week, DateTime date, DateTime time)
        {
            List<byte> args = new List<byte>();
            args.Add((byte)week);
            args.AddRange(date.ToBinaryDate(Connector.Node.GetBaseConfiguration().LittleEndian));
            args.AddRange(time.ToBinaryTime());
            return GetDefaultOperation(OPCode.RouteTableRead, args.ToArray());
        }

        public Operation DateTimeRead() { return GetDefaultOperation(OPCode.DateTimeRead); }

        //WARNING Y ERROR CODE NOT IMPLEMENTED :D

        public Operation LogicWrite(LogicWriteValues value, byte seconds) { return GetDefaultOperationAddr(OPCode.LogicWrite, new byte[] { (byte)value, seconds }); }

        public Operation LogicSwitch(byte seconds) { return GetDefaultOperationAddr(OPCode.LogicSwitch, new byte[] { seconds }); }

        public Operation LogicRead() { return GetDefaultOperationAddr(OPCode.LogicRead, new byte[] { }); }

        public Operation DimmerWrite(byte value, byte seconds) { return GetDefaultOperationAddr(OPCode.DateTimeWrite, new byte[] { value, seconds }); }

        public Operation DimmerRead() { return GetDefaultOperationAddr(OPCode.DimmerRead, new byte[] { }); }

        public Operation ColorWrite(Color color, byte seconds) { return GetDefaultOperationAddr(OPCode.ColorWrite, new byte[] { color.R, color.G, color.B, seconds }); }

        public Operation ColorWriteRandom(byte seconds) { return GetDefaultOperationAddr(OPCode.ColorWriteRandom, new byte[] { seconds }); }

        public Operation ColorRandomSecuenceWrite(byte seconds, Color[] colors)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            foreach (var item in colors)
            {
                args.AddRange(new byte[] { item.R, item.G, item.B });
            }

            return GetDefaultOperationAddr(OPCode.ColorRandomSecuenceWrite, args.ToArray());
        }

        public Operation ColorSortedSecuenceWrite(byte seconds, Color[] colors)
        {
            List<byte> args = new List<byte>();
            args.Add(seconds);
            foreach (var item in colors)
            {
                args.AddRange(new byte[] { item.R, item.G, item.B });
            }

            return GetDefaultOperationAddr(OPCode.ColorSortedSecuenceWrite, args.ToArray());
        }

        public Operation ColorRead() { return GetDefaultOperationAddr(OPCode.ColorRead, new byte[] { }); }

        public Operation PresenceRead() { return GetDefaultOperationAddr(OPCode.PresenceRead, new byte[] { }); }

        public Operation TemperatureRead() { return GetDefaultOperationAddr(OPCode.TemperatureRead, new byte[] { }); }

        public Operation HumidityRead() { return GetDefaultOperationAddr(OPCode.HumidityRead, new byte[] { }); }

        public Operation PowerRead() { return GetDefaultOperationAddr(OPCode.PowerRead, new byte[] { }); }

        public Operation LuminosityRead() { return GetDefaultOperationAddr(OPCode.LuminosityRead, new byte[] { }); }

        //EXTENSION CODE NOT IMPLEMENTED
        #endregion

    }
}
