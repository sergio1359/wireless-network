#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Entities.HomeDevices;
using System.Reflection;
using System.ComponentModel.DataAnnotations; 
#endregion

namespace DataLayer.Entities
{
    public class Operation
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual HomeDevice DestionationHomeDevice { get; set; }

        [Required]
        public string OperationName { get; set; }

        public object[] Params { get; set; }

        public virtual ICollection<TimeRestriction> TimeRestrictions { get; set; }

        public virtual ICollection<ConditionalRestriction> ConditionalRestriction { get; set; }

        public Operation()
        {
            this.TimeRestrictions = new List<TimeRestriction>();
            this.ConditionalRestriction = new List<ConditionalRestriction>();
        }

        public void Execute() 
        { 

        }

        public byte[] ToBinaryOperation()
        {
            return this.GetOperationMessage().ToBinary();
        }

        public OperationMessage GetOperationMessage()
        {
            MethodInfo method = this.DestionationHomeDevice.GetType().GetMethods().First(m => m.Name == OperationName && m.ReturnType == typeof(OperationMessage) && m.Attributes == OperationAttribute);

            return (OperationMessage)method.Invoke(this.DestionationHomeDevice, this.Params);
        }
    }
}