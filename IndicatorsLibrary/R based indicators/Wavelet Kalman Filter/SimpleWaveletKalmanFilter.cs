using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IndicatorsLibrary.PlainIndicators;
using RManaged.BaseTypes;
using RManaged.Extensions;
using RManaged.Core;
using StockSharp.Algo.Indicators;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Double;

namespace IndicatorsLibrary.RBasedIndicators
{
    public sealed class SimpleWaveletKalmanFilter : RLenghtIndicator<decimal>
    {
        private int windowSize;
        private int highFreqLevel;
        private int lowFreqLevel;
        private string waveletType;

        private RecursiveKalmanFilter kalmanFilter;        

        private void RecalculateQRMatrix(IEnumerable<double> data)
        {
            var RVector = Engine.CreateNumericVector(data);
            Engine.SetSymbol("timeSeriesDataVector", RVector);

            Engine.Evaluate("varianceDivision <- WaveletMODWTVariance(timeSeriesDataVector, WaveletType, HighFreqLevel, LowFreqLevel");
            var varianceDivision = Engine.GetSymbol("varianceDivision").AsNumeric();

            kalmanFilter.Qmatrix = DenseMatrix.Build.DenseDiagonal(kalmanFilter.XDim, kalmanFilter.XDim, varianceDivision[0]);
            kalmanFilter.Rmatrix = DenseMatrix.Build.DenseDiagonal(kalmanFilter.YDim, kalmanFilter.YDim, varianceDivision[1]);
        }
        public SimpleWaveletKalmanFilter(int xDim, int yDim, float fadingCoeff, IREngine engine, string scriptConfigName = "scriptConfig.xml") : base(engine)
        {
            LoadScripts(scriptConfigName);

            highFreqLevel = int.Parse(this.ScriptParameters.First(parameter => parameter.Name.CompareTo("HighFreqLevel") == 0).Value);
            highFreqLevel = int.Parse(this.ScriptParameters.First(parameter => parameter.Name.CompareTo("LowFreqLevel") == 0).Value);
            windowSize = int.Parse(this.ScriptParameters.First(parameter => parameter.Name.CompareTo("WindowSize") == 0).Value);
            waveletType = this.ScriptParameters.First(parameter => parameter.Name.CompareTo("WaveletType") == 0).Value;

            kalmanFilter = new RecursiveKalmanFilter(xDim, yDim, fadingCoeff, 1.0f, 1.0f);
        }

        public SimpleWaveletKalmanFilter(int xDim, int yDim, float fadingCoeff, int highFreqLevel, int lowFreqLevel, int windowSize,
                                         string waveletType, IREngine engine, string scriptConfigName = "scriptConfig.xml") : base(engine)
        {
            LoadScripts(scriptConfigName);

            this.highFreqLevel = highFreqLevel;
            this.lowFreqLevel = lowFreqLevel;
            this.windowSize = windowSize;
            this.waveletType = waveletType;

            kalmanFilter = new RecursiveKalmanFilter(xDim, yDim, fadingCoeff, 1.0f, 1.0f);
        }

        protected override IIndicatorValue OnProcess(IIndicatorValue input)
        {
            return kalmanFilter.Process(input);
        }
    }
}
