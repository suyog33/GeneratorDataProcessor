using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GeneratorDataProcessor.Core.Entities
{
    public class DayGeneration
    {
        public DateTime Date { get; set; }
        public double Energy { get; set; }
        public double Price { get; set; }
    }
}
