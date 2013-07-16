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
        #region Enumerators
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

        enum OPCode : byte
        {
            Reset = 0x00,
            FirmwareVersionRead,
            FirmwareVersionReadResponse,
            ShieldModelRead,
            ShieldModelResponse,
            BaseModelRead,
            BaseModelResponse,
            ConfigWrite,
            ConfigWriteResponse,
            ConfigRead,
            ConfigReadResponse,
            ConfigReadConfirmation,
            ConfigChecksumRead,
            ConfigChecksumResponse,

            MacRead = 0x20,
            MacReadResponse,
            NextHopRead,
            NextHopReadResponse,
            RouteTableRead,
            RouteTableReadResponse,
            RouteTableReadConfirmation,

            DateTimeWrite = 0x30,
            DateTimeRead,
            DateTimeReadResponse,

            Warning = 0x3E,
            Error = 0x3F,

            LogicWrite = 0x40,
            LogicSwitch,
            LogicRead,
            LogicReadResponse,

            ColorWrite = 0x50,
            ColorWriteRandom = 0x51,
            ColorSecuenceWrite = 0x52,
            ColorSortedSecuenceWrite = 0x53,
            ColorRead = 0x54,
            ColorReadResponse = 0x55,

            PresenceRead = 0x57,
            PresenceReadResponse = 0x58,

            TemperatureRead = 0x5A,
            TemperatureReadResponse = 0x5B,
            HumidityRead = 0x5C,
            HumidityReadResponse = 0x5D,

            PowerRead = 0x60,
            PowerReadResponse = 0x61,

            LuminosityRead = 0x63,
            LuminosityReadResponse = 0x64,

            PCOperation = 0xFE,
            Extension = 0xFF,
        } 
        #endregion

        // Magic symbol to start SOF end EOF sequences with. Should be duplicated if
        // occured inside the message.
        const byte APP_MAGIC_SYMBOL = 0x10;
        byte[] SOF = { APP_MAGIC_SYMBOL, 0x02 };
        byte[] EOF = { APP_MAGIC_SYMBOL, 0x03 };

        SerialPort serial = new SerialPort();
        SerialReceiverState serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
        List<byte> currentMessage = new List<byte>();
        byte CheckSum;

        const int MAX_CONTENT_SIZE = 50;
        List<Operation> configWriteBuffer = new List<Operation>();
        List<Operation> configReadBuffer = new List<Operation>();

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

        public void SendOperation(Operation operation)
        {
            OutputMessage outputMessage = new OutputMessage()
            {
                SecurityEnabled = true,
                RoutingEnabled = true,
                EndPoint = 1,
                Retries = 3,
                Content = operation
            };

            SendData(outputMessage.ToBinary());
        }

        public void SendConfigFile(string inputFilename, ushort destinationAddress)
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
                    OpCode = (byte)OPCode.ConfigWrite,
                    Args = new byte[MAX_CONTENT_SIZE + 2]
                };

                frameSize = Math.Min(MAX_CONTENT_SIZE, fileBytes.Length - (i * MAX_CONTENT_SIZE));
                currentOp.Args[0] = (byte)((numberOfFrames << 4) | (i & 0xF));
                currentOp.Args[1] = (byte)frameSize;
                Buffer.BlockCopy(fileBytes, (i * MAX_CONTENT_SIZE), currentOp.Args, 2, frameSize);

                configWriteBuffer.Add(currentOp);
            }

            SendOperation(configWriteBuffer[0]);
        }

        #region Private Methods
        private void SendData(byte[] buffer)
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

        private void SaveConfigFile()
        {
            List<byte> rawBytes = new List<byte>();

            foreach (Operation op in configReadBuffer)
            {
                rawBytes.AddRange(op.Args.Skip(2));
            }

            //TODO: Calculate Checksum
            ushort configLength = (ushort)((((ushort)rawBytes[2]) << 8) | (ushort)rawBytes[1]);

            if (configLength == rawBytes.Count)
            {
                this.BeginInvoke((Action)(() =>
                {
                    var res = saveFileDialog1.ShowDialog();

                    if (res == System.Windows.Forms.DialogResult.OK)
                    {
                        System.IO.File.WriteAllBytes(saveFileDialog1.FileName, rawBytes.ToArray());
                    }
                }));
            }
            else
            {
                PrintMessage("CONFIG ER9ROR. UNEXPECTED SIZE");
            }
        }

        private void PrintMessage(string message)
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

        private void PrintOperation(Operation operation)
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
        #endregion

        #region EventCallbacks
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

                    foreach (var control in this.Controls)
                    {
                        if (control is Button && control != button1)
                        {
                            ((Button)control).Enabled = true;
                        }
                    }
                }
            }
            else
            {
                serial.Close();

                button1.Text = "Open";
                comboBox1.Enabled = true;

                foreach (var control in this.Controls)
                {
                    if (control is Button && control != button1)
                    {
                        ((Button)control).Enabled = false;
                    }
                }
            }
        }

        private void buttonCmd_Click(object sender, EventArgs e)
        {
            if (sender == buttonDigSwitch)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.LogicSwitch, Args = new byte[] { 0x03, 0x40, 0x00 } });
            }
            else if (sender == buttonSwitchTime)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.LogicSwitch, Args = new byte[] { 0x03, 0x40, 0x02 } });
            }
            else if (sender == buttonDigRead)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.LogicRead, Args = new byte[] { 0x03, 0x40 } });
            }
            else if (sender == buttonCheckSum)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.ConfigChecksumRead, Args = new byte[] { } });
            }
            else if (sender == buttonTemp)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.TemperatureRead, Args = new byte[] { 0x00 } });
            }
            else if (sender == buttonHum)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.HumidityRead, Args = new byte[] { 0x00 } });
            }
            else if (sender == buttonDateTime)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.DateTimeRead, Args = new byte[] { } });
            }
            else if (sender == buttonMAC)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.MacRead, Args = new byte[] { } });
            }
            else if (sender == buttonFirm)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.FirmwareVersionRead, Args = new byte[] { } });
            }
            else if (sender == buttonReset)
            {
                SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.Reset, Args = new byte[] { } });
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var res = openFileDialog1.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK)
            {
                SendConfigFile(openFileDialog1.FileName, CurrentAddress);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var dat = DateTime.Now;
            var dow = (int)Enum.Parse(typeof(DayOfWeeks), dat.DayOfWeek.ToString().ToUpper().Substring(0, 3));

            SendOperation(new Operation()
            {
                SourceAddress = 0x00,
                DestinationAddress = CurrentAddress,
                OpCode = (byte)OPCode.DateTimeWrite,
                Args = new byte[] { (byte)dow, (byte)dat.Day, (byte)dat.Month, (byte)dat.Year, (byte)(dat.Year >> 8), (byte)dat.Hour, (byte)dat.Minute, (byte)dat.Second }
            });
        }

        private void button9_Click(object sender, EventArgs e)
        {
            configReadBuffer.Clear();

            SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.ConfigRead, Args = new byte[] { } });
        }

        void Form1_OperationReceived(object sender, EventArgs e)
        {
            Operation operation = sender as Operation;

            if (operation.OpCode == (byte)OPCode.ConfigWriteResponse)
            {
                if (configWriteBuffer.Count > 0)
                {
                    if (operation.Args[0] == configWriteBuffer[0].Args[0])
                    {
                        if (operation.Args[1] == 0x00)
                        {
                            configWriteBuffer.RemoveAt(0);
                            if (configWriteBuffer.Count > 0)
                            {
                                if (configWriteBuffer.Count == 1)
                                    PrintMessage(String.Format("CONFIG. UPDATE DONE TO 0x{0:X4}", operation.SourceAddress));

                                SendOperation(configWriteBuffer[0]);
                            }
                        }
                        else
                        {
                            configWriteBuffer.Clear();
                            PrintMessage("CONFIG. ERROR CODE: " + Enum.GetName(typeof(ConfigErrorCodes), (object)operation.Args[1]));
                        }
                    }
                    else
                    {
                        int rcvFragment = operation.Args[0] & 0x0F;
                        int rcvTotalFragment = (operation.Args[0] & 0xF0) >> 4;
                        int expFragment = configWriteBuffer[0].Args[0] & 0x0F;
                        int expTotalFragment = (configWriteBuffer[0].Args[0] & 0xF0) >> 4;

                        PrintMessage(String.Format("CONFIG. INVALID PROTOCOL: RECEIVED({0}/{1}) EXPECTED({2}/{3})",
                            rcvFragment, rcvTotalFragment, expFragment, expTotalFragment));
                        configWriteBuffer.Clear();
                    }
                }
            }
            else if (operation.OpCode == (byte)OPCode.TemperatureReadResponse || operation.OpCode == (byte)OPCode.HumidityReadResponse)
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
            else if (operation.OpCode == (byte)OPCode.ConfigChecksumResponse)
            {
                PrintMessage(String.Format("CHECKSUM RECEIVED FROM 0x{0:X4}: 0x{1:X4}",
                    operation.SourceAddress,
                    ((ushort)operation.Args[1]) << 8 | operation.Args[0]));
            }
            else if (operation.OpCode == (byte)OPCode.LogicReadResponse)
            {
                PrintMessage(String.Format("LOGIC READ FROM 0x{0:X4} PORT_{1}: 0x{2:X2}",
                    operation.SourceAddress,
                    (char)('A' + operation.Args[0]),
                    operation.Args[1]));
            }
            else if (operation.OpCode == (byte)OPCode.DateTimeReadResponse)
            {
                string DateStr = String.Format("{1}  {2:d2}/{3:d2}/{4:d4}",
                    Enum.GetName(typeof(DayOfWeeks), (object)operation.Args[0]),
                    operation.Args[1],
                    operation.Args[2],
                    ((ushort)operation.Args[4]) << 8 | operation.Args[3]);

                string TimeStr = String.Format(" {1:d2}:{2:d2}:{3:d2}",
                    operation.Args[5],
                    operation.Args[6],
                    operation.Args[7]);

                PrintMessage(String.Format("DATETIME READ FROM 0x{0:X4} -> ", operation.SourceAddress) + DateStr + " " + TimeStr);
            }
            else if (operation.OpCode == (byte)OPCode.MacReadResponse)
            {
                PrintMessage(String.Format("MAC FROM 0x{0:X4} -> 0x{1:X2}-0x{2:X2}-0x{3:X2}-0x{4:X2}-0x{5:X2}-0x{6:X2}",
                    operation.SourceAddress,
                    operation.Args[0],
                    operation.Args[1],
                    operation.Args[2],
                    operation.Args[3],
                    operation.Args[4],
                    operation.Args[5]));
            }
            else if (operation.OpCode == (byte)OPCode.FirmwareVersionReadResponse)
            {
                PrintMessage(String.Format("FIRMWARE VERSION FROM 0x{0:X4} -> {1}",
                    operation.SourceAddress,
                    operation.Args[0]));
            }
            else if (operation.OpCode == (byte)OPCode.ConfigReadResponse)
            {
                int fragment = operation.Args[0] & 0x0F;
                int totalFragment = (operation.Args[0] & 0xF0) >> 4;

                Operation lastOp = null;
                if (configReadBuffer.Count > 0)
                    lastOp = configReadBuffer.Last();

                if (lastOp == null || (lastOp != null && operation.Args[0] == lastOp.Args[0] + 1))
                {
                    PrintMessage(String.Format("CONFIG READ RESPONSE FROM 0x{0:X4} -> {1}/{2} OK",
                        operation.SourceAddress,
                        fragment,
                        totalFragment));

                    configReadBuffer.Add(operation);
                    SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.ConfigReadConfirmation, Args = new byte[] { operation.Args[0], 0x00 } });

                    if (fragment == totalFragment)
                    {
                        SaveConfigFile();
                    }
                }
                else
                {
                    PrintMessage(String.Format("CONFIG READ RESPONSE FROM 0x{0:X4} -> {1}/{2} ERROR",
                        operation.SourceAddress,
                        fragment,
                        totalFragment));

                    //Send error code back
                    SendOperation(new Operation() { SourceAddress = 0x00, DestinationAddress = CurrentAddress, OpCode = (byte)OPCode.ConfigReadConfirmation, Args = new byte[] { operation.Args[0], 0x01 } });
                }
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
                            InputMessage inputMessage = new InputMessage();
                            inputMessage.FromBinary(currentMessage.ToArray());

                            OnOperationReceived(inputMessage.Content);
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

        #endregion

    }
}
