using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;

namespace WirelessNetwork
{
    public class RFMessage
    {
        public string From { get; set; }
        public string To { get; set; }
        public byte Header { get; set; }
        public string Data { get; set; }
        public string Root { get; set; }
    }

    public class Node
    {
        const int TIMEOUT = 5000;

        private Vector2 positionNode { get { return new Vector2(Position.X - (Size / 2), Position.Y - (Size / 2)); } }
        private Vector2 positionRange { get { return new Vector2(Position.X - (Range / 2), Position.Y - (Range / 2)); } }
        private Vector2 positionString { get { return new Vector2(Position.X - (DrawHelper.GameFont.MeasureString(Address).X / 2), Position.Y - (DrawHelper.GameFont.MeasureString(Address).Y / 2)); } }

        public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Color Color { get; set; }
        public bool IsSelected { get; set; }
        public bool ShowAddress { get; set; }
        public bool Paused { get; set; }

        public int Range { get; set; }
        public string Address { get; set; }

        List<string> NeighborsTable;
        public Dictionary<string, KeyValuePair<string, int>> RouteTable { get; private set; } //Nodo destino -> Par <Gateway, Length>
        public Dictionary<string, KeyValuePair<string, int>> LookTable { get; private set; } //Nodo buscado -> Par <interesado, cuentaAtrás>

        List<RFMessage> inputBuffer = new List<RFMessage>();
        Dictionary<RFMessage, bool> outputBuffer = new Dictionary<RFMessage, bool>();
        Timer mainLoopTimer = new Timer();

        #region XNAItems
        public Node()
        {
            Size = 40;
            Range = Size * 3;
            Color = Color.DarkBlue;
            NeighborsTable = new List<string>();
            RouteTable = new Dictionary<string, KeyValuePair<string, int>>();
            LookTable = new Dictionary<string, KeyValuePair<string, int>>();

            mainLoopTimer.Interval = 100;
            mainLoopTimer.AutoReset = true;
            mainLoopTimer.Elapsed += new ElapsedEventHandler(mainLoopTimer_Elapsed);
            mainLoopTimer.Start();
        }

        public bool ContainsPoint(Vector2 point)
        {
            return (point.X > positionNode.X && point.X < positionNode.X + Size) && (point.Y > positionNode.Y && point.Y < positionNode.Y + Size);
        }

        public bool IsNeigbor(Node node)
        {
            float dx = node.Position.X - Position.X;
            float dy = node.Position.Y - Position.Y;

            return ((dx * dx) + (dy * dy) < Range * Range) && node.Address != this.Address;
        }

        public void Update()
        {
            mainLoopTimer.Enabled = !Paused;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Texture2D circleNode = DrawHelper.CreateCircle(Size / 2, graphicsDevice);
            Texture2D circleRange = DrawHelper.CreateCircle(Range / 2, graphicsDevice);

            spriteBatch.Draw(circleRange, positionRange, Color.Orange);
            spriteBatch.Draw(Program.circleTexture, new Rectangle((int)positionNode.X , (int)positionNode.Y, Size, Size), Color);
            if (ShowAddress)
            {
                spriteBatch.DrawString(DrawHelper.GameFont, Address, positionString, Color.Red);
            }
        }
        #endregion

        public void SendMessage(RFMessage message)
        {
            inputBuffer.Add(message);
        }

        private void LookFor(string search, string sender)
        {
            if (LookTable.ContainsKey(search))
                return;

            LookTable.Add(search, new KeyValuePair<string, int>(sender, TIMEOUT));

            lock (NeighborsTable)
            {
                foreach (string neigborAddress in NeighborsTable)
                {
                    if (neigborAddress != sender)
                    {
                        Node neigbor = Program.NodeList.First<Node>(x => x.Address == neigborAddress);

                        RFMessage routeAnswer = new RFMessage();
                        routeAnswer.From = Address;
                        routeAnswer.To = neigbor.Address;
                        routeAnswer.Header = 0x40; //01000000 ROUTE ANSWER
                        routeAnswer.Data = search;
                        neigbor.SendMessage(routeAnswer);
                    }
                }
            }
            mainLoopTimer.Enabled = true;
        }

