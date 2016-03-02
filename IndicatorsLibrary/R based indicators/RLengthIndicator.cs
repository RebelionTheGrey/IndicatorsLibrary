using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using System.Collections;

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


using RManaged.Communications;
using RManaged.Core;
using RManaged.BaseTypes;
using RManaged.Utilities;

using RDotNet;
using RDotNet.Internals;

namespace IndicatorsLibrary.RBasedIndicators
{
    using BaseTypes;

    public abstract class RLenghtIndicator<T> : LengthIndicator<T>
    {
        protected IREngine Engine { get; set; }

        protected Script MainScript { get; set; }
        protected List<Script> SubScripts { get; set; }
        protected List<Parameter> ScriptParameters { get; set; }        


        private void Initialize()
        {
            SubScripts = new List<Script>();
            ScriptParameters = new List<Parameter>();
        }

        protected virtual void InjectParameters(IEnumerable<Parameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                string parameterSetupString;
                parameterSetupString = parameter.IsSerialized ? string.Format("{0} <- unserialize({1});", parameter.Name, parameter.Value) : string.Format("{0} <- {1}", parameter.Name, parameter.Value);

                Engine.Evaluate(parameterSetupString);
            }
        }
        protected virtual void InjectFunctions(IEnumerable<Script> scripts)
        {
            foreach (var script in scripts)
            {
                if (script.IsValid)
                {
                    Engine.Evaluate(script.ScriptBody);

                    script.InternalFunctions.ForEach(function =>
                    {
                        if (function.Item2)
                            Engine.Evaluate(string.Format("{0} <- cmpfun({0});", function.Item1));
                    });
                }
            }
        }

        public virtual void LoadScripts(string scriptConfigName = "scriptConfig.xml")
        {
            var config = XElement.Load(scriptConfigName);

            var mainScriptSource = config.Descendants("MainScriptSource").Select(elem => { return elem.Value; }).ElementAt(0);
            var scriptXMLParams = config.Descendants("Parameters").ToList();

            MainScript = new Script(mainScriptSource, null);


            var subScriptSources = config.Descendants("SubScriptSource").ToList();
            subScriptSources.ForEach(elem =>
            {
                var scriptSource = elem.Descendants("SourceName").Select(source => { return source.Value; }).ElementAt(0);

                var functionNamesNode = elem.Descendants("FunctionNames").ToList();
                var functionDataPairs = functionNamesNode.Select(node =>
                {
                    return (new Tuple<string, bool>(node.Value, bool.Parse(node.Attribute("NeedCompile").Value)));
                });

                var newSubScript = new Script(elem.Value, functionDataPairs);
                SubScripts.Add(newSubScript);
            });

            scriptXMLParams.ForEach(elem =>
            {
                var currentParameter = new Parameter();
                currentParameter.IsSerialized = bool.Parse(elem.Attribute("Serialized").Value);

                currentParameter.Name = elem.Name.LocalName;
                currentParameter.Value = elem.Value;

                ScriptParameters.Add(currentParameter);
            });

            InjectFunctions(SubScripts);
            InjectParameters(ScriptParameters);
        }

        public RLenghtIndicator(IREngine engine) : base()
        {
            Engine = engine;

            Initialize();
        }
    }
}
