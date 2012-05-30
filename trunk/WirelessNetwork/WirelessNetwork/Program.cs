using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WirelessNetwork
{
#if WINDOWS || XBOX
    static class Program
    {
        public static List<Node> NodeList = new List<Node>();
        public static List<NodeuC> NodeListuC = new List<NodeuC>();
        public static DateTime init { get; set; }

        public static Texture2D circleTexture;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
                game.Run();
        }

        public static NodeuC GetNode(byte address)
        {
            //Search node in GLOBAL NODELIST
            return NodeListuC.First<NodeuC>(x => x.NodeAddress == address);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public static uint MessageBoxCustom(String text, String caption)
        {
            TimeSpan diff = new TimeSpan(DateTime.Now.Ticks - init.Ticks);

            return MessageBox(new IntPtr(0), text + "\n" + diff.ToString(), caption, 0);
        }
    }
#endif
}

