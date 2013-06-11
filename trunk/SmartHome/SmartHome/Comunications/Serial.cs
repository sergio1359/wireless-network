using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace SmartHome.Comunications
{
    public class SerialSingleton
    {
        private static Serial serial;

        public static Serial Serial
        {
            get
            {
                if (serial == null)
                {
                    serial = new Serial();
                }
                return serial;
            }
        }
    }

    public class Serial : IConnexion
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
        byte[] SOF = { APP_MAGIC_SYMBOL, 0x02 };
        byte[] EOF = { APP_MAGIC_SYMBOL, 0x03 };

        SerialPort serial = new SerialPort();
        SerialReceiverState serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
        List<byte> currentMessage = new List<byte>();
        byte CheckSum;

        public Serial()
        {
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);
            serial.BaudRate = 38400;
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
                            serialState = SerialReceiverState.USART_RECEIVER_ERROR_RX_STATE;
                            //listBox1.Items.Add("TRAMA DEFECTUOSA");
                        }
                        break;

                    case SerialReceiverState.USART_RECEIVER_EOF_RX_STATE:
                        if (CheckSum == c)
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_IDLE_RX_STATE;
                            //PrintOperation(currentMessage.ToArray());
                        }
                        else
                        {
                            serialState = SerialReceiverState.USART_RECEIVER_ERROR_RX_STATE;
                            //listBox1.Items.Add("CHECKSUM INCORRECTO");
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
    }
}