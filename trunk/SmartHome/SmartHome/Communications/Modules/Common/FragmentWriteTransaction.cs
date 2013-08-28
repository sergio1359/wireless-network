#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Communications.Messages;
using SmartHome.Communications.Modules;
#endregion

namespace SmartHome.Communications.Modules.Common
{
    public class FragmentWriteTransaction
    {
        private const int MAX_CONTENT_SIZE = 50;

        private ModuleBase callingModule;

        private List<OperationMessage> outputBuffer;

        private OperationMessage.OPCodes operationResponseOPCode;

        private Type applicationStatusCodeType;

        private int totalFragments;

        #region Properties
        public OperationMessage.OPCodes OpCode { get; private set; }

        public ushort DestinationAddress { get; private set; }

        public int CurrentFrameSize { get; private set; }

        public int NumberOfFragments { 
            get 
            { 
                return this.totalFragments + 1;
            }
        }

        public int RemainingFragments
        {
            get
            {
                return this.outputBuffer.Count;
            }
        }

        public float Percentage
        {
            get
            {
                return ((float)(this.NumberOfFragments - this.RemainingFragments) / (float)this.NumberOfFragments) * 100f;
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.outputBuffer.Count != (this.NumberOfFragments);
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this.RemainingFragments == 0;
            }
        }

        public bool IsAborted { get; private set; }

        // TODO: Implement this property
        public bool IsWaitingForResponse { get; private set; }

        #endregion

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="callingModule">Reference to the calling module</param>
        /// <param name="opcode">OPCode used for the outgoing messages</param>
        /// <param name="statusCodeType">Type of the enum for status code parsing on error</param>
        /// <param name="destinationAddress">Destination node address</param>
        /// <param name="content">Content to send in bytes</param>
        public FragmentWriteTransaction(ModuleBase callingModule, OperationMessage.OPCodes opcode, Type statusCodeType, ushort destinationAddress, byte[] content)
        {
            this.callingModule = callingModule;
            this.operationResponseOPCode = (OperationMessage.OPCodes)((int)opcode + 1);
            this.applicationStatusCodeType = statusCodeType;

            this.OpCode = opcode;
            this.DestinationAddress = destinationAddress;

            this.outputBuffer = new List<OperationMessage>();

            this.totalFragments = (byte)(content.Length / MAX_CONTENT_SIZE);

            for (byte i = 0; i <= this.totalFragments; i++)
            {
                this.CurrentFrameSize = Math.Min(MAX_CONTENT_SIZE, content.Length - (i * MAX_CONTENT_SIZE));

                OperationMessage currentOp = new OperationMessage()
                {
                    SourceAddress = 0x00,
                    DestinationAddress = destinationAddress,
                    OpCode = OperationMessage.OPCodes.ConfigWrite,
                    Args = new byte[this.CurrentFrameSize + 2]
                };

                currentOp.Args[0] = (byte)((this.totalFragments << 4) | (i & 0xF));
                currentOp.Args[1] = (byte)this.CurrentFrameSize;
                Buffer.BlockCopy(content, (i * MAX_CONTENT_SIZE), currentOp.Args, 2, this.CurrentFrameSize);

                this.outputBuffer.Add(currentOp);
            }
        }

        /// <summary>
        /// Method that start the FragmentWriteTransaction
        /// </summary>
        /// <returns>True if the operation was successfully sent</returns>
        public async Task<bool> StartTransaction()
        {
            if (this.IsStarted && !this.IsWaitingForResponse)
                throw new InvalidOperationException("The transaction already been started");

            Debug.WriteLine(String.Format("TRANSACTION WRITE TO 0x{0:X4} STARTED", this.DestinationAddress));

            return await callingModule.SendMessage(this.outputBuffer[0]);
        }

        /// <summary>
        /// Method that process a received response. This method will check the response and send the next fragment.
        /// </summary>
        /// <param name="response">The received operation response</param>
        /// <returns>True if the operation was successfully sent</returns>
        public async Task<bool> ProcessResponse(OperationMessage response)
        {
            if (response.OpCode != this.operationResponseOPCode)
                throw new InvalidOperationException("The received OperationMessage has an invalid OPCode");

            if (response.SourceAddress != this.DestinationAddress)
                throw new InvalidOperationException("The received OperationMessage has an invalid SourceAddress");

            if (this.IsCompleted)
                throw new InvalidOperationException("The transaction is completed");


            if (response.Args[0] == this.outputBuffer[0].Args[0])
            {
                if (response.Args[1] == (byte)WriteSessionStatusCodes.OK)
                {
                    this.outputBuffer.RemoveAt(0);
                    if (this.outputBuffer.Count > 0)
                    {
                        return await callingModule.SendMessage(this.outputBuffer[0]);
                    }
                    else
                    {
                        Debug.WriteLine(String.Format("TRANSACTION WRITE DONE TO 0x{0:X4}", response.SourceAddress));

                        return true;
                    }
                }
                else
                {
                    object errorCode = (object)response.Args[1];
                    if (Enum.IsDefined(typeof(WriteSessionStatusCodes), errorCode))
                        Debug.WriteLine(String.Format("TRANSACTION WRITE TO 0x{0:X4} ERROR CODE: {1}", this.DestinationAddress, Enum.GetName(typeof(WriteSessionStatusCodes), errorCode)));
                    else if (Enum.IsDefined(typeof(ConfigWriteStatusCodes), errorCode))
                        Debug.WriteLine(String.Format("TRANSACTION WRITE TO 0x{0:X4} APP ERROR CODE: {1}", this.DestinationAddress, Enum.GetName(typeof(ConfigWriteStatusCodes), errorCode)));
                    else
                        Debug.WriteLine(String.Format("TRANSACTION WRITE TO 0x{0:X4} UNKNOWN ERROR CODE: {1}", this.DestinationAddress, errorCode));

                    this.IsAborted = true;
                    return false;
                }
            }
            else
            {
                int rcvFragment = response.Args[0] & 0x0F;
                int rcvTotalFragment = (response.Args[0] & 0xF0) >> 4;
                int expFragment = this.outputBuffer[0].Args[0] & 0x0F;
                int expTotalFragment = (this.outputBuffer[0].Args[0] & 0xF0) >> 4;

                Debug.WriteLine(String.Format("TRANSACTION WRITE INVALID PROTOCOL: RECEIVED({0}/{1}) EXPECTED({2}/{3})",
                    rcvFragment, rcvTotalFragment, expFragment, expTotalFragment));

                this.IsAborted = true;
                return false;
            }
        }
    }
}
