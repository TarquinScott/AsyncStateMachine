using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
  public enum ContinuationOption
  {
    None = 0,
    Succeed= 1,
    Fail = 2,
    Cancel = 3
  }
}
