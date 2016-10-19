using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Erwine.Leonard.T.SsmlNotePad.Common;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for SynchronizedStateTest
    /// </summary>
    [TestClass]
    public class SynchronizedStateTest
    {
        public SynchronizedStateTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
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

        [TestMethod]
        public void SynchronizedStateTestMethod1()
        {
            int expected = 1;
            Debug.WriteLine("Instantiating target");
            SynchronizedState<int> target = new SynchronizedState<int>(expected);
            int actual = target.CurrentState;
            Assert.AreEqual(expected, actual);
            Debug.WriteLine("Initial: Checking WaitStateChangeActive");
            Assert.IsFalse(target.WaitStateChangeActive(10));
            Debug.WriteLine("Initial: Checking WaitStateChangeInactive");
            Assert.IsTrue(target.WaitStateChangeInactive(10));
            Debug.WriteLine("Initial: Checking WaitStateChanged");
            Assert.IsFalse(target.WaitStateChanged(10));
            Debug.WriteLine("Initial: Checking StateChanging");
            Assert.IsFalse(target.StateChanging);
            object expectedUserState1 = "7";
            object expectedUserState2 = 7;
            int expected3 = 3;
            Task<Tuple<bool, bool, bool, bool, int, int, object>> stateChangeTask;
            using (AutoResetEvent okayToDispose = new AutoResetEvent(false))
            {
                int expected2 = 2;
                Debug.WriteLine("Invoking ChangeStateA");
                Task<bool> taskChanged;
                using (SynchronizedStateChange<int> stateChangeA = target.ChangeState(expectedUserState1))
                {
                    Debug.WriteLine("ChangeStateA instantiated");
                    actual = target.CurrentState;
                    Assert.AreEqual(expected, actual);
                    actual = stateChangeA.NewState;
                    Assert.AreEqual(expected, actual);
                    Debug.WriteLine("A: Checking WaitStateChangeActive");
                    Assert.IsTrue(target.WaitStateChangeActive(10));
                    Debug.WriteLine("A: Checking WaitStateChangeInactive");
                    Assert.IsFalse(target.WaitStateChangeInactive(10));
                    Debug.WriteLine("A: Checking WaitStateChanged");
                    Assert.IsFalse(target.WaitStateChanged(10));
                    Debug.WriteLine("A: Checking StateChanging");
                    Assert.IsTrue(target.StateChanging);
                    object actualUserState = stateChangeA.UserState;
                    Assert.AreEqual(expectedUserState1, actualUserState);

                    using (AutoResetEvent parentReady = new AutoResetEvent(false))
                    {
                        using (AutoResetEvent taskReady = new AutoResetEvent(false))
                        {
                            stateChangeTask = Task<Tuple<bool, bool, bool, bool, int, int, object>>.Factory.StartNew(() =>
                            {
                                parentReady.WaitOne();
                                taskReady.Set();
                                Tuple<bool, bool, bool, bool, int, int, object> result;
                                Debug.WriteLine("Invoking ChangeStateB");
                                using (SynchronizedStateChange<int> changeStateB = target.ChangeState(expectedUserState2))
                                {
                                    Debug.WriteLine("ChangeStateB instantiated");
                                    result = new Tuple<bool, bool, bool, bool, int, int, object>(target.StateChanging, target.WaitStateChangeActive(10),
                                        target.WaitStateChangeInactive(10), target.WaitStateChanged(10), changeStateB.CurrentState, changeStateB.NewState,
                                        changeStateB.UserState);
                                    Debug.WriteLine("B: Changing state");
                                    changeStateB.NewState = expected3;
                                    okayToDispose.WaitOne();
                                    Debug.WriteLine("B: Disposing");
                                }
                                Debug.WriteLine("B: Disposed");

                                return result;
                            });
                            parentReady.Set();
                            taskReady.WaitOne();
                        }
                    }

                    actual = target.CurrentState;
                    Assert.AreEqual(expected, actual);
                    actual = stateChangeA.NewState;
                    Assert.AreEqual(expected, actual);
                    Debug.WriteLine("A: Checking WaitStateChangeActive");
                    Assert.IsTrue(target.WaitStateChangeActive(10));
                    Debug.WriteLine("A: Checking WaitStateChangeInactive");
                    Assert.IsFalse(target.WaitStateChangeInactive(10));
                    Debug.WriteLine("A: Checking WaitStateChanged");
                    Assert.IsFalse(target.WaitStateChanged(10));
                    Debug.WriteLine("A: Checking StateChanging");
                    Assert.IsTrue(target.StateChanging);
                    actualUserState = stateChangeA.UserState;
                    Assert.AreEqual(expectedUserState1, actualUserState);

                    Debug.WriteLine("A: Changing state");
                    stateChangeA.NewState = expected2;

                    actual = target.CurrentState;
                    Assert.AreEqual(expected, actual);
                    actual = stateChangeA.NewState;
                    Assert.AreEqual(expected2, actual);
                    Debug.WriteLine("A: Checking WaitStateChangeActive");
                    Assert.IsTrue(target.WaitStateChangeActive(10));
                    Debug.WriteLine("A: Checking WaitStateChangeInactive");
                    Assert.IsFalse(target.WaitStateChangeInactive(10));
                    Debug.WriteLine("A: Checking WaitStateChanged");
                    Assert.IsFalse(target.WaitStateChanged(10));
                    Debug.WriteLine("A: Checking StateChanging");
                    Assert.IsTrue(target.StateChanging);
                    actualUserState = stateChangeA.UserState;
                    Assert.AreEqual(expectedUserState1, actualUserState);
                    Debug.WriteLine("A: Disposing");
                    taskChanged = Task<bool>.Factory.StartNew(() =>
                    {
                        Debug.WriteLine("A: Waiting for WaitStateChanged");
                        bool result = target.WaitStateChanged(10000);
                        Debug.WriteLine((result) ? "A: WaitStateChanged raised" : "A: WaitStateChanged NOT raised");
                        return result;
                    });
                }
                Debug.WriteLine("A: Disposed");
                Assert.IsTrue(taskChanged.Result);

                expected = expected2;

                actual = target.CurrentState;
                Assert.AreEqual(expected, actual);
                Assert.IsTrue(target.WaitStateChangeActive(10));
                Assert.IsFalse(target.WaitStateChangeInactive(10));
                Assert.IsFalse(target.WaitStateChanged(10));
                Assert.IsTrue(target.StateChanging);

                taskChanged = Task<bool>.Factory.StartNew(() =>
                {
                    Debug.WriteLine("B: Waiting for WaitStateChanged");
                    bool result = target.WaitStateChanged(10000);
                    Debug.WriteLine((result) ? "B: WaitStateChanged raised" : "B: WaitStateChanged NOT raised");
                    return result;
                });
                okayToDispose.Set();
                Assert.IsTrue(taskChanged.Result);
            }
            
            Tuple<bool, bool, bool, bool, int, int, object> results = stateChangeTask.Result;
            
            actual = target.CurrentState;
            Assert.AreEqual(expected3, actual);
            Debug.WriteLine("Final: Checking WaitStateChangeActive");
            Assert.IsFalse(target.WaitStateChangeActive(10));
            Debug.WriteLine("Final: Checking WaitStateChangeInactive");
            Assert.IsTrue(target.WaitStateChangeInactive(10));
            Debug.WriteLine("Final: Checking WaitStateChanged");
            Assert.IsFalse(target.WaitStateChanged(10));
            Debug.WriteLine("Final: Checking StateChanging");
            Assert.IsFalse(target.StateChanging);

            Assert.IsTrue(results.Item1); // target.StateChanging
            Assert.IsTrue(results.Item2); // target.WaitStateChangeActive
            Assert.IsFalse(results.Item3); // target.WaitStateChangeInactive
            Assert.IsFalse(results.Item4); // target.WaitStateChanged
            Assert.AreEqual(expected, results.Item5); // changeStateB.CurrentState
            Assert.AreEqual(expected, results.Item6); // changeStateB.NewState
            Assert.AreEqual(expectedUserState2, results.Item7); // changeStateB.UserState
        }
    }
}
