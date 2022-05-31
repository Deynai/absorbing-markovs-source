using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Deynai.Markov
{
    public class DragWindow : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField] private Image _highlightBorder;

        private Canvas _canvas;
        private RectTransform _rectTransform;

        private INode _node;

        private NodeSelectionManager _selectionManager;

        private List<ISocket> _relatedSockets;

        private void Awake()
        {
            _node = GetComponent<INode>();
            _selectionManager = ObjectContainer.SelectionManager;

            _canvas = transform.root.GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_selectionManager.IsSelected(_node))
            {
                foreach (var node in _selectionManager.selectedNodes)
                {
                    node.RectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
                }
            }
            else
            {
                _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
            }

            if (_relatedSockets == null)
                OnPointerDown(eventData);

            foreach (var socket in _relatedSockets)
            {
                socket.OnMoved(eventData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_selectionManager.IsSelected(_node)) { return; }
            _node.HoverEntered();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_selectionManager.IsSelected(_node)) { return; }
            _node.HoverExited();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_selectionManager.IsSelected(_node))
            {
                _relatedSockets = GetAllSockets(_node);
                _selectionManager.ClearSelection();
                _selectionManager.SelectNode(_node);
            }
            else
            {
                _relatedSockets = GetAllSockets(_selectionManager.selectedNodes);
            }

            _rectTransform.SetAsLastSibling();
        }

        public List<ISocket> GetAllSockets(List<INode> nodes)
        {
            List<ISocket> sockets = new List<ISocket>();
            foreach (var node in nodes)
            {
                sockets.AddRange(node.Sockets);
            }
            return sockets;
        }

        public List<ISocket> GetAllSockets(INode node)
        {
            List<ISocket> sockets = new List<ISocket>();
            sockets.AddRange(node.Sockets);
            return sockets;
        }
    }
}