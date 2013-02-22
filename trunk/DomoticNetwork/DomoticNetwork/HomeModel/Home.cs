using System;
using System.Collections.Generic;

namespace DomoticNetwork.HomeModel
{
    class Home
    {
        public String Name { get; set; }
        public List<Floor> Floors { get; set; }
    }

    class Floor
    {
        public String Name { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        //public Image map  { get; set; }
    }
}
