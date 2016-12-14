using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
  public class StateRepresentation
  {
    private List<TriggerBehaviour> _triggers = new List<TriggerBehaviour>();
    private List<ContinuationBehaviour> _continuations = new List<ContinuationBehaviour>();
    private List<ContinuationAction> _continuationActions = new List<ContinuationAction>();
    private List<Action> _entryActions = new List<Action>();
    private List<Action> _exitActions = new List<Action>();
    private List<Func<Task>> _asyncEntryActions = new List<Func<Task>>();
    private List<Func<Task>> _asyncExitActions = new List<Func<Task>>();

    public StateRepresentation(string state)
    {
      State = state;
    }

    public string State { get; private set; }

    public StateRepresentation SuperState { get; private set; }

    public void AddTriggerBehaviour(TriggerBehaviour triggerBehaviour)
    {
      _triggers.Add(triggerBehaviour);
    }

    public void AddContinuationBehaviour(ContinuationBehaviour continuationBehaviour)
    {
      _continuations.Add(continuationBehaviour);
    }

    public void AddContinuationAction(ContinuationAction continuationAction)
    {
      _continuationActions.Add(continuationAction);
    }

    public void AddEntryAction(Action action)
    {
      _entryActions.Add(action);
    }

    public void AddExitAction(Action action)
    {
      _exitActions.Add(action);
    }

    public void AddAsyncEntryAction(Func<Task> action)
    {
      _asyncEntryActions.Add(action);
    }

    public void AddAsyncExitAction(Func<Task> action)
    {
      _asyncExitActions.Add(action);
    }

    public void SetSuperState(StateRepresentation stateRepresentation)
    {
      SuperState = stateRepresentation;
    }

    public bool CanHandle(string trigger)
    {
      return GetTriggerBehaviour(trigger) != null;
    }

    public bool IsIgnored(string trigger)
    {
      TriggerBehaviour triggerBehaviour = GetTriggerBehaviour(trigger);
      return triggerBehaviour != null ? !triggerBehaviour.AllowTransition : false;
    }

    private TriggerBehaviour GetTriggerBehaviour(string trigger)
    {
      var triggers = from t in _triggers
                     where t.Trigger == trigger
                     && t.IsGuardConditionMet
                     select t;

      if (triggers.Count() > 1)
      {
        throw new Exception($"Multiple triggers cannot be configured for state: {State} and trigger: {trigger}");
      }
      
      TriggerBehaviour triggerBehaviour = triggers.FirstOrDefault();
      if(triggerBehaviour == null && SuperState != null)
      {
        triggerBehaviour = SuperState.GetTriggerBehaviour(trigger);
      }
      return triggerBehaviour;
    }

    private ContinuationBehaviour GetContinuationBehaviour(ContinuationOption option)
    {
      var continuations = from c in _continuations
                     where c.Option == option
                     && c.IsGuardConditionMet
                     select c;

      if (continuations.Count() > 1)
      {
        throw new Exception($"Multiple continuations cannot be configured for state: {State} and option: {option}");
      }
      //continuations do not look at super state
      return continuations.FirstOrDefault();
    }

    private IEnumerable<ContinuationAction> GetContinuationActions(ContinuationOption option)
    {
      var actions = from c in _continuationActions
                          where c.Option == option
                          select c;

      return actions;
    }

    public bool CanTransition(string trigger, out string state)
    {
      bool result = false;
      state = null;
      if(!CanHandle(trigger))
      {
        throw new InvalidOperationException($"There is no trigger: {trigger} configured for state: {State}");
      }

      if (!IsIgnored(trigger))
      {
        TriggerBehaviour triggerBehaviour = GetTriggerBehaviour(trigger);
        result = true;
        state = triggerBehaviour.DestinationState;
      }

      return result;
    }

    public bool CanContinue(ContinuationOption option, out ContinuationBehaviour continuation, out IEnumerable<ContinuationAction> actions)
    {
      continuation = GetContinuationBehaviour(option);
      actions = GetContinuationActions(option);
      return continuation != null;
    }

    public void Enter()
    {
      if (_asyncEntryActions.Any()) throw new InvalidOperationException("Cannot enter sync with async entry actions configured");

      foreach (Action action in _entryActions)
      {
        action();
      }
    }
    
    public void Exit()
    {
      if (_asyncExitActions.Any()) throw new InvalidOperationException("Cannot exit sync with async exit actions configured");

      foreach (Action action in _exitActions)
      {
        action();
      }
    }

    public async Task EnterAsync()
    {
      foreach (Action action in _entryActions)
      {
        action();
      }

      foreach (Func<Task> action in _asyncEntryActions)
      {
        await action();
      }
    }

    public async Task ExitAsync()
    {
      foreach (Action action in _exitActions)
      {
        action();
      }

      foreach (Func<Task> action in _asyncExitActions)
      {
        await action();
      }
    }

    public override string ToString()
    {
      return State;
    }
  }
}
