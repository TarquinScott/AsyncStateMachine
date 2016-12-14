
# AsyncStateMachine<br />
<br />
State machine supporting the async operator<br />
See units tests for useage i.e. how to set up guards and triggers.<br />

Example:<br />

//create a state machine to simulate a traffic robot in the Red state<br />
StateMachine sm = new StateMachine("Red");<br />

//configure the Red state permitting a RedToGreen trigger that changes state to Green<br />
sm.Configure("Red")<br />
  .Permit("RedToGreen", "Green")<br />
  .AddEntryAction(() => Debug.WriteLine("Light has turned red"));<br />

//configure the Green state by adding an async task to show how it can be invoked from FireAsync<br />
sm.Configure("Green")<br />
  .AddAsyncEntryAction<br />
  (<br />
    async () =><br />
    {<br />
      await Task.Yield();<br />
      Debug.WriteLine("Light has turned green after an async task");<br />
    }<br />
  );<br />

await sm.FireAsync("RedToGreen");<br />

Assert.IsTrue(sm.State == "Green");<br />

//prevent changing to Red from Green as we need to go to Amber first<br />
sm.Configure("Green")<br />
  .Ignore("GreenToRed");<br />

await sm.FireAsync("GreenToRed");<br />

//trigger ignored and state still Green<br />
Assert.IsTrue(sm.State == "Green");<br />

//allow Green to Amber<br />
sm.Configure("Green")<br />
  .Permit("GreenToAmber", "Amber");<br />

//add an entry action for Amber that simulates a delay and then just go straight to Green with a continuation<br />
sm.Configure("Amber")<br />
  .AddAsyncEntryAction<br />
  (<br />
    async () =><br />
    {<br />
      await Task.Delay(500);<br />
      Debug.WriteLine("Light has turned amber after an async delay");<br />
    }<br />
  )<br />
  .Permit("AmberToRed", "Red")<br />
  .Continue(ContinuationOption.Succeed, "AmberToRed");<br />

await sm.FireAsync("GreenToAmber");<br />

//we gone right from Green to Amber to Red by firing a single trigger (the continuation allows this)<br />
Assert.IsTrue(sm.State == "Red");<br />

//Console output<br />
Debug Trace:<br />
Light has turned green after an async task<br />
Light has turned amber after an async delay<br />
Light has turned red<br />

