using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    public class NodeSelectionManager
    {
        public List<INode> selectedNodes;
        //public HashSet<Node> hashNodes;

        public NodeSelectionManager()
        {
            selectedNodes = new List<INode>();
        }

        public bool IsSelected(INode node)
        {
            return selectedNodes.Contains(node);
        }

        public void SelectNode(INode node)
        {
            node.SelectEntered();
            selectedNodes.Add(node);
            //hashNodes.Add(node);
        }

        public void ClearSelection()
        {
            foreach (INode node in selectedNodes)
            {
                node.SelectExited();
            }
            selectedNodes.Clear();
            //hashNodes.Clear();
        }
    }
}
