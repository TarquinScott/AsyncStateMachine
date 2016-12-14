
# AsyncStateMachine

State machine supporting the async operator in c#
See units tests for useage i.e. how to set up guards and triggers.

Example:

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

//allow Green to Amber
sm.Configure("Green")
  .Permit("GreenToAmber", "Amber");

//add an entry action for Amber that simulates a delay and then just go straight to Green with a continuation
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

//Console output
Debug Trace:
Light has turned green after an async task
Light has turned amber after an async delay
Light has turned red

