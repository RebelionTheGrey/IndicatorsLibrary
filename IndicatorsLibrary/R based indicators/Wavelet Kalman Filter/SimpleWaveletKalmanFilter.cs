using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IndicatorsLibrary.PlainIndicators;
using RManaged.BaseTypes;
using RManaged.Core;


namespace IndicatorsLibrary.RBasedIndicators
{
    public sealed class SimpleWaveletKalmanFilter : RLenghtIndicator<decimal>
    {
        private int windowSize;
        private int highFreqLevel;
        private int lowFreqLevel;

        private RecursiveKalmanFilter kalmanFilter;

        private void 

        public SimpleWaveletKalmanFilter(int xDim, int yDim, float fadingCoeff, int highFreqLevel, int lowFreqLevel, int windowSize, 
                                         IREngine engine, string scriptConfigName) :base(engine, scriptConfigName)
        {
            kalmanFilter = new RecursiveKalmanFilter(xDim, yDim, fadingCoeff, 1.0f, 1.0f);
        }
    }
}
