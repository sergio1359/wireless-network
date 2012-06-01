using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;
using System.Runtime.InteropServices;
using System.Threading;

namespace WirelessNetwork
{
    public class MSG
    {
        public byte header;
        public byte from;
        public byte to;
        public byte parent; //neighbor who has sent the message
    }

    public class DATA_MSG : MSG
    {
        public byte header_id { 
            get{ return (byte)(header & 0x3F);} 
            set{ header = (byte)((header & 0xC0) | (value & 0x3F));}
        }

        public byte header_reserved
        {
            get { return (byte)((header >> 6) & 0x01); }
            set { header = (byte)((header & 0xBF) | (value << 6)); }
        }

        public byte header_type
        {
            get { return (byte)((header >> 7) & 0x01); }
            set { header = (byte)((header & 0x7F) | (value << 7)); }
        }

		public string data;

        public byte[] raw_bytes { 
            get{
                byte[] result = new byte[4 + data.Length];
                result[0] = header;
                result[1] = from;
                result[2] = to;
                result[3] = parent;
                for (int i = 0; i < data.Length; i++)
                    result[4 + i] = (byte)data[i];

                return result;
            }
            set
            {
                header = value[0];
                from = value[1];
                to = value[2];
                parent = value[3];
                data = "";
                for (int i = 4; i < value.Length; i++)
                    data += (char)value[i];
            }
        }
    }

    public class ROUTE_MSG : MSG
    {
        public byte header_distance { 
            get{ return (byte)(header & 0x1F);} 
            set{ header = (byte)((header & 0xE0) | (value & 0x1F));}
        }

        public byte header_ok {
            get { return (byte)((header >> 5) & 0x01); }
            set{ header = (byte)((header & 0xDF) | (value << 5));}
        }

        public byte header_restype {
            get{ return (byte)((header >> 6) & 0x01);}
            set{ header = (byte)((header & 0xBF) | (value << 6));}
        }

        public byte header_type {
            get{ return (byte)((header >> 7) & 0x01);}
            set{ header = (byte)((header & 0x7F) | (value << 7));}
        }

		public byte reference; //searched node

        public byte[] raw_bytes
        {
            get { return new byte[] { header, from, to, parent, reference }; }
            set
            {
                header = value[0];
                from = value[1];
                to = value[2];
                parent = value[3];
                reference = value[4];
            }
        }
    }

    public class NodeuC
    {
        byte TIMEOUT = 10; //In seconds
        //const int SLEEP_TIME = 500; //In milliseconds

        #region XNAVars

        private string addressName { get { return "NODE " + NodeAddress; } }

        private Vector2 positionNode { get { return new Vector2(Position.X - (Size / 2), Position.Y - (Size / 2)); } }
        private Vector2 positionRange { get { return new Vector2(Position.X - (Range / 2), Position.Y - (Range / 2)); } }
        private Vector2 positionString { get { return new Vector2(Position.X - (DrawHelper.GameFont.MeasureString(addressName).X / 2), Position.Y - (DrawHelper.GameFont.MeasureString(addressName).Y / 2)); } }

        public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Color Color { get; set; }
        public bool IsSelected { get; set; }
        public bool ShowAddress { get; set; }
        public bool Paused { get; set; }

        #endregion

        public int Range { get; set; }
        public byte NodeAddress { get; set; }

        public List<byte> NeighborsTable;
        public Dictionary<byte, byte[]> RouteTable { get; private set; } //Nodo destino -> { Gateway, Length }
        public Dictionary<byte, byte[]> LookTable { get; private set; } //Nodo buscado(reference) -> {interesado, contador}

        byte counterId = 0;
        List<ROUTE_MSG> routeBuffer = new List<ROUTE_MSG>();
        List<DATA_MSG> dataInputBuffer = new List<DATA_MSG>();
        List<DATA_MSG> dataOutputBuffer = new List<DATA_MSG>();
        Thread mainThread;

        public NodeuC(int size)
        {
            Size = size;
            Range = Size * 3;
            Color = Color.DarkBlue;

            NeighborsTable = new List<byte>();
            RouteTable = new Dictionary<byte, byte[]>();
            LookTable = new Dictionary<byte, byte[]>();

            Paused = true;

            mainThread = new Thread(mainLoop);
            mainThread.Start();
        }

        #region XNAFunctions

        public void End()
        {
            mainThread.Abort();
        }

