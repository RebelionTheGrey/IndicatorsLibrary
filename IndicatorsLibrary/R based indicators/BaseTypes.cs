using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndicatorsLibrary.RBasedIndicators.BaseTypes
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsSerialized { get; set; }
    }
}
