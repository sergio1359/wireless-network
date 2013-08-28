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
    class FragmentReadTransaction
    {
        private const int MAX_CONTENT_SIZE = 50;

        private ModuleBase callingModule;

        private List<OperationMessage> inputBuffer;

        private OperationMessage.OPCodes operationResponseOPCode;

        #region Properties
        public OperationMessage.OPCodes StartOpCode { get; private set; }

        public OperationMessage.OPCodes ConfirmationOpCode { get; private set; }

        public ushort DestinationAddress { get; private set; }

        public byte NumberOfFragments { get; private set; }

        public int FrameSize { get; private set; }

        public int FragmentsRemain
        {
            get
            {
                return this.NumberOfFragments - this.inputBuffer.Count;
            }
        }

        public float Percentage
        {
            get
            {
                return (this.NumberOfFragments / this.FragmentsRemain) * 100f;
            }
        }

        public bool IsStarted
        {
            get
            {
                return this.inputBuffer.Count > 0;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return this.IsStarted && this.FragmentsRemain == 0;
            }
        }

        public bool IsAborted { get; private set; }

        // TODO: Implement this property
        public bool IsWaitingForResponse { get; private set; }

        #endregion

        public FragmentReadTransaction(ModuleBase callingModule, OperationMessage.OPCodes startOpcode, OperationMessage.OPCodes confirmationOpcode, Type statusCodeType, ushort destinationAddress)
        {
            this.callingModule = callingModule;
            this.operationResponseOPCode = (OperationMessage.OPCodes)((int)startOpcode + 1);

            this.StartOpCode = startOpcode;
            this.ConfirmationOpCode = confirmationOpcode;
            this.DestinationAddress = destinationAddress;

            this.inputBuffer = new List<OperationMessage>();
        }

        /// <summary>
        /// Method that start the FragmentReadTransaction
        /// </summary>
        /// <returns>True if the operation was successfully sent</returns>
        public async Task<bool> StartTransaction()
        {
            if (this.IsStarted && !this.IsWaitingForResponse)
                throw new InvalidOperationException("The transaction already been started");

            return await callingModule.SendMessage(
                new OperationMessage() { 
                    SourceAddress = 0x00,
                    DestinationAddress = this.DestinationAddress,
                    OpCode = this.StartOpCode,
                    Args = null
                });
        }

        /// <summary>
        /// Method that process a received response. This method will check the response and send the next fragment.
        /// </summary>
        /// <param name="response">The received operation response</param>
        /// <returns>True if the operation was successfully sent</returns>
        public async Task<bool> ProcessResponse(OperationMessage operation)
        {
            if (operation.OpCode != this.operationResponseOPCode)
                throw new InvalidOperationException("The received OperationMessage has an invalid OPCode");

            if (operation.SourceAddress != this.DestinationAddress)
                throw new InvalidOperationException("The received OperationMessage has an invalid SourceAddress");

            if (this.IsCompleted)
                throw new InvalidOperationException("The transaction is completed");

            int fragment = operation.Args[0] & 0x0F;
            OperationMessage lastOp = null;

            if (this.inputBuffer.Count == 0) //First response
            {
                this.NumberOfFragments = (byte)((operation.Args[0] & 0xF0) >> 4);
            }
            else
            {
                lastOp = this.inputBuffer.Last();
            }

            if (lastOp == null || (lastOp != null && operation.Args[0] == lastOp.Args[0] + 1))
            {
                if (operation.Args[0] == 0 && operation.Args[1] == 0) //ERROR
                {
                    Debug.WriteLine(String.Format("CONFIG READ RESPONSE FROM 0x{0:X4} -> BUSY SENDING CONFIG STATE",
                                            operation.SourceAddress));

                    return false;
                }
                else //OK
                {
                    Debug.WriteLine(String.Format("CONFIG READ RESPONSE FROM 0x{0:X4} -> {1}/{2} OK",
                        operation.SourceAddress,
                        fragment,
                        this.NumberOfFragments));

                    this.inputBuffer.Add(operation);

                    return await this.callingModule.SendMessage(
                        new OperationMessage() 
                        { 
                            SourceAddress = 0x00,
                            DestinationAddress = this.DestinationAddress,
                            OpCode = this.ConfirmationOpCode,
                            Args = new byte[] { operation.Args[0], 0x00 } });
                }
            }
            else
            {
                Debug.WriteLine(String.Format("CONFIG READ RESPONSE FROM 0x{0:X4} -> {1}/{2} ERROR",
                    operation.SourceAddress,
                    fragment,
                    this.NumberOfFragments));

                //Send error code back
                await this.callingModule.SendMessage(
                        new OperationMessage()
                        {
                            SourceAddress = 0x00,
                            DestinationAddress = this.DestinationAddress,
                            OpCode = this.ConfirmationOpCode,
                            Args = new byte[] { operation.Args[0], 0x01 }
                        });

                return false;
            }
        }
    }
}