        public bool ContainsPoint(Vector2 point)
        {
            return (point.X > positionNode.X && point.X < positionNode.X + Size) && (point.Y > positionNode.Y && point.Y < positionNode.Y + Size);
        }

        public bool IsNeigbor(NodeuC node)
        {
            float dx = node.Position.X - Position.X;
            float dy = node.Position.Y - Position.Y;

            return ((dx * dx) + (dy * dy) < Range * Range) && node.NodeAddress != this.NodeAddress;
        }

        public void Update()
        {
            TIMEOUT = (byte)MathHelper.Clamp(3.75f + 0.02f * Program.SleepDelay, 5, 15);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Texture2D circleNode = DrawHelper.CreateCircle(Size / 2, graphicsDevice);
            Texture2D circleRange = DrawHelper.CreateCircle(Range / 2, graphicsDevice);

            spriteBatch.Draw(circleRange, positionRange, Color.Orange);
            spriteBatch.Draw(Program.circleTexture, new Rectangle((int)positionNode.X , (int)positionNode.Y, Size, Size), Color);
            if (ShowAddress)
            {
                spriteBatch.DrawString(DrawHelper.GameFont, addressName, positionString, Color.Red);
            }
        }
        #endregion

        //This method is used to send a message to THIS node
        public void SendMessage(MSG message)
        {
            if (message is DATA_MSG)
            {
                DATA_MSG msgCopy = new DATA_MSG();
                msgCopy.raw_bytes = ((DATA_MSG)message).raw_bytes;

                dataInputBuffer.Add(msgCopy);
            }
            else
            {
                ROUTE_MSG msgCopy = new ROUTE_MSG();
                msgCopy.raw_bytes = ((ROUTE_MSG)message).raw_bytes;

                routeBuffer.Add(msgCopy);
            }
        }

        public void Transmit(byte to, string message)
        {
            DATA_MSG data_msg = new DATA_MSG();
            data_msg.header_type = 1;
            data_msg.header_reserved = 0;
            data_msg.header_id = counterId++;
            data_msg.from = NodeAddress;
            data_msg.to = to;
            data_msg.parent = NodeAddress;
            data_msg.data = message;

            if (NeighborsTable.Contains(to) || RouteTable.ContainsKey(to))
            {
                transmitRF(data_msg);
            }
            else
            {
                LookFor(to, 0, NodeAddress, 0);
                dataOutputBuffer.Add(data_msg);
            }
        }

        private void LookFor(byte reference, byte neighborInterested, byte rootInterested, byte distance) //RootInterested represent the first interesed, who start the interrogation
        {
            //This occur when many neighbors ask for the same node
            if (LookTable.ContainsKey(reference))
                return;

            //This check prevents a node with no neighbors store a search in lookTable
            if (neighborInterested== 0 || NeighborsTable.Count > 1)
            {
                LookTable.Add(reference, new byte[] { neighborInterested, 0, (byte)(distance - 1) });

                //Send RouteAnswer to everyone in NeighborsTable != neighborInterested
                lock (NeighborsTable)
                {
                    foreach (byte neigborAddress in NeighborsTable)
                    {
                        if (neigborAddress != neighborInterested)
                        {
                            NodeuC neigbor = Program.GetNode(neigborAddress);

                            ROUTE_MSG routeAnswer = new ROUTE_MSG();
                            routeAnswer.header_distance = distance;
                            routeAnswer.header_restype = 1;//ANSWER
                            routeAnswer.header_type = 0;//ROUTE
                            routeAnswer.from = rootInterested;
                            routeAnswer.to = neigborAddress;
                            routeAnswer.parent = NodeAddress;
                            routeAnswer.reference = reference;

                            neigbor.SendMessage(routeAnswer);
                        }
                    }
                }
            }
        }

        private void refreshNeigbors()
        {
            //lock must be used, since these variables can be accessed from Game1 methods
            lock (NeighborsTable)
            {
                NeighborsTable.Clear();

                //NeighborsTable.AddRange(Program.NodeListuC.Where(x => IsNeigbor(x)).Select(x => x.NodeAddress));

                foreach (var node in Program.NodeListuC)
                {
                    if (IsNeigbor(node))
                        NeighborsTable.Add(node.NodeAddress);
                }
            }
            
            //remove from routeTable all the gateways that currently aren't neighbors
            lock (RouteTable)
            {
                foreach (var item in RouteTable.ToList())
                {
                    byte address = item.Key;
                    byte gateway = item.Value[0];

                    if (!NeighborsTable.Contains(gateway))//gateway is not neighbor
                        RouteTable.Remove(address);
                }
            }
        }

