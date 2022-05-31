using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deynai.Markov
{
    public struct ConnectionInfo
    {
        public float Weight { get; set; }
        
        public ConnectionInfo(float w)
        {
            Weight = w;
        }
    }

    public class Socket : MonoBehaviour, ISocket, IPointerDownHandler
    {
        [SerializeField] private GameObject nodeObject;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private bool isOutgoing;

        public bool Outgoing { get; private set; }
        //public List<ISocket> Connections { get; private set; } = new List<ISocket>();

        public Dictionary<ISocket, ConnectionInfo> Connections { get; private set; } = new Dictionary<ISocket, ConnectionInfo>();

        //public RectTransform RectTransform { get; private set; }
        public Vector2 Position => AnchoredPosition();
        public INode Node { get; private set; }

        private RectTransform _rectTransform;
        private SocketManager _socketManager;

        private void Awake()
        {
            Outgoing = isOutgoing;
            _rectTransform = rectTransform;
            Node = nodeObject.GetComponent<INode>();

            if (Node == null)
            {
                Debug.Log($"Socket attached to object without a corresponding node - destroying it");
                Destroy(this);
            }

            _socketManager = ObjectContainer.SocketManager;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_socketManager == null) return;

            _socketManager.OnClickedSocket(this);
        }

        public void OnMoved(PointerEventData eventData)
        {
            if (_socketManager == null) return;
            _socketManager.OnSocketMoved(this);
        }

        public bool IsConnected(ISocket socket)
        {
            return Connections.ContainsKey(socket);
        }

        public bool TryConnect(ISocket end)
        {
            if (!this.Outgoing || end.Outgoing) return false;
            if (IsConnected(end)) return false;

            Connections[end] = new ConnectionInfo(0);
            end.Connections[this] = new ConnectionInfo(0);
            return true;
        }

        public void TryDisconnect(ISocket end)
        {
            Connections.Remove(end);
            end.Connections.Remove(this);

            if (_socketManager == null) return;
            _socketManager.OnRemoveConnection(this, end);
        }

        //public void TryEdit(ISocket end)
        //{
        //    TryDisconnect(end);
        //    OnPointerDown(null);
        //}

        private Vector2 AnchoredPosition()
        {
            if (Node == null) { return Vector2.zero; }
            return _rectTransform.anchoredPosition + Node.RectTransform.anchoredPosition;
        }

        public void OnNodeDestroyed()
        {
            foreach (ISocket socket in Connections.Keys)
            {
                socket.Connections.Remove(this);
            }
            _socketManager?.OnSocketRemoved(this);
        }
    }
}