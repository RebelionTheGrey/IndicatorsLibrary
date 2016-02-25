using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using StockSharp.Algo;
using StockSharp.Algo.Indicators;
using StockSharp.Algo.Candles;

using System.IO;
using System.Collections;

using System.Collections.Concurrent;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


using RManaged;
using RManaged.Core;
using BaseTypes;

using RDotNet;
using RDotNet.Internals;

namespace IndicatorsLibrary.RBasedIndicators
{
    public abstract class RLenghtIndicator<T> : LengthIndicator<T>
    {
        protected IREngine Engine { get; set; }

        protected Script MainScript { get; set; }
        protected List<Script> SubScripts { get; set; }

        protected void LoadScripts(string scriptConfigName)
        {
            var config = XElement.Load(scriptConfigName);

            var mainScriptName = config.Descendants("MainScriptName").Select(elem => { return elem.Value; }).ElementAt(0);
            var subScriptNames = config.Descendants("SubScriptNames").ToList();

            MainScript = new Script(mainScriptName, null);
            SubScripts = new List<Script>();

            subScriptNames.ForEach(elem =>
            {
                var funcNames = elem.Descendants("FunctionNames").Select(e => { return e.Value; }).ToList();
                var compileCode = bool.Parse(elem.Descendants("CompileCode").Select(e => { return e.Value; }).ElementAt(0));

                var newSubScript = new Script(elem.Value, funcNames);
                SubScripts.Add(newSubScript);

                if (newSubScript.IsValid)
                {
                    Engine.Evaluate(newSubScript.ScriptBody);

                    if (compileCode)
                        funcNames.ForEach(e => Engine.Evaluate(string.Format("{0} <- cmpfun({0});", e)));
                }
            });
        }

        public RLenghtIndicator(IREngine engine, string scriptConfigName) : base()
        {
            Engine = engine;

            LoadScripts(scriptConfigName);
        }
    }
}
