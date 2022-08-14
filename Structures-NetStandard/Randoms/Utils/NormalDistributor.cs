using System;


namespace Structures.NetStandard.Randoms.Utils {
    public class NormalDistributor {
        public double Mean { get; }
        public double Stdev { get; }
        
        private double _y2;
        private bool _useLast = false;
        private readonly Random _randomizer = new Random();

        public NormalDistributor(double mean, double stdev) {
            Mean = mean;
            Stdev = stdev;
        }

        public double Next() {
            double y1;
            if (_useLast) {
                y1 = _y2;
                _useLast = false;
            } else {
                double x1, x2, w;
                do {
                    x1 = 2 * _randomizer.NextDouble() - 1;
                    x2 = 2 * _randomizer.NextDouble() - 1;
                    w = x1 * x1 + x2 * x2;
                } while (w > 1.0f);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                _y2 = x2 * w;
                _useLast = true;
            }
            
            var retValue = Mean + Stdev * y1;
            return retValue > 0 ? retValue : -retValue;
        }
    }
}