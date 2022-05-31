using Deynai.Markov;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager
{
    private readonly List<ulong> _allNodeIDs;
    private readonly List<INode> _allNodes;
    private readonly Dictionary<ulong, INode> _nodeById;

    private ulong s_lastID = 0;

    private readonly INodeFactory _factory;

    public NodeManager(INodeFactory factory)
    {
        _factory = factory;

        _allNodeIDs = new List<ulong>();
        _allNodes = new List<INode>();
        _nodeById = new Dictionary<ulong, INode>();
    }

    public INode CreateNewNode(Vector2 position)
    {
        INode node = _factory.Create(position);
        RegisterNode(node);
        return node;
    }

    public INode CreateNewNode()
    {
        INode node = _factory.Create(Vector2.zero);
        RegisterNode(node);
        return node;
    }

    public ulong RegisterNode(INode node)
    {
        ulong id = GetUniqueID();
        _allNodeIDs.Add(id);
        _allNodes.Add(node);
        _nodeById[id] = node;
        node.SetID(id);

        return id;
    }

    public void UnregisterNode(INode node)
    {
        ulong id = node.ID;
        _allNodeIDs.Remove(id);
        _allNodes.Remove(node);
        _nodeById.Remove(id);
    }

    public ulong GetUniqueID()
    {
        return s_lastID++;
    }

    public INode GetNodeForID(ulong id)
    {
        _nodeById.TryGetValue(id, out INode result);
        return result;
    }

    public IReadOnlyList<INode> GetAllNodes()
    {
        return _allNodes;
    }

    public IReadOnlyList<ulong> GetAllNodeIDs()
    {
        return _allNodeIDs;
    }
}
