using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deynai.Markov
{
    public class SelectMultipleNodes : MonoBehaviour, IDragHandler, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject selectionBoxRectPrefab;

        private Vector2 _startMousePos;
        private Vector2 _endMousePos;
        private RectTransform _selectionBox;

        private NodeManager _nodeManager;
        private NodeSelectionManager _selectionManager;
        private CanvasService _canvasService;
        private Canvas _interactionCanvas;

        private void Awake()
        {
            _nodeManager = ObjectContainer.NodeManager;
            _canvasService = ObjectContainer.CanvasService;
            _selectionManager = ObjectContainer.SelectionManager;
            _interactionCanvas = GetComponentInParent<Canvas>();

            GameObject selectionBox = Instantiate(selectionBoxRectPrefab, this.transform);
            _selectionBox = selectionBox.GetComponent<RectTransform>();
            _selectionBox.gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            _startMousePos = eventData.position;
            _selectionBox.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            // draw box
            DrawBox(_startMousePos, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            _selectionBox.gameObject.SetActive(false);
            _endMousePos = eventData.position;

            PopulateSelectionList(_startMousePos, _endMousePos);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            _selectionManager.ClearSelection();
        }

        private void DrawBox(Vector2 start, Vector2 end)
        {
            Rect box = GetInteractionBox(start, end);

            _selectionBox.anchoredPosition = box.center;
            _selectionBox.sizeDelta = box.size;
        }

        private void PopulateSelectionList(Vector2 start, Vector2 end)
        {
            Rect box = GetInteractionBox(start, end);
            Rect nodeCanvasBox = _canvasService.InteractionToNodeCanvasRect(box);

            _selectionManager.ClearSelection();
            foreach (INode node in _nodeManager.GetAllNodes())
            {
                // RectTransform.rect is in local space so we need to create a fresh rect taking anchoredposition & size into account
                Rect nodeRect = _canvasService.GetNodeCanvasRectForRectTransform(node.RectTransform);
                if (CheckRectIntersect(nodeCanvasBox, nodeRect))
                {
                    _selectionManager.SelectNode(node);
                }
            }
        }

        private Rect GetInteractionBox(Vector2 start, Vector2 end)
        {
            float minX = Mathf.Min(start.x, end.x);
            float minY = Mathf.Min(start.y, end.y);
            float maxX = Mathf.Max(start.x, end.x);
            float maxY = Mathf.Max(start.y, end.y);

            Vector2 pos = new Vector2(minX, minY) - (_interactionCanvas.pixelRect.size / 2f);
            Vector2 size = new Vector2(maxX - minX, maxY - minY);

            return new Rect { position = pos, size = size };
        }

        private bool CheckRectIntersect(Rect a, Rect b)
        {
            if (a.max.x > b.min.x && a.max.y > b.min.y && a.min.x < b.max.x && a.min.y < b.max.y)
            {
                return true;
            }
            return false;
        }
    }
}
