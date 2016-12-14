using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
  public class StateMachine : INotifyPropertyChanged
  {
    private Dictionary<string, StateRepresentation> _stateRepresentations = new Dictionary<string, StateRepresentation>();
    private string _state;

    public StateMachine(string initialState)
    {
      _state = initialState;
    }

    public string State
    {
      get
      {
        return _state;
      }
      set
      {
        SetProperty(ref _state, value);
      }
    }

    public StateRepresentation CurrentRepresentation
    {
      get
      {
        return GetRepresentation(State);
      }
    }

    public StateRepresentation GetRepresentation(string state)
    {
      StateRepresentation representation;
      if (!_stateRepresentations.TryGetValue(state, out representation))
      {
        representation = new StateRepresentation(state);
        _stateRepresentations.Add(state, representation);
      }
      return representation;
    }

    public StateConfiguration Configure(string state)
    {
      return new StateConfiguration(this, GetRepresentation(state));
    }

    public bool CanFire(string trigger)
    {
      return CurrentRepresentation.CanHandle(trigger);
    }

    public bool IsIgnored(string trigger)
    {
      return CurrentRepresentation.IsIgnored(trigger);
    }

    public void Fire(string trigger)
    {
      string destinationState;
      if (CurrentRepresentation.CanTransition(trigger, out destinationState))
      {
        ContinuationOption option = ContinuationOption.Succeed;
        Exception error = null;

        try
        {
          //exit actions for current state
          CurrentRepresentation.Exit();
          //change state 
          State = destinationState;
          //entry actions for new state
          CurrentRepresentation.Enter();
        }
        catch (Exception ex)
        {
          option = ContinuationOption.Fail;
          error = ex;
        }

        //if a continuation has been configured for option then get it and fire trigger    
        ContinuationBehaviour continuation;
        IEnumerable<ContinuationAction> actions;
        if (CurrentRepresentation.CanContinue(option, out continuation, out actions))
        {
          foreach (var action in actions)
          {
            action.Action();
          }

          Fire(continuation.Trigger);
        }
        else if (error != null)
        {
          throw error;
        }
      }
    }

    public async Task FireAsync(string trigger)
    {
      string destinationState;
      if (CurrentRepresentation.CanTransition(trigger, out destinationState))
      {
        ContinuationOption option = ContinuationOption.Succeed;
        Exception error = null;

        try
        {
          //exit actions for current state
          await CurrentRepresentation.ExitAsync();
          //change state 
          State = destinationState;
          //entry actions for new state
          await CurrentRepresentation.EnterAsync();
        }
        catch (Exception ex)
        {
          option = ContinuationOption.Fail;
          error = ex;
        }

        if (error != null)
        {
          throw error;
        }
        else
        {
          //if a continuation has been configured for option then get it and fire trigger    
          ContinuationBehaviour continuation;
          IEnumerable<ContinuationAction> actions;
          if (CurrentRepresentation.CanContinue(option, out continuation, out actions))
          {
            foreach (var action in actions)
            {
              action.Action();
            }

            await FireAsync(continuation.Trigger);
          }
        }
      }
    }

    #region  PropertyChanged
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
      if (object.Equals(storage, value)) return false;

      storage = value;
      this.OnPropertyChanged(propertyName);

      return true;
    }

    #endregion
  }
}
