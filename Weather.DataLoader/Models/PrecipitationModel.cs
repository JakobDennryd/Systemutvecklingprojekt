using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.DataLoader.Models
{
    internal class PrecipitationModel
    {
        public DateTime CreatedOn { get; set; }
        public decimal AmountCm { get; set; }
        public string WeatherType { get; set; }
        public string ZipCode { get; set; }
    }
}
