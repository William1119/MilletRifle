using UnityEngine;
using System.Collections;
using NUnit.Framework;

namespace BehaviorNodeUnitTest
{
    internal class PreconEffectorTest
    {
        public PreconEffectorAgent testAgent = null;

        [TestFixtureSetUp]
        public void initGlobalTestEnv() {
            BehaviacSystem.Instance.Init();

            GameObject testAgentObject = new GameObject();
            testAgentObject.name = "@UnitTestAgent";
            testAgentObject.transform.localPosition = Vector3.zero;
            testAgentObject.transform.localRotation = Quaternion.identity;
            testAgentObject.transform.localScale = Vector3.one;
            testAgent = testAgentObject.AddComponent<PreconEffectorAgent>();
            testAgent.init();

            //Debug.Log("InitTestFixture");
        }

        [TestFixtureTearDown]
        public void finlGlobalTestEnv() {
            testAgent.finl();

            BehaviacSystem.Instance.Uninit();
            //Debug.Log("FinlTestFixture");
        }

        [SetUp]
        public void initTestEnv() {
        }

        [TearDown]
        public void finlTestEnv() {
            testAgent.btunloadall();
        }

        [Test]
        [Category("test_precond_effector")]
        public void test_precondition() {
            testAgent.btsetcurrent("node_test/PreconditionEffectorTest/PreconditionEffectorTest_0");
            testAgent.resetProperties();
            testAgent.count_both = 1;
            testAgent.btexec();

            //precondition failed
            Assert.AreEqual(0, testAgent.get_count_success());
            Assert.AreEqual(0, testAgent.count_failure);
            Assert.AreEqual(1, testAgent.count_both);
        }


        [Test]
        [Category("test_precond_effector")]
        public void test_precondition_alive() {
            testAgent.btsetcurrent("node_test/PreconditionEffectorTest/PreconditionEffectorTest_1");
            testAgent.resetProperties();
            behaviac.EBTStatus status = behaviac.EBTStatus.BT_INVALID;

            for (int i = 0; i < 10; ++i) {
                status = testAgent.btexec();
                Assert.AreEqual(behaviac.EBTStatus.BT_RUNNING, status);

                Assert.AreEqual(0, testAgent.get_count_success());
                Assert.AreEqual(0, testAgent.count_failure);
                Assert.AreEqual(0, testAgent.count_both);
            }

            testAgent.count_both = 1;
            status = testAgent.btexec();
            Assert.AreEqual(behaviac.EBTStatus.BT_FAILURE, status);

            Assert.AreEqual(0, testAgent.get_count_success());
            Assert.AreEqual(1, testAgent.count_failure);
            Assert.AreEqual(2, testAgent.count_both);
            Assert.AreEqual(5, testAgent.ret);
        }

        [Test]
        [Category("test_precond_effector")]
        public void test_effector() {
            testAgent.btsetcurrent("node_test/PreconditionEffectorTest/PreconditionEffectorTest_0");
            testAgent.resetProperties();
            testAgent.btexec();

            //success/failure/both effectors
            Assert.AreEqual(1, testAgent.get_count_success());
            Assert.AreEqual(1, testAgent.count_failure);
            Assert.AreEqual(2, testAgent.count_both);
        }

    }
}
