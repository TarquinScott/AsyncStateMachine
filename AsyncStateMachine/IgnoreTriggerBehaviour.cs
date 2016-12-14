using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{

  public class IgnoreTriggerBehaviour : TriggerBehaviour
  {
    public IgnoreTriggerBehaviour(string trigger, Func<bool> guard)
      : base(trigger, String.Empty, guard)
    {
      //nop
    }

    public override bool AllowTransition
    {
      get
      {
        return false;
      }
    }
  }
} 
