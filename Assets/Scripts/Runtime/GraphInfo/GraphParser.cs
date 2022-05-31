using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Deynai.Markov
{
    public class GraphParser
    {
        public Dictionary<ulong, int> NodeIDToIndexMap => _nodeIDToIndexMap;
        public ulong[] NodeIndexToIDMap => _nodeIndexToIDMap;
        public Matrix Transition => _transitionMatrix;
        public Matrix Fundamental => _fundamentalMatrix;
        public Matrix R => _rMatrix;
        public bool Valid => _isValid;

        private Dictionary<ulong, int> _nodeIDToIndexMap;
        private ulong[] _nodeIndexToIDMap;

        private Matrix _transitionMatrix;
        private Matrix _fundamentalMatrix;
        private Matrix _rMatrix;
        private bool _isValid = false;

        private NodeManager _nodeManager;

        public GraphParser()
        {
            _nodeManager = ObjectContainer.NodeManager;
        }

        // This should take in all nodes and build a transition matrix for it
        public void GenerateTransitionMatrix()
        {
            var allNodes = _nodeManager.GetAllNodes();
            var size = allNodes.Count;

            if (!FindErrorsOnInvalidNodes(allNodes))
            {
                return;
            }
            _isValid = true;

            List<INode> absorbingNodes = allNodes.Where(n => n.Sockets.Where(s => s.Outgoing).All(s => s.Connections.Where(c => c.Value.Weight > 0).All(k => k.Key.Node == s.Node))).ToList();
            List<INode> transientNodes = allNodes.Where(n => !absorbingNodes.Contains(n)).ToList();
            BuildNodeIndexMapping(transientNodes, absorbingNodes);

            var outgoingConnections = AllOutgoingConnections(allNodes);
            double[] transitionMatrix = new double[size * size];
            foreach (Link link in outgoingConnections)
            {
                int row = _nodeIDToIndexMap[link.start.ID];
                int col = _nodeIDToIndexMap[link.end.ID];
                int mIndex = col + row * size;
                transitionMatrix[mIndex] = link.weight;
            }

            int fdSize = transientNodes.Count;
            double[] fundamentalMatrix = new double[fdSize * fdSize];
            for (int rows = 0; rows < fdSize; rows++)
            {
                for (int cols = 0; cols < fdSize; cols++)
                {
                    double val = rows == cols ? 1 : 0;
                    fundamentalMatrix[cols + fdSize * rows] = val - transitionMatrix[cols + size * rows];
                }
            }

            int rSizeRows = fdSize;
            int rSizeCols = absorbingNodes.Count;
            double[] rMatrix = new double[rSizeRows * rSizeCols];
            for (int rows = 0; rows < rSizeRows; rows++)
            {
                for (int cols = 0; cols < rSizeCols; cols++)
                {
                    rMatrix[cols + rSizeCols * rows] = transitionMatrix[cols + fdSize + size * rows];
                }
            }

            _transitionMatrix = new Matrix(transitionMatrix, size, size);
            Matrix IQmatrix = new Matrix(fundamentalMatrix, fdSize, fdSize);
            _fundamentalMatrix = Matrix.Invert(IQmatrix);
            _rMatrix = new Matrix(rMatrix, rSizeCols, rSizeRows);

            DebugTesting();
        }

        private bool FindErrorsOnInvalidNodes(IReadOnlyList<INode> nodes)
        {
            Help.Clear();

            if (nodes.Count == 0)
            {
                Help.Notify(this, "404: Nodes not found");
                return false;
            }

            List<string> warnings = new List<string>();
            foreach (INode node in nodes)
            {
                if (node.Sockets.Where(s => s.Outgoing).FirstOrDefault() == null) { warnings.Add($"{node.Name} has no outgoing sockets"); continue; }

                float sum = 0f;
                bool negativeWeight = false;
                foreach (ISocket socket in node.Sockets.Where(s => s.Outgoing))
                {
                    if (socket.Connections.Count == 0) continue;
                    //sum += socket.Connections.Values.Select(c => Mathf.Max(0, c.Weight)).Aggregate((total, w) => total + w);
                    foreach (var connection in socket.Connections.Values)
                    {
                        if (connection.Weight < 0)
                        {
                            negativeWeight = true;
                        }
                        sum += Mathf.Max(0, connection.Weight);
                    }
                }

                if (negativeWeight)
                {
                    warnings.Add($"{node.Name}: Connection weights should be positive");
                }
                else if (!Mathf.Approximately(1f, sum))
                {
                    warnings.Add($"{node.Name}: Outgoing connections do not sum to 1");
                    continue;
                }
            }

            if (warnings.Count > 0)
            {
                foreach (string warning in warnings)
                {
                    Help.Notify(this, warning);
                }
                return false;
            }

            return true;
        }

        private void DebugTesting()
        {
            Debug.Log($"TransitionMatrix:\n{_transitionMatrix}");

            Debug.Log($"FundamentalMatrix:\n{_fundamentalMatrix}");
            
            Matrix bMatrix = Matrix.Multiply(_fundamentalMatrix, _rMatrix);
            Debug.Log($"B-Matrix:\n{bMatrix}");
            //Debug.Log($"{string.Join(",", _transitionMatrix.Value)}");
        }

        private void BuildNodeIndexMapping(List<INode> transientNodes, List<INode> absorbingNodes)
        {
            int numberOfNodes = transientNodes.Count + absorbingNodes.Count;
            _nodeIDToIndexMap = new Dictionary<ulong, int>();
            _nodeIndexToIDMap = new ulong[numberOfNodes];

            int index = 0;
            for (int i = 0; i < transientNodes.Count; i++)
            {
                _nodeIDToIndexMap[transientNodes[i].ID] = index;
                _nodeIndexToIDMap[index] = transientNodes[i].ID;
                index++;
            }

            for (int i = 0; i < absorbingNodes.Count; i++)
            {
                _nodeIDToIndexMap[absorbingNodes[i].ID] = index;
                _nodeIndexToIDMap[index] = absorbingNodes[i].ID;
                index++;
            }
        }

        private IEnumerable<Link> AllOutgoingConnections(IEnumerable<INode> nodes)
        {
            return nodes
                .SelectMany(node => node.Sockets.Where(socket => socket.Outgoing))
                .SelectMany(socket => socket.Connections.Keys
                    .Select(otherSocket =>
                    {
                        return new Link { start = socket.Node, end = otherSocket.Node, weight = socket.Connections[otherSocket].Weight };
                    })
                );
        }

        private struct Link
        {
            public INode start;
            public INode end;
            public float weight;
        }
    }
}
