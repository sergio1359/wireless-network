using System.Drawing;

namespace ServiceLayer.DTO
{
    public class ZoneDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ViewDTO MainView { get; set; }

        public ViewDTO[] Views { get; set; }

        public override string ToString()
        {
            return "Id: " + Id + " " +Name;
        }
    }

    public class ViewDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] ImageMap { get; set; }

        public override string ToString()
        {
            return "Id: " + Id + " " + Name;
        }
    }
}
