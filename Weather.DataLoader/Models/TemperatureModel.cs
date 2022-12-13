using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.DataLoader.Models
{
    internal class TemperatureModel
    {
        public DateTime CreatedOn { get; set; }
        public decimal TempHighC { get; set; }
        public decimal TempLowC { get; set; }
        public string ZipCode { get; set; }
    }
}
