using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deynai.Markov
{
    public interface ISocket
    {
        public bool Outgoing { get; }
        public Dictionary<ISocket, ConnectionInfo> Connections { get; }
        public Vector2 Position { get; }
        public INode Node { get; }
        public bool IsConnected(ISocket socket);
        public bool TryConnect(ISocket end);
        public void TryDisconnect(ISocket end);
        public void OnMoved(PointerEventData eventData);
        public void OnNodeDestroyed();
    }
}