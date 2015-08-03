using System;
using NUnit.Framework;
using GameLoop.Engine.Infrastructure.GameScreen;
using GameLoop.ScreenStates;

namespace GameLoop.Test
{
    [TestFixture]
    public class StateManagerTest
    {
        [Test]
        public void Test_Add_EmptyManager_Add()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen newState = new SplashScreen(manager);
            string stateName = "newState";

            try
            {
                manager.Add(stateName, newState);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

            Assert.IsTrue(manager.Registered(stateName));
        }

        [Test]
        public void Test_Add_RegisteredState_ThrowException()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen newState = new SplashScreen(manager);
            string stateName = "newState";

            try
            {
                manager.Add(stateName, newState);
                manager.Add(stateName, newState);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void Test_ChangeState_RegisteredState_UpdateState()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen startState = new SplashScreen(manager);
            SplashScreen newState = new SplashScreen(manager);
            string startStateName = "startState";
            string newStateName = "newState";

            try
            {
                manager.Add(startStateName, startState);
                Assert.IsTrue(manager.Registered(startStateName));
                Assert.IsTrue(manager.ActiveScreen == startState);
                
                manager.Add(newStateName, newState);
                Assert.IsTrue(manager.Registered(newStateName));

                manager.ChangeScreen(newStateName);
                Assert.IsTrue(manager.ActiveScreen == newState);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Test_ChangeState_UnregisteredState_ThrowException()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen startState = new SplashScreen(manager);
            string startStateName = "startState";
            string invalidStateName = "invalidState";

            try
            {
                manager.Add(startStateName, startState);
                Assert.IsTrue(manager.Registered(startStateName));
                Assert.IsTrue(manager.ActiveScreen == startState);

                manager.ChangeScreen(invalidStateName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void Test_Remove_UnregisteredState_ThrowException()
        {
            ScreenManager manager = new ScreenManager();
            string invalidStateName = "invalidState";

            try
            {
                manager.Remove(invalidStateName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void Test_Remove_ActiveState_ThrowException()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen startState = new SplashScreen(manager);
            string startStateName = "startState"; 

            try
            {
                manager.Add(startStateName, startState);
                Assert.IsTrue(manager.Registered(startStateName));
                Assert.IsTrue(manager.ActiveScreen == startState);

                manager.Remove(startStateName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void Test_Remove_ValidState_Remove()
        {
            ScreenManager manager = new ScreenManager();
            SplashScreen startState = new SplashScreen(manager);
            SplashScreen newState = new SplashScreen(manager);
            string startStateName = "startState";
            string newStateName = "newState";

            try
            {
                manager.Add(startStateName, startState);
                Assert.IsTrue(manager.Registered(startStateName));

                manager.Add(newStateName,newState);
                Assert.IsTrue(manager.Registered(newStateName));

                Assert.IsTrue(manager.ActiveScreen == startState);
                manager.Remove(newStateName);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

            Assert.IsTrue(manager.Registered(startStateName));
            Assert.IsTrue(!manager.Registered(newStateName));
        }

    }
}