        private void transmitRF(MSG message)
        {
            if(message is DATA_MSG)
                ((DATA_MSG)message).data += "\n" + NodeAddress + " -> ";

            NodeuC Dest;

            if(NeighborsTable.Contains(message.to))
                Dest = Program.GetNode(message.to); //Send directly
            else
                Dest = Program.GetNode(RouteTable[message.to][0]); //Send by gateway

            Dest.SendMessage(message);
        }

        private void addToRouteTable(byte nodeReference, byte nodeGateway, byte distance)
        {
            if (RouteTable.ContainsKey(nodeReference))
            {
                if (distance < RouteTable[nodeReference][1])
                {
                    RouteTable[nodeReference][0] = nodeGateway;
                    RouteTable[nodeReference][1] = distance;
                }
            }
            else
            {
                RouteTable.Add(nodeReference, new byte[] { nodeGateway, distance });
            }
        }

        /*TODO:
         * Detectar cuando un mensaje ha sido recibido y establecer un número de reintentos
         * Añadir ID a los mensajes (autoincremental e independiente para cada nodo)
         * */
        TimeSpan lastLookTableUpdate = new TimeSpan(DateTime.Now.Ticks);
        private void loop()
        {
            TimeSpan iterationTime = new TimeSpan(DateTime.Now.Ticks);

            refreshNeigbors();

            if (routeBuffer.Count > 0) //check the outstanding route messages
            {
                ROUTE_MSG message = routeBuffer[0];

                if (message.header_restype == 1) //Route Answer
                {
                    bool isNeighbor = NeighborsTable.Contains(message.reference);

                    //Check if the first variable returns true, because in this case the second search is not needed
                    if (isNeighbor || RouteTable.ContainsKey(message.reference))
                    {
                        ROUTE_MSG routeResponse = new ROUTE_MSG();
                        if (isNeighbor)
                            routeResponse.header_distance = 1;
                        else
                            routeResponse.header_distance = (byte)(RouteTable[message.reference][1] + 1);
                        routeResponse.header_restype = 0;//RESPONSE
                        routeResponse.header_type = 0;//ROUTE
                        routeResponse.header_ok = 1;

                        routeResponse.from = NodeAddress;
                        routeResponse.to = message.from;
                        routeResponse.parent = NodeAddress;
                        routeResponse.reference = message.reference;

                        NodeuC Dest = Program.GetNode(message.parent);
                        Dest.SendMessage(routeResponse);

                        //TODO: Notify the node sought to avoid a possible subsequent search in the opposite
                        routeResponse.header_distance = (byte)(message.header_distance + 1);
                        routeResponse.from = NodeAddress;
                        routeResponse.to = message.reference;
                        routeResponse.parent = NodeAddress;
                        routeResponse.reference = message.from;
                        Dest = Program.GetNode(isNeighbor ? message.reference : RouteTable[message.reference][0]);
                        Dest.SendMessage(routeResponse);

                        addToRouteTable(message.from, message.parent, message.header_distance);
                    }
                    else
                    {
                        LookFor(message.reference, message.parent, message.from, (byte)(message.header_distance + 1));
                    }
                    Color = Color.Yellow;
                }
                else //Route Response
                {
                    if (message.header_ok > 0)
                    {
                        addToRouteTable(message.reference, message.parent, message.header_distance);

                        if (LookTable.ContainsKey(message.reference))
                        {
                            byte interested =  LookTable[message.reference][0];
                            if (interested != 0)
                            {
                                ROUTE_MSG routeResponse = new ROUTE_MSG();
                                routeResponse.header_distance = (byte)(message.header_distance + 1);
                                routeResponse.header_restype = 0;//RESPONSE
                                routeResponse.header_type = 0;//ROUTE
                                routeResponse.header_ok = 1;

                                routeResponse.from = message.from;
                                routeResponse.to = message.to;
                                routeResponse.parent = NodeAddress;
                                routeResponse.reference = message.reference;

                                NodeuC Dest = Program.GetNode(interested);
                                Dest.SendMessage(routeResponse);

                                //Store the way back if distance > 0 (message.to is not a neighbor)
                                byte distance = LookTable[message.reference][2];
                                if(distance > 0)
                                    addToRouteTable(message.to, interested, distance);
                            }
                            LookTable.Remove(message.reference);
                            Color = Color.Purple;
                        }
                    }
                    //This case is given when it tried to route a message from a node that is no longer able to reach the recipient.
                    else if (RouteTable.ContainsKey(message.reference) && RouteTable[message.reference][0] == message.parent)
                    {
                        RouteTable.Remove(message.reference);

                        //Notify the neighbors that I can't reach the intended recipient
                        foreach (var neighborAddress in NeighborsTable)
                        {
                            if (message.parent != neighborAddress)
                            {
                                ROUTE_MSG routeResponse = new ROUTE_MSG();
                                routeResponse.header_restype = 0;//RESPONSE
                                routeResponse.header_type = 0;//ROUTE
                                routeResponse.header_ok = 0;//FAIL
                                routeResponse.header_distance = message.header_distance;
                                routeResponse.from = NodeAddress;
                                routeResponse.to = message.to;
                                routeResponse.parent = NodeAddress;
                                routeResponse.reference = message.reference;

                                NodeuC Dest = Program.GetNode(neighborAddress);
                                Dest.SendMessage(routeResponse);
                            }
                        }

                        //TODO: If there are pending messages for this node, try to trace a new route
                        if (message.to == NodeAddress)
                        {
                            Program.MessageBoxCustom("Ha cambiado la topología de la red. Reenviar", "MESSAGE "+message.header_distance+
                                " FROM " + NodeAddress + " TO " + message.reference + "           ");
                        }
                    }
                }

                routeBuffer.Remove(message);
            }

            if (dataInputBuffer.Count > 0)//Data messages to read
            {
                Color = Color.YellowGreen;
                DATA_MSG message = dataInputBuffer[0];
                bool isNeighbor;
                if (message.to == NodeAddress) //it's to me?
                {
                    Color = Color.Magenta;
                    message.data += NodeAddress;
                    Program.MessageBoxCustom(message.data, "MESSAGE " + message.header_id +
                                " FROM " + message.from + " TO " + message.to + " BY " + message.parent + "           ");
                }
                else if ((isNeighbor = NeighborsTable.Contains(message.to)) || RouteTable.ContainsKey(message.to))
                {
                    message.parent = NodeAddress;
                    message.data += NodeAddress + " -> ";
                    NodeuC Dest =  Program.GetNode(isNeighbor ? message.to : RouteTable[message.to][0]);
                    Dest.SendMessage(message);
                }
                else
                {
                    ROUTE_MSG routeResponse = new ROUTE_MSG();
                    routeResponse.header_restype = 0;//RESPONSE
                    routeResponse.header_type = 0;//ROUTE
                    routeResponse.header_ok = 0;//FAIL
                    routeResponse.header_distance = message.header_id;//Use the length field to store the id of the failed message
                    routeResponse.from = NodeAddress;
                    routeResponse.to = message.from;
                    routeResponse.parent = NodeAddress;
                    routeResponse.reference = message.to;

                    NodeuC Dest = Program.GetNode(message.parent);
                    Dest.SendMessage(routeResponse);
                }
                dataInputBuffer.Remove(message);
            }

            if (dataOutputBuffer.Count > 0)//Messages to send
            {
                List<DATA_MSG> items = dataOutputBuffer.ToList();

                foreach (var message in items)
                {
                    if (!LookTable.ContainsKey(message.to))//Look finished
                    {
                        transmitRF(message);
                        dataOutputBuffer.Remove(message);
                    }
                    else if (LookTable[message.to][1] >= TIMEOUT)
                    {
                        Program.MessageBoxCustom("Timeout superado", "MESSAGE FROM " + message.from + " TO " + message.to + "           ");
                        dataOutputBuffer.Remove(message);
                        LookTable.Remove(message.to);
                    }
                }
            }

            int diff = (int)(iterationTime - lastLookTableUpdate).TotalMilliseconds;

            if (diff > 1000 && LookTable.Count > 0) //Check the pending searches every second
            {
                foreach (var item in LookTable.ToList())
                {
                    byte reference = item.Key;
                    byte counter = item.Value[1];

                    if (counter < TIMEOUT)
                        LookTable[reference][1]++;
                    else
                        LookTable.Remove(reference); //TIMEOUT limit exceeded
                }

                lastLookTableUpdate = iterationTime;
            }
        }

        void mainLoop()
        {
            while(true)
            {
                if (!Paused)
                {
                    loop();
                    Thread.Sleep(Program.SleepDelay);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
