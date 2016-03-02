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
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra;

using MathNet.Numerics.Distributions;

namespace IndicatorsLibrary.PlainIndicators
{
    public class RecursiveKalmanFilter: LengthIndicator<decimal>
    {
        private Matrix<double> Pplus, Pminus;
        private Matrix<double> Q, R, F, H;
        private Vector<double> xplus, xminus;

        public Matrix<double> Qmatrix { get { return Q.Clone(); } set { Qmatrix.CopyTo(Q); } }
        public Matrix<double> Rmatrix { get { return R.Clone(); } set { Rmatrix.CopyTo(R); } }
        public Vector<double> AprioriStateEstimation { get { return xminus; } }
        public Vector<double> AposterioriStateEstimate { get { return xplus; } }

        public int XDim { get; private set; }
        public int YDim { get; private set; }
        public double FadingCoeff { get; set; }

        public RecursiveKalmanFilter(int xDim, int yDim, double fadingCoeff, double RdiagValue, double QdiagValue)
        {
            this.XDim = xDim;
            this.YDim = yDim;

            FadingCoeff = fadingCoeff;

            Pplus = DenseMatrix.CreateRandom(xDim, xDim, new ContinuousUniform(double.MinValue, double.MaxValue));
            Pminus = DenseMatrix.CreateRandom(xDim, xDim, new ContinuousUniform(double.MinValue, double.MaxValue));

            Q = DenseMatrix.Create(xDim, xDim, delegate (int i, int j) { return RdiagValue; });
            R = DenseMatrix.Create(yDim, yDim, delegate (int i, int j) { return QdiagValue; });
            F = DenseMatrix.Create(xDim, xDim, delegate (int i, int j) { return 1.0f; });
            H = DenseMatrix.Create(yDim, xDim, delegate (int i, int j) { return 1.0f; });

            xplus = DenseVector.Create(xDim, delegate (int i) { return 0; });
            xminus = DenseVector.Create(xDim, delegate (int i) { return 0; });

            IsFormed = false;
        }

        private void RecalculateFilterParameters(Vector<double> y)
        {
            Pminus = (double)Math.Pow(FadingCoeff, 2) * F * Pplus * F.Transpose() + Q;

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

            Vector<double> val = DenseVector.Create(1, (i) => 0);
            val[0] = (double)medianPrice;

            RecalculateFilterParameters(val);

            if (Buffer.Count < 5)
                return new DecimalIndicatorValue(this, medianPrice);

            IsFormed = true;

            return new DecimalIndicatorValue(this, (decimal)xplus[0]);
        }
    }


}

