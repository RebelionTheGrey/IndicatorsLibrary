using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IndicatorsLibrary.PlainIndicators;
using RManaged;
using RManaged.Core;

namespace IndicatorsLibrary.RBasedIndicators
{
    public sealed class SimpleWaveletKalmanFilter : RLenghtIndicator<decimal>
    {
        private int windowSize;
        private int level;

        private RecursiveKalmanFilter kalmanFilter;

        public SimpleWaveletKalmanFilter(int xDim, int yDim, float fadingCoeff, int level, int windowSize) :base(xDim, yDim, fadingCoeff)
        {

        }
    }
}
