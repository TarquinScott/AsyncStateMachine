using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
  public class StateConfiguration
  {
    public StateConfiguration(StateMachine stateMachine, StateRepresentation representation)
    {
      StateMachine = stateMachine;
      Representation = representation;
    }

    public StateMachine StateMachine { get; private set; }

    public StateRepresentation Representation { get; private set; }
    
    public StateConfiguration SuperState(string superState)
    {
      StateRepresentation superStateRepresentation = StateMachine.GetRepresentation(superState);
      //prevent cyclical references
      Assertion.AreNotEqual(Representation.State, superStateRepresentation.SuperState?.State ?? null);

      Representation.SetSuperState(superStateRepresentation);
      return this;
    }
    
    public StateConfiguration Permit(string trigger, string destinationState)
    {
      return PermitIf(trigger, destinationState, () => true);
    }
    
    public StateConfiguration PermitIf(string trigger, string destinationState, Func<bool> guard)
    {
      Representation.AddTriggerBehaviour(new TriggerBehaviour(trigger, destinationState, guard));
      return this;
    }
    
    public StateConfiguration Continue(ContinuationOption option, string trigger)
    {
      return ContinueIf(option, trigger, () => true);
    }
    
    public StateConfiguration ContinueIf(ContinuationOption option, string trigger, Func<bool> guard)
    {
      Representation.AddContinuationBehaviour(new ContinuationBehaviour(option, trigger, guard));
      return this;
    }

    public StateConfiguration ContinueAction(ContinuationOption option, Action action)
    {
      Representation.AddContinuationAction(new ContinuationAction(option, action));
      return this;
    }

    public StateConfiguration Ignore(string trigger)
    {
      return IgnoreIf(trigger, () => true);
    }
    
    public StateConfiguration IgnoreIf(string trigger, Func<bool> guard)
    {
      Representation.AddTriggerBehaviour(new IgnoreTriggerBehaviour(trigger, guard));
      return this;
    }

    public StateConfiguration AddEntryAction(Action action)
    {
      Representation.AddEntryAction(action);
      return this;
    }

    public StateConfiguration AddExitAction(Action action)
    {
      Representation.AddExitAction(action);
      return this;
    }

    public StateConfiguration AddAsyncEntryAction(Func<Task> action)
    {
      Representation.AddAsyncEntryAction(action);
      return this;
    }

    public StateConfiguration AddAsyncExitAction(Func<Task> action)
    {
      Representation.AddAsyncExitAction(action);
      return this;
    }

    public void Continue(object continuatioOption)
    {
      throw new NotImplementedException();
    }
  }
}
