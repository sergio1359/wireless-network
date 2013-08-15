#region Using Statements
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
#endregion

namespace DataLayer.Entities
{
    public class Home
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public Coordenate Location { get; set; }

        public Security Security {get; set;}

        public virtual ICollection<Zone> Zones { get; set; }

        public Home()
        {
            this.Zones = new List<Zone>();
        }
    }

    public class Coordenate
    {
        public float? Longitude { get; set; }

        public float? Latitude { get; set; }
    }

    public class Security
    {
        public const byte CHANNEL = 0x0F;
        public const ushort PANID = 0x1234;

        /// <summary>
        /// Gets or sets the channel. Valid range of values for the channel parameter on 2.4GHz radios is 11 – 26 (0x0b – 0x1a).
        /// </summary>
        /// <value>
        /// The frequency channel.
        /// </value>
        [Range(11, 26)]
        public int Channel { set; get; }

        /// <summary>
        /// Gets or sets the pan id.
        /// </summary>
        /// <value>
        /// The pan id.
        /// </value>
        [Range(0, 65534)]
        public int PanId { set; get; }

        [MaxLength(16), MinLength(16, ErrorMessage = "Security Key must have 16 characters length")]
        public string SecurityKey { set; get; }

        public byte[] GetSecurityKey()
        {
            return Encoding.ASCII.GetBytes(this.SecurityKey);
        }

        public Security()
        {
            this.Channel = CHANNEL;
            this.PanId = PANID;
            this.SecurityKey = "TestSecurityKey0";
        }
    }
}
