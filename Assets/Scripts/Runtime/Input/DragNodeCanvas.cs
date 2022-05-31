using UnityEngine;
using UnityEngine.EventSystems;

public class DragNodeCanvas : MonoBehaviour, IDragHandler
{
    [SerializeField] private RectTransform _rootRect;
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = transform.root.GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Middle) { return; }
        _rootRect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
}
