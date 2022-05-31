#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deynai.Markov
{
    /// <summary>
    /// Manager class to handle visualisations of socket connections, draw lines, etc
    /// </summary>
    public class SocketVisualiser : MonoBehaviour, ISocketVisualiser
    {
        private struct Key
        {
            public ISocket Outgoing;
            public ISocket Incoming;
        }

        [SerializeField] private GameObject linePrefab;

        private CanvasService _canvasService;
        private readonly Dictionary<Key, Connection> _lineObjects = new Dictionary<Key, Connection>();
        private readonly List<Connection> _linesToUpdate = new List<Connection>();

        private Connection? _selectedLine;
        private ISocket? _selectedSocket;

        private Coroutine? _selectedUpdateCoroutine;

        private void Awake()
        {
            _canvasService = ObjectContainer.CanvasService;
        }

        public void OnSelectedOutgoingSocket(ISocket socket)
        {
            if (_selectedLine != null)
            {
                Debug.Log($"Overwriting non-null selected line");
            }

            _selectedLine = InstantiateLine();
            _selectedSocket = socket;

            if (_selectedUpdateCoroutine != null)
            {
                Debug.Log($"Coroutine not cancelled before assigning new one!");
                StopAllCoroutines();
            }
            _selectedUpdateCoroutine = StartCoroutine(SelectionMoveCoroutine());

            if (_canvasService == null) { return; }
            _selectedLine.UpdateConnection(socket.Position, _canvasService.MouseToNodeCanvasPosition(Mouse.current.position.ReadValue()));
        }

        public void OnCancelSelection()
        {
            if (_selectedLine == null) { return; }

            _selectedLine.Destroy();
            _selectedLine = null;
            _selectedSocket = null;
            StopCoroutine(_selectedUpdateCoroutine);
            _selectedUpdateCoroutine = null;
        }

        public void OnConnectSockets(ISocket start, ISocket end)
        {
            StopCoroutine(_selectedUpdateCoroutine);
            _selectedUpdateCoroutine = null;

            Debug.Log($"Connect Sockets: {start}, {end}");
            if (_selectedLine == null)
            {
                Debug.Log($"Connecting sockets without a selected line - creating one");
                _selectedLine = InstantiateLine();
            }

            Key key = new Key { Outgoing = start, Incoming = end };
            _lineObjects.Add(key, _selectedLine);

            _selectedLine.Start = start;
            _selectedLine.End = end;

            SetOffsets(start, end, _selectedLine);

            _selectedLine.UpdateConnection(start.Position, end.Position);
            _selectedLine.RectTransform.SetAsFirstSibling();

            _selectedLine = null;
        }

        public void OnSelectionMoved(ISocket selection)
        {
            if (_selectedLine == null) { return; }

            _selectedLine.UpdateConnection(selection.Position, _canvasService.MouseToNodeCanvasPosition(Mouse.current.position.ReadValue()));
        }

        public void OnSocketMoved(ISocket socket)
        {
            _linesToUpdate.Clear();

            foreach (Connection line in _lineObjects.Values)
            {
                if (line.Start == socket || line.End == socket)
                {
                    _linesToUpdate.Add(line);
                }
            }

            foreach (Connection line in _linesToUpdate)
            {
                line.UpdateConnection(line.Start.Position, line.End.Position);
            }
        }

        public void OnRemoveConnection(ISocket start, ISocket end)
        {
            Debug.Log($"Remove Connection between {start} and {end}");

            Key key = new Key { Outgoing = start, Incoming = end };

            if (_lineObjects.ContainsKey(key))
            {
                Debug.Log($"Found connection between {start} and {end}, destroying it");
                _lineObjects[key].Destroy();
                _lineObjects.Remove(key);
            }
            else
            {
                Debug.Log($"Could not find connection to remove");
            }
        }

        public void OnSocketRemoved(ISocket socket)
        {
            List<Key> keysToRemove = new List<Key>();

            foreach (Key key in _lineObjects.Keys)
            {
                if (key.Incoming == socket || key.Outgoing == socket)
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (Key key in keysToRemove)
            {
                if (_lineObjects.ContainsKey(key))
                {
                    _lineObjects[key].Destroy();
                    _lineObjects.Remove(key);
                }
            }
        }

        private Connection InstantiateLine()
        {
            GameObject lineObject = Instantiate(linePrefab, _canvasService.GetWorldRootObject().transform);
            return lineObject.GetComponent<Connection>();
        }

        private IEnumerator SelectionMoveCoroutine()
        {
            while (_selectedSocket != null && _selectedLine != null)
            {
                _selectedLine.UpdateConnection(_selectedSocket.Position, _canvasService.MouseToNodeCanvasPosition(Mouse.current.position.ReadValue()));
                yield return null;
            }

            yield break;
        }

        private void SetOffsets(ISocket start, ISocket end, Connection currentConnection)
        {
            if (start.Node == end.Node)
            {
                currentConnection.SetSameNodeOffsets();
            }
            else if (start.Node.RectTransform.anchoredPosition.x < end.Node.RectTransform.anchoredPosition.x)
            {
                currentConnection.SetOutRightOffsets();
            }
            else
            {
                currentConnection.SetOutLeftOffsets();
            }
        }
    }
}