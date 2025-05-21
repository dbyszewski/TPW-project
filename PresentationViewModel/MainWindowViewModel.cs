using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using TP.ConcurrentProgramming.Data;
using TP.ConcurrentProgramming.BusinessLogic;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
    }

    #endregion ctor

    #region public API

    public void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      ModelLayer.Start(NumberOfBalls);
      Observer.Dispose();
    }

    public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

    public double TableWidth => BusinessLogicAbstractAPI.TableWidth;
    public double TableHeight => BusinessLogicAbstractAPI.TableHeight;

    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;

    private int _numberOfBalls;

    public int NumberOfBalls
    {
      get => _numberOfBalls;
      set
      {
        if (_numberOfBalls != value)
        {
          _numberOfBalls = value;
          RaisePropertyChanged(nameof(NumberOfBalls));
        }
      }
    }

    public string NumberOfBallsInput
    {
      get => _numberOfBalls.ToString();
      set
      {
        if(int.TryParse(value, out int number) && number > 0)
        {
          NumberOfBalls = number;
        }
        else
        {
          NumberOfBalls = 1;
        }
        RaisePropertyChanged(nameof(NumberOfBallsInput));
      }
    }
    #endregion private
  }
}