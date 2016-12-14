using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncStateMachine
{
  public class ContinuationAction
  {
    readonly Action _action;
    public ContinuationAction(ContinuationOption option, Action action)
    {
      Option = option;
      _action = action == null ? () => { } : action;
    }
    
    public ContinuationOption Option { get; private set; }
    
    public Action Action { get { return _action; } }
  }
}

