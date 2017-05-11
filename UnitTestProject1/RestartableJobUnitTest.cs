using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.Common;
using System.Threading;
using System.Collections.ObjectModel;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for RestartableJobUnitTest
    /// </summary>
    [TestClass]
    public class RestartableJobUnitTest
    {
        public RestartableJobUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        
        public class RestartableJobArgs : IDisposable
        {
            public RestartableJobArgs(string text)
            {
                Text = text;
                BackgroundWaiter = new ManualResetEvent(false);
                WorkStarted = new ManualResetEvent(false);
            }
            public string Text { get; private set; }
            public ManualResetEvent BackgroundWaiter { get; private set; }
            public ManualResetEvent WorkStarted { get; private set; }
            public int Counter { get; set; }
            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        BackgroundWaiter.Dispose();
                        WorkStarted.Dispose();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~RestartableJobArgs() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

        public class EventParams
        {
            public string EventName { get; private set; }
            public EventArgs Args { get; private set; }
            public object Sender { get; private set; }
            public EventParams(string eventName, object sender, EventArgs args)
            {
                EventName = eventName;
                Sender = sender;
                Args = args;
            }

            public override string ToString()
            {
                return String.Format("{0} - {1}", EventName, Args.ToString());
            }
        }

        public class RestartableJobHandler : IDisposable
        {
            private object _syncRoot = new object();
            private ManualResetEvent _workerEvent = new ManualResetEvent(false);
            public Collection<EventParams> EventParams { get; private set; }

            public RestartableJobHandler() { EventParams = new Collection<EventParams>(); }

            public void Job_WorkerFault(object sender, WorkerEventArgs<RestartableJobArgs, Exception> e)
            {
                lock (_syncRoot)
                {
                    EventParams.Add(new EventParams("WorkerFault", sender, e));
                    _workerEvent.Set();
                }
            }

            public void Job_WorkerCanceled(object sender, WorkerEventArgs<RestartableJobArgs> e)
            {
                lock (_syncRoot)
                {
                    EventParams.Add(new EventParams("WorkerCanceled", sender, e));
                    _workerEvent.Set();
                }
            }

            public void Job_WorkerComplete(object sender, WorkerEventArgs<RestartableJobArgs, int> e)
            {
                lock (_syncRoot)
                {
                    EventParams.Add(new EventParams("WorkerComplete", sender, e));
                    _workerEvent.Set();
                }
            }

            public bool WaitOne(out Collection<EventParams> eventParams)
            {
                if (_workerEvent.WaitOne())
                {
                    lock (_syncRoot)
                    {
                        eventParams = EventParams;
                        EventParams = new Collection<EventParams>();
                        _workerEvent.Reset();
                    }
                }
                else
                    eventParams = null;

                return eventParams != null;
            }

            public bool WaitOne(int millisecondsTimeout, out Collection<EventParams> eventParams)
            {
                if (_workerEvent.WaitOne(millisecondsTimeout))
                {
                    lock (_syncRoot)
                    {
                        eventParams = EventParams;
                        EventParams = new Collection<EventParams>();
                        _workerEvent.Reset();
                    }
                }
                else
                    eventParams = null;

                return eventParams != null;
            }
            
            #region IDisposable Support

            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _workerEvent.Dispose();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~RestartableJobHandler() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
        }

        [TestMethod]
        public void RestartableJobTestMethod()
        {
            using (RestartableJob<RestartableJobArgs, int> job = new RestartableJob<RestartableJobArgs, int>())
            {
                RestartableJobHandler handler = new RestartableJobHandler();
                job.WorkerCanceled += handler.Job_WorkerCanceled;
                job.WorkerComplete += handler.Job_WorkerComplete;
                job.WorkerFault += handler.Job_WorkerFault;
                using (RestartableJobArgs argsA = new RestartableJobArgs("1"))
                {
                    job.Start(RestartableJobWorkerMethod, argsA);
                    while (!argsA.WorkStarted.WaitOne(10))
                    {
                        Collection<EventParams> eventParams;
                        if (handler.WaitOne(0, out eventParams))
                        {
                            foreach (EventParams p in eventParams)
                                TestContext.WriteLine(p.ToString());
                            Assert.Fail("Unexpected {0} event", eventParams.Last().EventName);
                        }
                    }
                    using (RestartableJobArgs argsB = new RestartableJobArgs("2"))
                    {
                        job.Start(RestartableJobWorkerMethod, argsB);
                        while (!argsB.WorkStarted.WaitOne(10))
                        {
                            Collection<EventParams> eventParams;
                            if (handler.WaitOne(0, out eventParams))
                            {
                                foreach (EventParams p in eventParams)
                                    TestContext.WriteLine(p.ToString());
                                Assert.Fail("Unexpected {0} event", eventParams.Last().EventName);
                            }
                        }
                        Assert.AreEqual(2, argsA.Counter);
                        Assert.AreEqual(2, argsB.Counter);
                        argsA.BackgroundWaiter.Set();
                        argsB.BackgroundWaiter.Set();
                        bool canceled = false, completed = false;
                        int waitCount = 0;
                        while (!canceled && !completed)
                        {
                            Collection<EventParams> eventParams;
                            if (handler.WaitOne(1000, out eventParams))
                            {
                                foreach (EventParams p in eventParams)
                                {
                                    if (p.EventName == "WorkerCanceled")
                                    {
                                        if (ReferenceEquals((p.Args as WorkerEventArgs<RestartableJobArgs>).Arg, argsA) && !canceled)
                                        {
                                            canceled = true;
                                            continue;
                                        }
                                    }
                                    else if (p.EventName == "WorkerComplete" && ReferenceEquals((p.Args as WorkerEventArgs<RestartableJobArgs, int>).Arg, argsB) && !completed)
                                    {
                                        Assert.AreEqual(2, (p.Args as WorkerEventArgs<RestartableJobArgs, int>).Result);
                                        completed = true;
                                        continue;
                                    }
                                    TestContext.WriteLine(p.ToString());
                                    Assert.Fail("Unexpected {0} event", eventParams.Last().EventName);
                                }
                            }
                            if (waitCount == 5 && !(canceled && completed))
                                Assert.Fail((canceled) ? "Did not get completed event." : ((completed) ? "Did not get canceled event." : "Did not get expected events."));
                        }
                        Assert.AreEqual(3, argsA.Counter);
                        Assert.AreEqual(4, argsB.Counter);
                    }
                }
            }
        }

        private int RestartableJobWorkerMethod(RestartableJobArgs args, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return 0;
            args.Counter++;
            TestContext.WriteLine("{0} starting.", args.Text);
            args.WorkStarted.Set();
            if (token.IsCancellationRequested)
            {
                TestContext.WriteLine("{0} cancellation requested.", args.Text);
                return 0;
            }
            args.Counter++;
            TestContext.WriteLine("{0} waiting.", args.Text);
            args.BackgroundWaiter.WaitOne();
            TestContext.WriteLine("{0} proceeding.", args.Text);
            args.Counter++;
            if (token.IsCancellationRequested)
            {
                TestContext.WriteLine("{0} cancellation requested.", args.Text);
                return 0;
            }
            args.Counter++;
            TestContext.WriteLine("{0} returning.", args.Text);
            return int.Parse(args.Text);
        }
    }
}
