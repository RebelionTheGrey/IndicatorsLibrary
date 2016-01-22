using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace IndicatorsLibrary.RBasedIndicators
{
    public class Script
    {
        public string ScriptBody { get; protected set; }
        public bool IsValid { get; protected set; }
        public List<string> FunctionNames { get; protected set; }

        public Script(string scriptFullName, IEnumerable<string> FunctionNames)
        {
            using (TextReader reader = File.OpenText(scriptFullName))
            {
                string ScriptBody = reader.ReadToEnd();
                IsValid = true;

                this.FunctionNames = new List<string>();

                if (FunctionNames != null)
                    this.FunctionNames.AddRange(FunctionNames);
            }
        }
    }
}
