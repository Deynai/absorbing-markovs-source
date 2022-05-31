using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Deynai.Markov.Tests
{
    public class NodeManagerTests
    {
        private NodeManager nodeManager;

        [SetUp]
        public void Setup()
        {
            INodeFactory factory = new TestFactory();
            nodeManager = new NodeManager(factory);
        }

        [TearDown]
        public void Teardown()
        {
            nodeManager = null;
        }

        [Test]
        public void NodeManager_Initialised_Empty()
        {
            INode returnedNode = nodeManager.GetNodeForID(0);
            Assert.IsNull(returnedNode);

            int nodeCount = nodeManager.GetAllNodes().Count;
            Assert.AreEqual(0, nodeCount);

            int nodeIDsCount = nodeManager.GetAllNodeIDs().Count;
            Assert.AreEqual(0, nodeIDsCount);
        }

        [Test]
        public void NodeManager_RegisteredNode_IsStored()
        {
            INode node = nodeManager.CreateNewNode();

            INode returnedNode = nodeManager.GetNodeForID(node.ID);
            Assert.AreEqual(node, returnedNode);

            IReadOnlyList<INode> allNodes = nodeManager.GetAllNodes();
            Assert.AreEqual(node, allNodes[0]);

            int nodeCount = allNodes.Count;
            Assert.AreEqual(1, nodeCount);

            int idCount = nodeManager.GetAllNodeIDs().Count;
            Assert.AreEqual(1, idCount);
        }

        [Test]
        public void NodeManager_RegisterMany_IsStored()
        {
            int amount = 100;

            List<INode> testNodes = new List<INode>();
            for (int i = 0; i < amount; i++)
            {
                INode node = nodeManager.CreateNewNode();
                testNodes.Add(node);
            }

            foreach (INode n in testNodes)
            {
                Assert.NotNull(nodeManager.GetNodeForID(n.ID));
            }

            int nodeCount = nodeManager.GetAllNodes().Count;
            Assert.AreEqual(amount, nodeCount);

            int idCount = nodeManager.GetAllNodeIDs().Count;
            Assert.AreEqual(amount, idCount);
        }

        [Test]
        public void NodeManager_Unregister_IsRemoved()
        {
            INode node = nodeManager.CreateNewNode();

            INode returnedNode = nodeManager.GetNodeForID(node.ID);
            Assert.AreEqual(node, returnedNode);
            Assert.AreEqual(1, nodeManager.GetAllNodes().Count);
            Assert.AreEqual(1, nodeManager.GetAllNodeIDs().Count);

            nodeManager.UnregisterNode(node);
            Assert.IsNull(nodeManager.GetNodeForID(node.ID));
            Assert.AreEqual(0, nodeManager.GetAllNodes().Count);
            Assert.AreEqual(0, nodeManager.GetAllNodeIDs().Count);
        }

        [Test]
        public void NodeManager_RegisterMany_AreUnique()
        {
            int amount = 100;

            List<INode> testNodes = new List<INode>();
            for (int i = 0; i < amount; i++)
            {
                INode node = nodeManager.CreateNewNode();
                testNodes.Add(node);
            }

            HashSet<ulong> idsSeen = new HashSet<ulong>();

            foreach (INode n in testNodes)
            {
                idsSeen.Add(n.ID);
            }

            Assert.AreEqual(amount, idsSeen.Count);
        }
    }
}