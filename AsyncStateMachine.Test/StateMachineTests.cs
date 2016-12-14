using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using AsyncStateMachine;

namespace AsyncStateMachine.Test
{
  [TestClass]
  public class StateMachineTests
  {
    [TestMethod]
    public void State_machine_add_configuration()
    {
      StateMachine sm = CreateStateMachine("S1");
      StateConfiguration config = sm.Configure("S1");
      Assert.AreEqual("S1", config.Representation.State);
    }

    [TestMethod]
    public void State_machine_add_trigger()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");
      
      Assert.IsTrue(sm.CanFire("T1"));
    }

    [TestMethod]
    public void State_machine_add_if_trigger()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .PermitIf("T1", "S2", () => false);

      Assert.IsFalse(sm.CanFire("T1"));
    }

    [TestMethod]
    public void State_machine_add_ignore_trigger()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Ignore("T1");

      Assert.IsTrue(sm.CanFire("T1"));
    }

    [TestMethod]
    public void State_machine_add_ignore_if_trigger()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .IgnoreIf("T1", () => false);

      Assert.IsFalse(sm.CanFire("T1"));
    }

    [TestMethod]
    public void State_machine_is_ignored()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Ignore("T1");

      Assert.IsTrue(sm.IsIgnored("T1"));
    }

    [TestMethod]
    public void State_machine_is_not_ignored()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Ignore("T1");

      Assert.IsFalse(sm.IsIgnored("T2"));
    }

    [TestMethod]
    public void State_machine_fire()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Assert.AreEqual("S1", sm.State);
      sm.Fire("T1");
      Assert.AreEqual("S2", sm.State);
      Assert.IsFalse(sm.CanFire("T1"));
    }

    [TestMethod]
    public void State_machine_fire_with_ignore()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Ignore("T1");

      Assert.AreEqual("S1", sm.State);
      sm.Fire("T1");
      Assert.AreEqual("S1", sm.State);
    }

    [TestMethod]
    public void State_machine_fire_with_continue()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      sm.Configure("S2")
        .Permit("T2", "S3")
        .Continue(ContinuationOption.Succeed, "T2");

      Assert.AreEqual("S1", sm.State);
      sm.Fire("T1");
      Assert.AreEqual("S3", sm.State);
    }

    [TestMethod]
    public void State_machine_fire_with_continue_and_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();
       
      sm.Configure("S2")
        .Permit("T2", "S3")
        .Continue(ContinuationOption.Succeed, "T2")
        .ContinueAction(ContinuationOption.Succeed, i.Enter);

      Assert.AreEqual("S1", sm.State);
      sm.Fire("T1");
      Assert.AreEqual("S3", sm.State);
      Assert.IsTrue(i.Entered);
    }

    [TestMethod]
    public async Task State_machine_fire_with_continue_async()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      sm.Configure("S2")
        .Permit("T2", "S3")
        .Continue(ContinuationOption.Succeed, "T2");

      Assert.AreEqual("S1", sm.State);
      await sm.FireAsync("T1");
      Assert.AreEqual("S3", sm.State);
    }

    [TestMethod]
    public async Task State_machine_fire_with_continue_async_and_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S2")
        .Permit("T2", "S3")
        .Continue(ContinuationOption.Succeed, "T2")
        .ContinueAction(ContinuationOption.Succeed, i.Enter);

      Assert.AreEqual("S1", sm.State);
      await sm.FireAsync("T1");
      Assert.AreEqual("S3", sm.State);
      Assert.IsTrue(i.Entered);
    }

    [TestMethod]
    public void State_machine_fire_with_entry_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S2")
        .AddEntryAction(i.Enter);

      sm.Fire("T1");
      Assert.IsTrue(i.Entered);
    }

    [TestMethod]
    public void State_machine_fire_with_exit_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S1")
        .AddExitAction(i.Exit);

      sm.Fire("T1");
      Assert.IsTrue(i.Exited);
    }

    [TestMethod]
    public void State_machine_fire_with_enter_and_exit_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor( );

      sm.Configure("S1")
        .AddExitAction(i.Exit);

      sm.Configure("S2")
        .AddEntryAction(i.Enter);

      sm.Fire("T1");
      Assert.IsTrue(i.Entered);
      Assert.IsTrue(i.Exited);
    }

    [TestMethod]
    public async Task State_machine_fire_with_async_entry_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S2")
        .AddAsyncEntryAction(i.EnterAsync);

      await sm.FireAsync("T1");
      Assert.IsTrue(i.Entered);
    }

    [TestMethod]
    public async Task State_machine_fire_with_async_exit_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S1")
        .AddAsyncExitAction(i.ExitAsync);

      await sm.FireAsync("T1");
      Assert.IsTrue(i.Exited);
    }

    [TestMethod]
    public async Task State_machine_fire_with_enter_and_exit_async_action()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      Interceptor i = new Interceptor();

      sm.Configure("S1")
        .AddAsyncExitAction(i.ExitAsync);

      sm.Configure("S2")
        .AddAsyncEntryAction(i.EnterAsync);

      await sm.FireAsync("T1");
      Assert.IsTrue(i.Entered);
      Assert.IsTrue(i.Exited);
    }

    [TestMethod]
    public void State_machine_set_superstate()
    {
      StateMachine sm = CreateStateMachine("S1");
      sm.Configure("S1")
        .Permit("T1", "S2");

      sm.Configure("S2")
        .SuperState("S1");

      Assert.IsTrue(sm.CanFire("T1"));
      sm.Fire("T1");
      //should be able to fire in state S2 as its super state is S1
      Assert.IsTrue(sm.CanFire("T1"));
    }

    [TestMethod]
    public async Task State_machine_traffic_robot()
    {
      //create a state machine to simulate a traffic robot in the Red state
      StateMachine sm = new StateMachine("Red");

      //configure the Red state permitting a RedToGreen trigger that changes state to Green
      sm.Configure("Red")
        .Permit("RedToGreen", "Green")
        .AddEntryAction(() => Debug.WriteLine("Light has turned red"));

      //configure the Green state by adding an async task to show how it can be invoked from FireAsync
      sm.Configure("Green")
        .AddAsyncEntryAction
        (
          async () =>
          {
            await Task.Yield();
            Debug.WriteLine("Light has turned green after an async task");
          }
        );

      await sm.FireAsync("RedToGreen");
      
      Assert.IsTrue(sm.State == "Green");

      //prevent changing to Red from Green as we need to go to Amber first
      sm.Configure("Green")
        .Ignore("GreenToRed");

      await sm.FireAsync("GreenToRed");

      //trigger ignored and state still Green
      Assert.IsTrue(sm.State == "Green");

      //allow green to amber
      sm.Configure("Green")
        .Permit("GreenToAmber", "Amber");

      //add an entry action for amber that simulates a delay and then just go straight to green with a continuation
      sm.Configure("Amber")
        .AddAsyncEntryAction
        (
          async () =>
          {
            await Task.Delay(500);
            Debug.WriteLine("Light has turned amber after an async delay");
          }
        )
        .Permit("AmberToRed", "Red")
        .Continue(ContinuationOption.Succeed, "AmberToRed");

      await sm.FireAsync("GreenToAmber");

      //we gone right from Green to Amber to Red by firing a single trigger (the continuation allows this)
      Assert.IsTrue(sm.State == "Red");
    }

    private StateMachine CreateStateMachine(string initialState)
    {
      return new StateMachine(initialState);
    }

    internal class Interceptor
    {
      public bool Entered { get; set; }

      public void Enter()
      {
        Entered = true;
      }

      public async Task EnterAsync()
      {
        await Task.Run(() => Entered = true);
      }

      public bool Exited { get; set; }

      public void Exit()
      {
        Exited = true;
      }

      public async Task ExitAsync()
      {
        await Task.Run(() => Exited = true);
      }
    }
  }
}
