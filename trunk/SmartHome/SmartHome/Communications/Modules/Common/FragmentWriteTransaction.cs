#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartHome.Comunications.Messages;
using SmartHome.Comunications.Modules;
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

        #region Properties
        public OperationMessage.OPCodes OpCode { get; private set; }

        public ushort DestinationAddress { get; private set; }

        public byte NumberOfFragments { get; private set; }

        public int FrameSize { get; private set; }

        public int FragmentRemain
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
                return (FragmentRemain / NumberOfFragments) * 100f;
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.outputBuffer.Count != NumberOfFragments;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this.outputBuffer.Count == 0;
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

            this.NumberOfFragments = (byte)(content.Length / MAX_CONTENT_SIZE);
            this.FrameSize = 0;
            for (byte i = 0; i <= this.NumberOfFragments; i++)
            {
                OperationMessage currentOp = new OperationMessage()
                {
                    SourceAddress = 0x00,
                    DestinationAddress = destinationAddress,
                    OpCode = OperationMessage.OPCodes.ConfigWrite,
                    Args = new byte[MAX_CONTENT_SIZE + 2]
                };

                this.FrameSize = Math.Min(MAX_CONTENT_SIZE, content.Length - (i * MAX_CONTENT_SIZE));
                currentOp.Args[0] = (byte)((this.NumberOfFragments << 4) | (i & 0xF));
                currentOp.Args[1] = (byte)this.FrameSize;
                Buffer.BlockCopy(content, (i * MAX_CONTENT_SIZE), currentOp.Args, 2, this.FrameSize);

                this.outputBuffer.Add(currentOp);
            }
        }

        /// <summary>
        /// Method that start the FragmentWriteTransaccion
        /// </summary>
        /// <returns>True if the operation was successfully sent</returns>
        public async Task<bool> StartTransaction()
        {
            if (this.IsStarted && !this.IsWaitingForResponse)
                throw new InvalidOperationException("The transaction already been started");

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

            if (this.outputBuffer.Count == 0)
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
                        Debug.WriteLine("TRANSACTION WRITE ERROR CODE: " + Enum.GetName(typeof(WriteSessionStatusCodes), errorCode));
                    else if (Enum.IsDefined(typeof(ConfigWriteStatusCodes), errorCode))
                        Debug.WriteLine("TRANSACTION WRITE APP ERROR CODE: " + Enum.GetName(typeof(ConfigWriteStatusCodes), errorCode));
                    else
                        Debug.WriteLine("TRANSACTION WRITE UNKNOWN ERROR CODE: " + errorCode);

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
