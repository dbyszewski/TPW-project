using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        public ModelBall(double top, double left, LogicIBall underneathBall)
        {
            TopBackingField = top;
            LeftBackingField = left;
            Diameter = underneathBall.Diameter;
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        public double Top
        {
            get { return TopBackingField; }
            private set
            {
                TopBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return LeftBackingField; }
            private set
            {
                LeftBackingField = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter { get; init; }

        public event PropertyChangedEventHandler PropertyChanged;

        private double TopBackingField;
        private double LeftBackingField;

        private void NewPositionNotification(object sender, IPosition e)
        {
            Top = e.y;
            Left = e.x;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }

        #endregion testing instrumentation
    }
}