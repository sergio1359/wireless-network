#region Using Statements
using SerialPortManager.ConnectionManager;
using SmartHome.Comunications.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Timers;
#endregion

namespace SmartHome.Communications.SerialManager
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

        #region Private Vars
        private SerialPort serialPort;

        private SerialReceiverStates receiverState;
        private List<byte> currentReceiverMessage;
        private byte checkSum;

        private TaskCompletionSource<bool> pendingConfirmationTask;

        private ConnectionStates connectionState;

        private Timer identifyTimeoutTimer;
        private int retriesCount;
        #endregion

        public event EventHandler<InputHeader> OperationReceived;
        public event EventHandler<ConnectionStates> ConnectionStateChanged;

        #region Properties
        public ConnectionStates ConnectionState
        {
            private set
            {
                if (connectionState == value)
                    return;

                connectionState = value;

                if (ConnectionStateChanged != null)
                    ConnectionStateChanged(this, connectionState);
            }
            get
            {
                return connectionState;
            }
        }

        public bool Identified
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

        const int IDENTIFY_MAX_RETRIES = 4;
        const int MILLISECONDS_BW_RETRIES = 2000;
        #endregion

        public NodeConnection(string PortName)
        {
            this.serialPort = new SerialPort(PortName, 38400);
            this.serialPort.DataReceived += serialPort_DataReceived;

            this.identifyTimeoutTimer = new Timer()
            {
                Interval = MILLISECONDS_BW_RETRIES,
                AutoReset = false,
            };
            this.identifyTimeoutTimer.Elapsed += identifyRetryTimer_Elapsed;

            this.currentReceiverMessage = new List<byte>();
        }
       
        #region Private Methods
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte c;

            while (this.serialPort.BytesToRead > 0)
            {
                bool acceptByte = false;

                c = (byte)this.serialPort.ReadByte();

                switch (this.receiverState)
                {
                    case SerialReceiverStates.IDLE_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            this.receiverState = SerialReceiverStates.SOF_RX_STATE;
                            this.currentReceiverMessage.Clear();
                            this.checkSum = 0;
                        }
                        break;

                    case SerialReceiverStates.SOF_RX_STATE:
                        if (SOF[1] == c)
                        {
                            this.receiverState = SerialReceiverStates.DATA_RX_STATE;
                        }
                        else
                        {
                            this.receiverState = SerialReceiverStates.IDLE_RX_STATE;
                        }
                        break;

                    case SerialReceiverStates.DATA_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            this.receiverState = SerialReceiverStates.MAGIC_RX_STATE;
                        }
                        else
                        {
                            acceptByte = true;
                        }
                        break;

                    case SerialReceiverStates.MAGIC_RX_STATE:
                        if (APP_MAGIC_SYMBOL == c)
                        {
                            this.receiverState = SerialReceiverStates.DATA_RX_STATE;
                            acceptByte = true;
                        }
                        else if (EOF[1] == c)
                        {
                            this.receiverState = SerialReceiverStates.EOF_RX_STATE;
                        }
                        else
                        {
                            this.receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            Debug.Print("INVALID FRAME RECEIVED IN " + this.PortName);
                        }
                        break;

                    case SerialReceiverStates.EOF_RX_STATE:
                        if (checkSum == c)
                        {
                            this.receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            InputHeader inputMessage = new InputHeader();
                            inputMessage.FromBinary(this.currentReceiverMessage.ToArray());

                            OnOperationReceived(inputMessage);
                        }
                        else
                        {
                            this.receiverState = SerialReceiverStates.IDLE_RX_STATE;
                            Debug.Print("INVALID CHECKSUM RECEIVED IN " + this.PortName);
                        }
                        break;

                    default:
                        break;
                }

                checkSum += c;

                if (acceptByte)
                {
                    this.currentReceiverMessage.Add(c);
                }
            }
        }

        private void identifyRetryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.ConnectionState == ConnectionStates.Identifying && this.retriesCount < IDENTIFY_MAX_RETRIES)
            {
                this.Identify(false);
                this.retriesCount++;
            }
        }

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

            if (!this.serialPort.IsOpen)
                this.serialPort.Open();

            try
            {
                this.serialPort.Write(frame.ToArray(), 0, frame.Count);

                if (this.ConnectionState == ConnectionStates.NotConnected)
                    this.ConnectionState = ConnectionStates.Connected;

                return true;
            }
            catch (InvalidOperationException)
            {
                this.ConnectionState = ConnectionStates.NotConnected;
                return false;
            }
        }

        private void OnOperationReceived(InputHeader operation)
        {
            if (operation.Content.SourceAddress == 0)
                operation.Content.SourceAddress = this.NodeAddress;

            if (ConnectionState == ConnectionStates.Identifying &&
                operation.Content is OperationMessage &&
                ((OperationMessage)operation.Content).OpCode == OperationMessage.OPCodes.PingResponse)
            {
                // Ping response for Indentify
                this.NodeAddress = ((OperationMessage)operation.Content).SourceAddress;
                this.Identified = true;
                this.ConnectionState = ConnectionStates.Connected;
            }
            else if (pendingConfirmationTask != null && !pendingConfirmationTask.Task.IsCompleted && operation.Confirmation != InputHeader.ConfirmationType.None)
            {
                // Data confirmation response
                pendingConfirmationTask.SetResult(operation.Confirmation == InputHeader.ConfirmationType.Ok);
            }
            else
            {
                // Real operation responde
                if (OperationReceived != null)
                    OperationReceived(this, operation);
            }
        }
        #endregion

        #region Public Methods
        public void Identify(bool resetCounter = true)
        {
            this.ConnectionState = ConnectionStates.Identifying;

            this.SendOperation(new OperationMessage()
            {
                OpCode = OperationMessage.OPCodes.PingRequest,
            });

            if (resetCounter)
                this.retriesCount = 0;

            identifyTimeoutTimer.Start();
        }
        
        public async Task<bool> SendMessage(OutputHeader message)
        {
            if (this.pendingConfirmationTask != null && !this.pendingConfirmationTask.Task.IsCompleted)
                throw new InvalidOperationException("Sending in progress");

            this.pendingConfirmationTask = new TaskCompletionSource<bool>();

            if (!this.SendData(message.ToBinary()))
                return false;
            else
                return await this.pendingConfirmationTask.Task;
        }

        public bool SendOperation(IMessage operation)
        {
            OutputHeader outputMessage = new OutputHeader()
            {
                SecurityEnabled = true,
                RoutingEnabled = true,
                EndPoint = 1,
                Retries = 3,
                Content = operation
            };

            return this.SendData(outputMessage.ToBinary());
        }
        #endregion

    }
}
