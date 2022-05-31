using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Deynai.Markov
{
    public class Node : MonoBehaviour, INode
    {
        // Inspector
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private Image _highlightBorder;
        [SerializeField] private List<GameObject> socketObjects;

        // Properties
        public ulong ID { get; private set; }
        public string Name { get; set; }
        public RectTransform RectTransform => _rectTransform;

        public IReadOnlyList<ISocket> Sockets => _sockets;

        // Values
        //private string _name;
        private List<ISocket> _sockets;

        // Interactions
        private Color _hoverColor = Color.white;
        private Color _selectedColor = Color.cyan;
        private InteractionState _interactionState;

        // References
        private RectTransform _rectTransform;

        // Statics/Globals
        private static readonly string defaultName = "New Node";
        private NodeManager _nodeManager;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            InitialiseSockets();
            _nodeManager = ObjectContainer.NodeManager;
        }

        private void InitialiseSockets()
        {
            _sockets = new List<ISocket>();
            foreach (var socketObject in socketObjects)
            {
                ISocket socket = socketObject.GetComponent<ISocket>();
                if (socket != null)
                {
                    _sockets.Add(socketObject.GetComponent<ISocket>());
                }
            }
        }

        public void SetState(InteractionState state)
        {
            _interactionState = state;
            UpdateHighlightColour();
        }

        private void UpdateHighlightColour()
        {
            if (this == null) { return; }
            Color col;

            switch (_interactionState)
            {
                case InteractionState.None:
                    col = _selectedColor;
                    col.a = 0f;
                    _highlightBorder.color = col;
                    break;
                case InteractionState.Hover:
                    col = _hoverColor;
                    col.a = 1f;
                    _highlightBorder.color = col;
                    break;
                case InteractionState.Selected:
                    col = _selectedColor;
                    col.a = 1f;
                    _highlightBorder.color = col;
                    break;
                default:
                    col = _selectedColor;
                    col.a = 0f;
                    _highlightBorder.color = col;
                    break;
            }
        }

        public void SetID(ulong id)
        {
            ID = id;
            Name = defaultName + " " + id;
            nameField.text = Name;
        }

        public void OnNameFieldUpdated(string name)
        {
            Name = name;
        }

        public void SelectEntered()
        {
            SetState(InteractionState.Selected);
        }

        public void SelectExited()
        {
            SetState(InteractionState.None);
        }

        public void HoverEntered()
        {
            SetState(InteractionState.Hover);
        }

        public void HoverExited()
        {
            SetState(InteractionState.None);
        }

        private void OnEnable()
        {
            nameField.onEndEdit.AddListener(OnNameFieldUpdated);
        }

        private void OnDisable()
        {
            nameField.onEndEdit.RemoveListener(OnNameFieldUpdated);
        }

        public void Remove()
        {
            NotifySocketsDestroy();
            Destroy(gameObject);
        }

        private void NotifySocketsDestroy()
        {
            foreach (ISocket socket in _sockets)
            {
                socket.OnNodeDestroyed();
            }
        }

        private void OnDestroy()
        {
            try
            {
                ObjectContainer.NodeManager.UnregisterNode(this);
            }
            catch
            {
            }
        }
    }
}