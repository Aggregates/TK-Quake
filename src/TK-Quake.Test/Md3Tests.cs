using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit;
using Should;
using TKQuake.Engine.Loader.Md3;

namespace TKQuake.Test
{
    /// <summary>
    /// Summary description for Md3Tests
    /// </summary>
    [TestFixture]
    public class Md3Tests
    {
        public Md3Tests()
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

        [Test]
        public void Md3Loading()
        {
            var fileName = Path.Combine("Fixtures", "Md3", "head.md3");
            File.Exists(fileName).ShouldBeTrue();

            var md3 = Md3.FromFile(fileName);
            md3.Id.ShouldEqual("IDP3");
            md3.Version.ShouldEqual(15);
            md3.Tags[0].Name.ShouldEqual("tag_head");
        }
    }
}
