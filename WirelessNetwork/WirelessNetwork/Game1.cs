using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WirelessNetwork
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blank;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Random ran = new Random();
            int[] locations = { 100, 200,
                                200, 200,
                                300, 200,
                                400, 200,
                                500, 200,
                                600, 200
                              };

            int height = Window.ClientBounds.Height - 30;
            int width = Window.ClientBounds.Width - 30;

            for (int i = 0; i < 10; i++)
            {
                Node node = new Node();
                int margin = node.Size / 2;

                if (locations.Length > 2 * i)
                    node.Position = new Vector2(locations[2 * i], locations[(2 * i) + 1]);
                else
                    node.Position = new Vector2(ran.Next(margin, width - margin), ran.Next(margin, height - node.Size));

                node.Address = "NODO" + (i + 1);
                Program.NodeList.Add(node);
            }

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            DrawHelper.GameFont = Content.Load<SpriteFont>("GameFont");
            blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });

            Program.circleTexture = Content.Load<Texture2D>("circle");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        MouseState mouseLast;
        KeyboardState keyboardLast;
        Node selectedNode;
        bool pauseSim = false;
        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            bool PReleased = keyboardLast.GetPressedKeys().Contains(Keys.P) && !keyboard.GetPressedKeys().Contains(Keys.P);

            if (PReleased)
                pauseSim = !pauseSim;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            bool leftPressed = mouse.LeftButton == ButtonState.Pressed;
            bool rightClick = mouse.RightButton == ButtonState.Pressed && mouseLast.RightButton == ButtonState.Released;
            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);
            bool selected = false;

            foreach (var node in Program.NodeList)
            {
                node.ShowAddress = node.ContainsPoint(mousePosition);
                node.Paused = pauseSim;

                if (!selected && leftPressed && (node.ContainsPoint(mousePosition) || node.IsSelected))
                {
                    node.Position = mousePosition;
                    node.IsSelected = true;
                    selected = true;
                }
                else
                {
                    node.IsSelected = false;
                }

                if (rightClick && node.ContainsPoint(mousePosition))
                {
                    if (selectedNode == null) //First click
                    {
                        selectedNode = node;
                        node.Color = Color.Pink;
                    }
                    else //Second click
                    {
                        if (node != selectedNode)
                        {
                            selectedNode.Color = Color.DarkBlue;
                            Program.init = DateTime.Now;
                            selectedNode.Transmit("Hello world", node.Address);
                        }
                        selectedNode = null;
                    }
                }

                node.Update();
            }

            mouseLast = mouse;
            keyboardLast = keyboard;

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);


            // TODO: Add your drawing code here
            string routeTable = "";
            string lookTable = "";

            spriteBatch.Begin();
            foreach (var node in Program.NodeList)
            {
                node.Draw(spriteBatch, GraphicsDevice);
                if (node.ShowAddress)
                {
                    foreach (var item in node.RouteTable)
                    {
                        routeTable += item.Key + "  " + item.Value.Key + "   " + item.Value.Value + "\n";
                    }
                    foreach (var item in node.LookTable)
                    {
                        lookTable += item.Key + "  " + item.Value.Key + "   " + item.Value.Value + "\n";
                    }
                }

                foreach (var node1 in Program.NodeList)
                {
                    if (node.IsNeigbor(node1))
                    {
                        DrawHelper.DrawLine(spriteBatch, blank, 1, Color.Turquoise, node.Position, node1.Position);
                    }
                }
            }

            spriteBatch.DrawString(DrawHelper.GameFont, routeTable, new Vector2(Window.ClientBounds.Width - 200, 20), Color.Red);
            spriteBatch.DrawString(DrawHelper.GameFont, lookTable, new Vector2(Window.ClientBounds.Width - 400, 20), Color.Red);

            if (pauseSim)
            {
                Vector2 pos = new Vector2((Window.ClientBounds.Width - DrawHelper.GameFont.MeasureString("PAUSE").X) / 2,
                                          (Window.ClientBounds.Height - DrawHelper.GameFont.MeasureString("PAUSE").Y) / 2);
                spriteBatch.DrawString(DrawHelper.GameFont, "PAUSE", pos, Color.Red);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
