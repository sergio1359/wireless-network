#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
#endregion

namespace SerialPortManager.ConnectionManager
{
    public class NodeConnection
    {
        #region Enums
        public enum ConnectionStates
        {
            Starting,
            Identifying,
            Connected,
            NotConnected
        }

        public enum NodeTypes
        {
            NotANode,
            Master,
            Slave
        }

        private enum SerialReceiverStates
        {
            IDLE_RX_STATE,
            MAGIC_RX_STATE,
            SOF_RX_STATE,
            DATA_RX_STATE,
            EOF_RX_STATE,
            ERROR_RX_STATE
        };
        #endregion

        private SerialPort serialPort;

        private SerialReceiverStates receiverState;
        private List<byte> currentReceiverMessage;
        private byte checkSum;

        public delegate void OperationEventHandler(object sender, InputMessage message);
        public event OperationEventHandler OperationReceived;

        public delegate void ConnectionStateHandler(object sender, ConnectionStates newState);
        public event ConnectionStateHandler ConnectionStateChanged;

        private ConnectionStates connectionState;

        #region Properties
        public ConnectionStates ConnectionState
        {
            private set
            {
                if (connectionState == value)
                    return;

                connectionState = value;
                
                if(ConnectionStateChanged != null)
                    ConnectionStateChanged(this, connectionState);
            }
            get
            {
                return connectionState;
            }
        }

        public NodeTypes NodeType
        {
            private set;
            get;
        }

        public ushort NodeAddress
        {
            private set;
            get;
        }

        public String PortName
        {
            get
            {
                if (serialPort == null)
                    return null;
                else
                    return serialPort.PortName;
            }
        } 
        #endregion

        #region Constants
        // Magic symbol to start SOF end EOF sequences with. Should be duplicated if
        // occured inside the message.
        const byte APP_MAGIC_SYMBOL = 0x10;
        static byte[] SOF = { APP_MAGIC_SYMBOL, 0x02 };
        static byte[] EOF = { APP_MAGIC_SYMBOL, 0x03 };

        const ushort MasterAddress = 0x4003;

        #endregion

        public NodeConnection(string PortName)
        {
            serialPort = new SerialPort(PortName, 38400);
            serialPort.DataReceived += serialPort_DataReceived;

            currentReceiverMessage = new List<byte>();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte c;

            while (serialPort.BytesToRead > 0)
            {
                bool acceptByte = false;

                c = (byte)serialPort.ReadByte();

                switch (receiverState)
                {
                    case SerialReceiverStates.IDLE_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            receiverState = SerialReceiverStates.SOF_RX_STATE;
                            currentReceiverMessage.Clear();
                            checkSum = 0;
                        }
                        break;

                    case SerialReceiverStates.SOF_RX_STATE:
                        if (SOF[1] == c)
                        {
                            receiverState = SerialReceiverStates.DATA_RX_STATE;
                        }
                        else
                        {
                            receiverState = SerialReceiverStates.IDLE_RX_STATE;
                        }
                        break;

                    case SerialReceiverStates.DATA_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            receiverState = SerialReceiverStates.MAGIC_RX_STATE;
                        }
                        else
                        {
                            acceptByte = true;
                        }
                        break;

                    case SerialReceiverStates.MAGIC_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            receiverState = SerialReceiverStates.DATA_RX_STATE;
                            acceptByte = true;
                        }
                        else if (EOF[1] == c)
                        {
                            receiverState = SerialReceiverStates.EOF_RX_STATE;
                        }
                        else
                        {
                            receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            Debug.Print("INVALID FRAME RECEIVED IN " + PortName);
                        }
                        break;

                    case SerialReceiverStates.EOF_RX_STATE:
                        if (checkSum == c)
                        {
                            receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            InputMessage inputMessage = new InputMessage();
                            inputMessage.FromBinary(currentReceiverMessage.ToArray());

                            OnOperationReceived(inputMessage);
                        }
                        else
                        {
                            receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            Debug.Print("INVALID CHECKSUM RECEIVED IN " + PortName);
                        }
                        break;

                    default:
                        break;
                }

                checkSum += c;

                if (acceptByte)
                {
                    currentReceiverMessage.Add(c);
                }
            }
        }

        #region Public Methods
        public void Identify()
        {
            this.ConnectionState = ConnectionStates.Identifying;

            this.SendOperation(new Operation()
            {
                OpCode = Operation.OPCodes.MacRead,
            });
        }

        public bool SendOperation(Operation operation)
        {
            OutputMessage outputMessage = new OutputMessage()
            {
                SecurityEnabled = true,
                RoutingEnabled = true,
                EndPoint = 1,
                Retries = 3,
                Content = operation
            };

            return SendData(outputMessage.ToBinary());
        } 
        #endregion

        #region Private Methods
        private bool SendData(byte[] buffer)
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

            if (!serialPort.IsOpen)
                serialPort.Open();

            try
            {
                serialPort.Write(frame.ToArray(), 0, frame.Count);

                if (ConnectionState == ConnectionStates.NotConnected)
                    ConnectionState = ConnectionStates.Connected;

                return true;
            }
            catch (InvalidOperationException)
            {
                ConnectionState = ConnectionStates.NotConnected;
                return false;
            }
        }

        private void OnOperationReceived(InputMessage oper)
        {
            if (ConnectionState == ConnectionStates.Identifying)
            {
                if (oper.Content.OpCode == Operation.OPCodes.MacReadResponse)
                {
                    this.NodeAddress = oper.Content.SourceAddress;
                    this.NodeType = this.NodeAddress == MasterAddress ? NodeTypes.Master : NodeTypes.Slave;
                    this.ConnectionState = ConnectionStates.Connected;
                }
            }
            
            if (OperationReceived != null)
                OperationReceived(this, oper);
        }
        #endregion
    }
}
