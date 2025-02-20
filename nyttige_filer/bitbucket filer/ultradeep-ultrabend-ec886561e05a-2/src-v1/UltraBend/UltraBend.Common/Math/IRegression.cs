using System.ComponentModel;

namespace UltraBend.Common.Math
{
    public interface IRegression : INotifyPropertyChanged
    {
        int Order { get; set; }
        double RSquared { get; set; }
        double[] a { get; set; }

        void Compute();
        double Eval(double x);
    }
}