        public void RefreshNeigbors()
        {
            lock (RouteTable)
            {
                lock (NeighborsTable)
                {
                    NeighborsTable.Clear();

                    foreach (var node in Program.NodeList)
                    {
                        if (IsNeigbor(node))
                        {
                            NeighborsTable.Add(node.Address);
                            if (RouteTable.ContainsKey(node.Address))
                                RouteTable.Remove(node.Address);

                            RouteTable.Add(node.Address, new KeyValuePair<string, int>(node.Address, 0)); //Longitud = 0 -> Camino directo
                        }
                    }
                }

                List<string> removeList = new List<string>();
                foreach (var item in RouteTable)
                {
                    if (!NeighborsTable.Contains(item.Value.Key))//is not a neigbor
                        removeList.Add(item.Key);
                }
                foreach (string address in removeList)
                    RouteTable.Remove(address);
            }
        }

        public void Transmit(string message, string to)
        {
            RFMessage rfMsg = new RFMessage();
            rfMsg.Root = Address;
            rfMsg.From = Address;
            rfMsg.To = to;
            rfMsg.Header = 0x80;//DATA HEADER
            rfMsg.Data = message;


            if (RouteTable.ContainsKey(to))
            {
                transmitRF(rfMsg);
            }
            else
            {
                LookFor(to, null);
                outputBuffer.Add(rfMsg, false);
            }
        }

        private void transmitRF(RFMessage message)
        {
            message.Data += "\n" + Address + " -> ";
            Node Dest = Program.NodeList.First<Node>(x => x.Address == RouteTable[message.To].Key);
            Dest.SendMessage(message);
        }

        /*TODO:
         * Detectar cuando un mensaje ha sido recibido y establecer un número de reintentos
         * Añadir ID a los mensajes (autoincremental e independiente para cada nodo)
         * */
        TimeSpan lastIterationTime = new TimeSpan(DateTime.Now.Ticks);
        void mainLoopTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                mainLoopTimer.Enabled = false;
                RefreshNeigbors();
                TimeSpan iterationTime = new TimeSpan(DateTime.Now.Ticks);
                int diff = (int)(iterationTime - lastIterationTime).TotalMilliseconds;
                lastIterationTime = iterationTime;

                if (LookTable.Count > 0) //Check pending searches
                {
                    List<string> items = LookTable.Keys.ToList();
                    for (int i = 0; i < items.Count; i++)
                    {
                        string search = items[i];
                        var aux = LookTable[search];
                        if (aux.Value > 0)
                        {
                            int newCount = aux.Value - diff;

                            LookTable[search] = new KeyValuePair<string, int>(aux.Key, newCount);
                        }
                        else
                        {
                            LookTable.Remove(search);
                        }
                    }
                }

                if (outputBuffer.Count > 0)//Messages to send
                {
                    List<RFMessage> items = outputBuffer.Keys.ToList();
                    //List<RFMessage> removeList = new List<RFMessage>();

                    foreach (var outmsg in items)
                    {
                        if (!LookTable.ContainsKey(outmsg.To))//Look finished
                        {
                            transmitRF(outmsg);
                            //removeList.Add(outmsg);
                            outputBuffer.Remove(outmsg);
                        }
                        else if (LookTable[outmsg.To].Value <= 0)
                        {
                            Program.MessageBoxCustom("Timeout superado", "MESSAGE FROM " + outmsg.Root + " TO " + outmsg.To + "           ");
                            //removeList.Add(outmsg);
                            outputBuffer.Remove(outmsg);
                            LookTable.Remove(outmsg.To);
                        }
                    }
                    //outputBuffer.RemoveAll(x => removeList.Contains(x));
                }

