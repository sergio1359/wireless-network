using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

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

        // Magic symbol to start SOF end EOF sequences with. Should be duplicated if
        // occured inside the message.
        const byte APP_MAGIC_SYMBOL = 0x10;
        byte[] SOF = {APP_MAGIC_SYMBOL, 0x02};
        byte[] EOF = {APP_MAGIC_SYMBOL, 0x03};

        SerialPort serial = new SerialPort();
        SerialReceiverState serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
        List<byte> currentMessage = new List<byte>();
        byte CheckSum;

        public Form1()
        {
            InitializeComponent();

            comboBox1_MouseClick(this, null);
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
        }

        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
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
                    button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = true;
                }
            }
            else
            {
                serial.Close();

                button1.Text = "Open";
                comboBox1.Enabled = true;
                button2.Enabled = button3.Enabled = button4.Enabled = button5.Enabled = button6.Enabled = false;
            }
        }

        private void buttonCmd_Click(object sender, EventArgs e)
        {
            if (sender == button2)
            {
                /*SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x02 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4004, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x02 } }.ToBinary());*/

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x40, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());
                
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x41, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x42, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x43, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());

                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x00, OpCode = 0x40, Args = new byte[] { 0x44, 0x03, 0x01, 0x02, 0x03 } }.ToBinary());
            }
            else if (sender == button3)
            {
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4002, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x00 } }.ToBinary());
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
                SendData(new Operation() { SourceAddress = 0x00, DestinationAddress = 0x4005, OpCode = 0x06, Args = new byte[] { 0x03, 0x40, 0x00 } }.ToBinary());
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
                        PrintOperation(currentMessage.ToArray());
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

        void PrintOperation(byte[] operation)
        {
            this.BeginInvoke((Action)(() =>
            {
                StringBuilder sb = new StringBuilder(operation.Length * 2);
                sb.Append("[" + DateTime.Now.ToLongTimeString() + "]   ->   ");
                foreach (byte b in operation)
                {
                    sb.AppendFormat("0x{0:X2} ", b);
                }
                listBox1.Items.Add(sb.ToString());
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }
    }
}
