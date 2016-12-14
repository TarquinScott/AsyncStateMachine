using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{

  public class TriggerBehaviour
  {
    readonly string _trigger;
    readonly string _destinationState;
    readonly Func<bool> _guard;

    public TriggerBehaviour(string trigger, string destinationState, Func<bool> guard)
    {
      _trigger = trigger;
      _destinationState = destinationState;
      _guard = guard;
    }

    public string Trigger { get { return _trigger; } }

    public string DestinationState { get { return _destinationState; } }

    public bool IsGuardConditionMet
    {
      get
      {
        return _guard();
      }
    }

    public virtual bool AllowTransition
    {
      get
      {
        return true;
      }
    }
    
    public override string ToString() => $"Trigger:{this.Trigger} to State:{this.DestinationState}";
  }
}
