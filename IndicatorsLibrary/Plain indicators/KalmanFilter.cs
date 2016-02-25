using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StockSharp.Algo;
using StockSharp.Algo.Candles;
using StockSharp.BusinessEntities;

using StockSharp.Algo.Indicators;


using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Single;
using MathNet.Numerics.LinearAlgebra;

using MathNet.Numerics.Distributions;

namespace IndicatorsLibrary.PlainIndicators
{
    public class RecursiveKalmanFilter: LengthIndicator<decimal>
    {
        private Matrix<float> Pplus, Pminus;
        private Matrix<float> Q, R, F, H;
        private Vector<float> xplus, xminus;

        public Matrix<float> Qmatrix { get { return Q.Clone(); } set { Qmatrix.CopyTo(Q); } }
        public Matrix<float> Rmatrix { get { return R.Clone(); } set { Rmatrix.CopyTo(R); } }
        public Vector<float> AprioriStateEstimation { get { return xminus; } }
        public Vector<float> AposterioriStateEstimate { get { return xplus; } }

        private int xDim, yDim;
        public float FadingCoeff { get; set; }



        public RecursiveKalmanFilter(int xDim, int yDim, float fadingCoeff)
        {
            this.xDim = xDim;
            this.yDim = yDim;

            FadingCoeff = fadingCoeff;

            Pplus = DenseMatrix.CreateRandom(xDim, xDim, new ContinuousUniform(float.MinValue, float.MaxValue));
            Pminus = DenseMatrix.CreateRandom(xDim, xDim, new ContinuousUniform(float.MinValue, float.MaxValue));

            Q = DenseMatrix.Create(xDim, xDim, delegate (int i, int j) { return 10.0f; });
            R = DenseMatrix.Create(yDim, yDim, delegate (int i, int j) { return 10.0f; });
            F = DenseMatrix.Create(xDim, xDim, delegate (int i, int j) { return 1.0f; });
            H = DenseMatrix.Create(yDim, xDim, delegate (int i, int j) { return 1.0f; });

            xplus = DenseVector.Create(xDim, delegate (int i) { return 0; });
            xminus = DenseVector.Create(xDim, delegate (int i) { return 0; });

            IsFormed = false;
        }

        private void RecalculateFilterParameters(Vector<float> y)
        {
            Pminus = (float)Math.Pow(FadingCoeff, 2) * F * Pplus * F.Transpose() + Q;

            var tempVal = (H * Pminus * H.Transpose() + R).Inverse();
            var K = Pminus * H.Transpose() * tempVal;

            xminus = F * xplus;

            xplus = xminus + K * (y - H * xminus);

            Pplus = Pminus - K * H * Pminus;
        }

        protected override IIndicatorValue OnProcess(IIndicatorValue input)
        {
            var candle = input.GetValue<Candle>();
            var medianPrice = (candle.ClosePrice + candle.OpenPrice + candle.HighPrice + candle.LowPrice) / 4m;

            Buffer.Add(medianPrice);

            Vector<float> val = DenseVector.Create(1, (i) => 0);
            val[0] = (float)medianPrice;

            RecalculateFilterParameters(val);

            if (Buffer.Count < 5)
                return new DecimalIndicatorValue(this, medianPrice);

            IsFormed = true;

            return new DecimalIndicatorValue(this, (decimal)xplus[0]);
        }
    }


}

