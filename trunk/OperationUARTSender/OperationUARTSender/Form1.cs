using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace OperationUARTSender
{
    public partial class Form1 : Form
    {
        enum SerialReceiverState
        {
            USART_RECEIVER_IDLE_RX_STATE,
            USART_RECEIVER_MAGIC_RX_STATE,
            USART_RECEIVER_SOF_RX_STATE,
            USART_RECEIVER_DATA_RX_STATE,
            USART_RECEIVER_EOF_RX_STATE,
            USART_RECEIVER_ERROR_RX_STATE
        };

        enum ConfigErrorCodes
        {
            OK = 0,
            ERROR_FRAGMENT_TOTAL_NOT_EXPECTED,
            ERROR_FRAGMENT_ORDER,
            ERROR_WAITING_FIRST_FRAGMENT,
            ERROR_CONFIG_SIZE_TOO_BIG,
            ERROR_CONFIG_INVALID_CHECKSUM,
            ERROR_CONFIG_SIZE_NOT_EXPECTED,
            ERROR_BUSY_RECEIVING_STATE
        };

        enum DayOfWeeks
        {
            SUN = 1,
            SAT = 2,
            FRI = 4,
            THU = 8,
            WED = 16,
            TUE = 32,
            MON = 64,
            UNK = 128
        };

        // Magic symbol to start SOF end EOF sequences with. Should be duplicated if
        // occured inside the message.
        const byte APP_MAGIC_SYMBOL = 0x10;
        byte[] SOF = { APP_MAGIC_SYMBOL, 0x02 };
        byte[] EOF = { APP_MAGIC_SYMBOL, 0x03 };

        SerialPort serial = new SerialPort();
        SerialReceiverState serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
        List<byte> currentMessage = new List<byte>();
        byte CheckSum;

        event EventHandler OperationReceived;

        ushort CurrentAddress
        {
            get
            {
                return Convert.ToUInt16(textBoxConfigAddress.Text, 16);
            }
        }

        void OnOperationReceived(Operation oper)
        {
            if (OperationReceived != null)
                OperationReceived(oper, null);
        }

        public Form1()
        {
            InitializeComponent();

            comboBox1_MouseClick(this, null);
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            OperationReceived += new EventHandler(Form1_OperationReceived);
        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                button1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Open")
            {
                if (comboBox1.SelectedIndex > 0)
                {
                    serial.BaudRate = 38400;
                    serial.PortName = comboBox1.Text;
                    serial.Open();

                    button1.Text = "Close";
                    comboBox1.Enabled = false;
                    button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = button7.Enabled = button8.Enabled = true;
                }
            }
            else
            {
                serial.Close();

                button1.Text = "Open";
                comboBox1.Enabled = true;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = button7.Enabled = button8.Enabled = false;
            }
        }

        private void buttonCmd_Click(object sender, EventArgs e)
        {
            if (sender == button2)
            {
                /*SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x02 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4004, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x02 } }.ToBinary());*/

                /*
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x20, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());
                
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x21, 0x03, 0x04, 0x05, 0x06 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x22, 0x03, 0x07, 0x08, 0x09 } }.ToBinary());*/

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x5A, Args = new byte[] { 0x00 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x5C, Args = new byte[] { 0x00 } }.ToBinary());
            }
            else if (sender == button3)
            {
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x02 } }.ToBinary());
            }
            else if (sender == button4)
            {
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4003, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x00 } }.ToBinary());
            }
            else if (sender == button5)
            {
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4004, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x00 } }.ToBinary());
            }
            else if (sender == button6)
            {
                //SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x44, Args = new byte[] { 0x03, 0x40, 0x00 } }.ToBinary());

                //SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x07, Args = new byte[] { 0x03 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x21, Args = new byte[] { } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x24, Args = new byte[] { } }.ToBinary());
            }
        }

        void Form1_OperationReceived(object sender, EventArgs e)
        {
            Operation operation = sender as Operation;

            if (operation.OpCode == 0x41)
            {
                if (operation.Args[1] == 0x00)
                {
                    configWriteBuffer.RemoveAt(0);
                    if (configWriteBuffer.Count > 0)
                    {
                        if(configWriteBuffer.Count == 1)
                            PrintMessage("CONFIG. UPDATE DONE");

                        SendData(configWriteBuffer[0].ToBinary());
                    }
                }
                else
                {
                    configWriteBuffer.Clear();
                    PrintMessage("CONFIG. ERROR CODE: " + Enum.GetName(typeof(ConfigErrorCodes), (object)operation.Args[1]));
                }
            }
            else if (operation.OpCode == 0x5B || operation.OpCode == 0x5D)
            {
                if (operation.Args[1] == 0xFF)
                {
                    PrintMessage(String.Format("SENSOR {1} FROM 0x{0:X4} UNKNOWN",
                        operation.SourceAddress,
                        operation.Args[0]));
                }
                else
                {
                    string baseStr = operation.OpCode == 0x5B ?
                        "TEMPERATURE RECEIVED FROM 0x{0:X4} SENSOR {1}: {2}ºC" :
                        "HUMIDITY RECEIVED FROM 0x{0:X4} SENSOR {1}: {2}%";

                    PrintMessage(String.Format(baseStr, operation.SourceAddress, operation.Args[0], operation.Args[1]));
                }
            }
            else if (operation.OpCode == 0x45)
            {
                PrintMessage(String.Format("CHECKSUM RECEIVED FROM 0x{0:X4}: 0x{1:X4}",
                    operation.SourceAddress,
                    ((ushort)operation.Args[1])<<8 | operation.Args[0]));
            }
            else if (operation.OpCode == 0x08)
            {
                PrintMessage(String.Format("DIGITAL READ FROM 0x{0:X4} PORT_{1}: 0x{2:X2}",
                    operation.SourceAddress,
                    (char)('A' + operation.Args[0]),
                    operation.Args[1]));
            }
            else if (operation.OpCode == 0x22)
            {
                PrintMessage(String.Format("TIME READ FROM 0x{0:X4} ->  {1:d2}:{2:d2}:{3:d2}",
                    operation.SourceAddress,
                    operation.Args[0],
                    operation.Args[1],
                    operation.Args[2]));
            }
            else if (operation.OpCode == 0x25)
            {
                PrintMessage(String.Format("DATE READ FROM 0x{0:X4} -> {1}  {2:d2}/{3:d2}/{4:d4}",
                    operation.SourceAddress,
                    Enum.GetName(typeof(DayOfWeeks), (object)operation.Args[0]),
                    operation.Args[1],
                    operation.Args[2],
                    ((ushort)operation.Args[4])<<8 | operation.Args[3]));
            }
            else
            {
                PrintOperation(operation);
            }
        }

        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte c;

            while (serial.BytesToRead > 0)
            {
                bool acceptByte = false;

                c = (byte)serial.ReadByte();

                switch (serialState)
                {
                    case SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_SOF_RX_STATE;
                            currentMessage.Clear();
                            CheckSum = 0;
                        }
                        break;

                    case SerialReceiverState.USART_RECEIVER_SOF_RX_STATE:
                        if (SOF[1] == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_DATA_RX_STATE;
                        }
                        else
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
                        }
                        break;

                    case SerialReceiverState.USART_RECEIVER_DATA_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_MAGIC_RX_STATE;
                        }
                        else
                        {
                            acceptByte = true;
                        }
                        break;

                    case SerialReceiverState.USART_RECEIVER_MAGIC_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_DATA_RX_STATE;
                            acceptByte = true;
                        }
                        else if (EOF[1] == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_EOF_RX_STATE;
                        }
                        else
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
                            PrintMessage("TRAMA DEFECTUOSA");
                        }
                        break;

                    case SerialReceiverState.USART_RECEIVER_EOF_RX_STATE:
                        if (CheckSum == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
                            Operation op = new Operation();
                            op.FromBinary(currentMessage.ToArray());
                            OnOperationReceived(op);
                        }
                        else
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
                            PrintMessage("CHECKSUM INCORRECTO");
                        }
                        break;

                    default:
                        break;
                }

                CheckSum += c;

                if (acceptByte)
                {
                    currentMessage.Add(c);
                }
            }
        }

        public void SendData(byte[] buffer)
        {
            List<byte> frame = new List<byte>();
            byte cs = 0;

            frame.Add(SOF[0]);
            frame.Add(SOF[1]);

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == APP_MAGIC_SYMBOL)
                {
                    frame.Add(APP_MAGIC_SYMBOL);
                    cs += APP_MAGIC_SYMBOL;
                }
                frame.Add(buffer[i]);
                cs += buffer[i];
            }

            frame.Add(EOF[0]);
            frame.Add(EOF[1]);

            cs += SOF[0];
            cs += SOF[1];
            cs += EOF[0];
            cs += EOF[1];

            frame.Add(cs);

            serial.Write(frame.ToArray(), 0, frame.Count);
        }

        const int MAX_CONTENT_SIZE = 50;
        List<Operation> configWriteBuffer = new List<Operation>();

        void SendConfigFile(string inputFilename, ushort destinationAddress)
        {
            byte[] fileBytes = File.ReadAllBytes(inputFilename);

            configWriteBuffer.Clear();
            byte numberOfFrames = (byte)(fileBytes.Length / MAX_CONTENT_SIZE);
            int frameSize = 0;
            for (byte i = 0; i <= numberOfFrames; i++)
            {
                Operation currentOp = new Operation()
                {
                    SourceAddress = 0x00,
                    DestinationAddress = destinationAddress,
                    OpCode = 0x40,
                    Args = new byte[MAX_CONTENT_SIZE + 2]
                };

                frameSize = Math.Min(MAX_CONTENT_SIZE, fileBytes.Length - (i * MAX_CONTENT_SIZE));
                currentOp.Args[0] = (byte)((numberOfFrames << 4) | (i & 0xF));
                currentOp.Args[1] = (byte)frameSize;
                Buffer.BlockCopy(fileBytes, (i * MAX_CONTENT_SIZE), currentOp.Args, 2, frameSize);

                configWriteBuffer.Add(currentOp);
            }

            SendData(configWriteBuffer[0].ToBinary());
        }

        void PrintMessage(string message)
        {
            this.BeginInvoke((Action)(() =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[" + DateTime.Now.ToLongTimeString() + "]   ->   ");
                sb.Append(message);
                listBox1.Items.Add(sb.ToString());
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        void PrintOperation(Operation operation)
        {
            this.BeginInvoke((Action)(() =>
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[" + DateTime.Now.ToLongTimeString() + "]   ->   ");
                sb.AppendFormat("FROM: 0x{0:X4}   TO: 0x{1:X4}   OPCODE: 0x{2:X2}   ARGS: ",
                    operation.SourceAddress,
                    operation.DestinationAddress,
                    operation.OpCode);
                foreach (byte b in operation.Args)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }
                listBox1.Items.Add(sb.ToString());
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var res = openFileDialog1.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                ushort address = Convert.ToUInt16(textBoxConfigAddress.Text, 16);

                SendConfigFile(openFileDialog1.FileName, address);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var dat = DateTime.Now;
            var dow = (int)Enum.Parse(typeof(DayOfWeeks), dat.DayOfWeek.ToString().ToUpper().Substring(0,3));

            SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x20, Args = new byte[] { (byte)dat.Hour, (byte)dat.Minute, (byte)dat.Second } }.ToBinary());
            SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = 0x23, Args = new byte[] { (byte)dow, (byte)dat.Day, (byte)dat.Month, (byte)dat.Year, (byte)(dat.Year>>8) } }.ToBinary());
        }
    }
}
