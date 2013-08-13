using System.Drawing;

namespace ServiceLayer.DTO
{
    public class ZoneDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ViewDTO MainView { get; set; }

        public ViewDTO[] Views { get; set; }
    }

    public class ViewDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Image ImageMap { get; set; }
    }
}
