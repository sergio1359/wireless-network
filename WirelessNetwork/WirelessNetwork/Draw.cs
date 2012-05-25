using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WirelessNetwork
{
    public static class DrawHelper
    {
        public static SpriteFont GameFont { get; set; }

        public static Texture2D CreateCircle(int radius, GraphicsDevice graphicsDevice)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(graphicsDevice, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
            {
                // Use the parametric definition of a circle: http://en.wikipedia.org/wiki/Circle#Cartesian_coordinates
                int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                data[y * outerRadius + x + 1] = Color.White;
            }
            /*
            bool finished = false;
            int firstSkip = 0;
            int lastSkip = 0;
            for (int i = 0; i <= data.Length - 1; i++)
            {
                if (finished == false)
                {
                    //T = transparent W = White;
                    //Find the First Batch of Colors TTTTWWWTTTT The top of the circle
                    if ((data[i] == Color.White) && (firstSkip == 0))
                    {
                        while (data[i + 1] == Color.White)
                        {
                            i++;
                        }
                        firstSkip = 1;
                        i++;
                    }
                    //Now Start Filling                       TTTTTTTTWWTTTTTTTT
                    //circle in Between                       TTTTTTW--->WTTTTTT
                    //transaparent blancks                    TTTTTWW--->WWTTTTT
                    //                                        TTTTTTW--->WTTTTTT
                    //                                        TTTTTTTTWWTTTTTTTT
                    if (firstSkip == 1)
                    {
                        if (data[i] == Color.White && data[i + 1] != Color.White)
                        {
                            i++;
                            while (data[i] != Color.White)
                            {
                                //Loop to check if its the last row of pixels
                                //We need to check this because of the 
                                //int outerRadius = radius * 2 + -->'2'<--;
                                for (int j = 1; j <= outerRadius; j++)
                                {
                                    if (data[i + j] != Color.White)
                                    {
                                        lastSkip++;
                                    }
                                }
                                //If its the last line of pixels, end drawing
                                if (lastSkip == outerRadius)
                                {
                                    finished = true;
                                    break;
                                }
                                else
                                {
                                    data[i] = Color.White;
                                    i++;
                                    lastSkip = 0;
                                }
                            }
                            while (data[i] == Color.White)
                            {
                                i++;
                            }
                            i--;
                        }


                    }
                }
            }
            */
            texture.SetData(data);
            return texture;
        }

        public static void DrawCircle(GraphicsDevice device, int vertexCount)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];
            Vector3 radius = new Vector3(4, 0, 0);
            Vector3 axis = new Vector3(53, 53, 0);
            Vector3 position;
            for (int i = 0; i < vertexCount; i++)
            {
                position = Vector3.Transform(radius, Matrix.CreateFromAxisAngle(axis, MathHelper.ToRadians(360f / (vertexCount - 1) * i)));
                vertices[i] = new VertexPositionColor(position, Color.Black);
            }
            VertexBuffer vertexBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionColor>(vertices);

            //device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
            // Set the vertex source
            device.SetVertexBuffer(vertexBuffer);
            BasicEffect effect = new BasicEffect(device);
            effect.CurrentTechnique.Passes[0].Apply();
            //device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
            //effect.World = worldMatrix;

            // Draw the 3-D axis
            //effect.Begin();
            
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, vertexCount - 1);
        }

        public static void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }
    }
}
