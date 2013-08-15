
namespace SmartHome.Products
{
    public class PinPort
    {
        //Direction
        public char Port { set; get; }
        public byte Pin { set; get; }

        public PinPort(char Port, byte Pin)
        {
            this.Port = Port;
            this.Pin = Pin;
        }

        public PinPort(string direction)
        {
            Port = direction[0];
            Pin = byte.Parse(direction[1].ToString());
        }

        public PinPort(int pinPortNumber)
        {
            Port = (char)((pinPortNumber/8) + 'A');
            Pin = (byte)(pinPortNumber % 8);
        }

        public PinPort(byte port, byte pin)
        {
            Port = (char)(port + 'A');
            Pin = pin;
        }

        public byte GetPinPortNumber()
        {
            return (byte)((Port-'A') * 8 + Pin);
        }

        public override bool Equals(object obj)
        {
            var p1 = obj as PinPort;
            return p1 != null && p1.Pin == this.Pin && p1.Port == this.Port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