                if (inputBuffer.Count > 0)//Messages to read
                {
                    RFMessage message = inputBuffer[0];
                    byte maskHeader = (byte)(message.Header & 0xC0);

                    if (maskHeader == 0x40)//Is RouteAnswer?
                    {
                        if (RouteTable.ContainsKey(message.Data))
                        {
                            RFMessage routeResponse = new RFMessage();
                            routeResponse.To = message.From;
                            routeResponse.From = Address;
                            routeResponse.Header = (byte)(0x20 | RouteTable[message.Data].Value); //RouteResponse OK y longitud almacenada en la routeTable
                            routeResponse.Data = message.Data; //Search Node

                            Node Dest = Program.NodeList.First<Node>(x => x.Address == message.From);
                            Dest.SendMessage(routeResponse);
                        }
                        else
                        {
                            LookFor(message.Data, message.From);
                        }
                    }
                    else if (maskHeader == 0x00)//Is RouteResponse?
                    {
                        bool isOK = (message.Header & 0xE0) == 0x20; //is OK?
                        if (isOK)
                        {
                            byte distance = (byte)(message.Header & 0x1F);
                            if (distance == 0x1F)
                                throw new IndexOutOfRangeException(); //Distance Overflow

                            if (RouteTable.ContainsKey(message.Data))
                            {
                                if ((distance + 1) < RouteTable[message.Data].Value) //Short way
                                {
                                    RouteTable[message.Data] = new KeyValuePair<string, int>(message.From, distance + 1);
                                }
                            }
                            else
                            {
                                RouteTable.Add(message.Data, new KeyValuePair<string, int>(message.From, distance + 1));
                            }
                        }
                        else if (RouteTable.ContainsKey(message.Data) && RouteTable[message.Data].Key == message.From)
                        {
                            RouteTable.Remove(message.Data);
                            //Avisar a mis vecinos de que ya no puedo llegar al destinatario del mensaje
                            foreach (var nodeAddress in NeighborsTable)
                            {
                                if (message.From != nodeAddress)
                                {
                                    RFMessage routeResponse = new RFMessage();
                                    routeResponse.To = nodeAddress;
                                    routeResponse.From = Address;
                                    routeResponse.Data = message.Data; //Search Node
                                    routeResponse.Header = 0x00; //RouteResponse FAIL
                                    routeResponse.Root = message.Root;//TRAMPA

                                    Node Dest = Program.NodeList.First<Node>(x => x.Address == routeResponse.To);
                                    Dest.SendMessage(routeResponse);
                                }
                            }

                            //TODO: Si existen mensajes pendientes para este nodo, se reintenta para trazar una nueva ruta
                            if (message.Root == Address)//outputBuffer.Exists(x => x.To == message.Data))
                            {
                                //RFMessage retry = outputBuffer.First(x => x.To == message.Data);
                                //outputBuffer.Remove(retry);
                                //transmitRF(retry);
                                Program.MessageBoxCustom("Ha cambiado la topologia de la red. Reenviar", "MESSAGE FROM " + Address + " TO " + message.Data + "           ");
                            }
                        }

                        if (LookTable.ContainsKey(message.Data) && LookTable[message.Data].Key != null)
                        {
                            RFMessage routeResponse = new RFMessage();
                            routeResponse.To = LookTable[message.Data].Key;
                            routeResponse.From = Address;
                            routeResponse.Data = message.Data; //Search Node

                            if (isOK)
                                routeResponse.Header = (byte)(0x20 | (byte)(message.Header & 0x1F) + 1); //RouteResponse OK y longitud recibida + 1
                            else
                                routeResponse.Header = 0x00; //RouteResponse FAIL

                            Node Dest = Program.NodeList.First<Node>(x => x.Address == routeResponse.To);
                            Dest.SendMessage(routeResponse);
                        }

                        LookTable.Remove(message.Data);
                    }
                    else //Is a message
                    {
                        if (message.To == Address) //it's to me?
                        {
                            message.Data += Address;
                            Program.MessageBoxCustom(message.Data, "MESSAGE FROM " + message.Root + " TO " + message.To + "           ");
                        }
                        else if (RouteTable.ContainsKey(message.To))
                        {
                            message.From = Address;
                            message.Data += Address + " -> ";
                            Node Dest = Program.NodeList.First<Node>(x => x.Address == RouteTable[message.To].Key);
                            Dest.SendMessage(message);
                        }
                        else
                        {
                            RFMessage routeResponse = new RFMessage();
                            routeResponse.To = message.From;
                            routeResponse.From = Address;
                            routeResponse.Data = message.To; //Search Node
                            routeResponse.Header = 0x00; //RouteResponse FAIL
                            routeResponse.Root = message.Root;//TRAMPA

                            Node Dest = Program.NodeList.First<Node>(x => x.Address == routeResponse.To);
                            Dest.SendMessage(routeResponse);
                        }

                    }

                    inputBuffer.Remove(message);
                }
            }
        }
    }
}
