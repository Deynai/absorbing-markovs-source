using UnityEngine;

namespace Deynai.Markov
{
    public class NodeFactory : INodeFactory
    {
        private readonly GameObject _nodePrefab;
        private readonly GameObject _rootRectObject;

        public NodeFactory()
        {
            _rootRectObject = ObjectContainer.CanvasService.GetWorldRootObject();
            _nodePrefab = Resources.Load<GameObject>("Prefabs/NodeSpace/Node");
        }

        public INode Create(Vector2 position)
        {
            if (_nodePrefab == null) { return null; }

            GameObject newNodeObject = Object.Instantiate(_nodePrefab, _rootRectObject.transform);
            RectTransform rect = newNodeObject.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            return newNodeObject.GetComponent<Node>();
        }
    }
}