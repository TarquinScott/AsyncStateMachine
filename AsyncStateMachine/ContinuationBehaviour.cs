using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncStateMachine
{
  public class ContinuationBehaviour
  {
    readonly Func<bool> _guard = () => true;

    public ContinuationBehaviour(string trigger, ContinuationOption option)
      : this(option, trigger, () => true)
    {
      //nop
    }

    public ContinuationBehaviour(ContinuationOption option, string trigger, Func<bool> guard)
    {
      Option = option;
      Trigger = trigger;
      _guard = guard;
    }

    public string Trigger { get; private set; }

    public ContinuationOption Option { get; private set; }

    public bool IsGuardConditionMet
    {
      get
      {
        return _guard == null ? true : _guard();
      }
    }
  } 
}

