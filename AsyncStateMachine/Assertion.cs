using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AsyncStateMachine
{
  public static class Assertion
  {
    public static void IsNotNull(object value, 
      string message = "Item is null", 
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if(value == null)
      {
        throw new ArgumentNullException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsNull(object value,
      string message = "Item is not null",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (value != null)
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void AreEqual(object expected, 
      object actual, 
      string message = "Items are not equal",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if ((expected != null && actual != null) && !actual.Equals(expected))
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void AreNotEqual(object expected,
      object actual,
      string message = "Items are equal",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if ((expected != null && actual != null) && actual.Equals(expected))
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsTrue(bool predicate, 
      string message = "Condition is not true",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (!predicate)
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsTrue(Func<bool> predicate,
      string message = "Condition is not true",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (predicate != null && !predicate())
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsNotTrue(bool predicate,
      string message = "Condition is true",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (predicate)
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsNotTrue(Func<bool> predicate,
      string message = "Condition is true",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (predicate != null && predicate())
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }

    public static void IsNotEmptyString(string value,
      string message = "Value is required",
      [CallerMemberName] string methodName = "",
      [CallerLineNumber] int lineNumber = 0)
    {
      if (String.IsNullOrEmpty(value))
      {
        throw new ArgumentException($"{message} [Method:{methodName}, Line: {lineNumber}]");
      }
    }
  }
}
