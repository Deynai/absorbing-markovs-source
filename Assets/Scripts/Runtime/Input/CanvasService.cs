using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deynai.Markov.Input;

public class CanvasService : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform worldRootRect;
    [SerializeField] private GameObject debugBoxObject;
    [SerializeField] private ZoomNodeCanvas zoomNodeCanvas;

    public Canvas Canvas => canvas;

    public Vector2 MouseToNodeCanvasPosition(Vector2 mousePos)
    {
        Vector2 canvasSize = canvas.pixelRect.size / canvas.scaleFactor;
        Vector2 mouseCanvasPos = mousePos / canvas.scaleFactor - canvasSize / 2f;
        return mouseCanvasPos - worldRootRect.anchoredPosition;
    }

    public Vector2 NodeCanvasToMousePosition(Vector2 point)
    {
        Vector2 canvasSize = canvas.pixelRect.size / canvas.scaleFactor;
        Vector2 mouseCanvasPos = point + worldRootRect.anchoredPosition;
        return mouseCanvasPos + (canvasSize / 2f) * canvas.scaleFactor;
    }

    public Vector2 InteractionToNodeCanvasPosition(Vector2 point)
    {
        Vector2 scaledPoint = point / canvas.scaleFactor;
        return scaledPoint - worldRootRect.anchoredPosition;
    }

    public Vector2 NodeCanvasToInteractionPosition(Vector2 point)
    {
        Vector2 offsetPoint = point + worldRootRect.anchoredPosition;
        return offsetPoint * canvas.scaleFactor;
    }

    public Rect InteractionToNodeCanvasRect(Rect rect)
    {
        Vector2 newMin = InteractionToNodeCanvasPosition(rect.min);
        Vector2 newMax = InteractionToNodeCanvasPosition(rect.max);

        return new Rect { position = newMin, size = newMax - newMin };
    }

    public Rect NodeCanvasToInteractionRect(Rect rect)
    {
        Vector2 newMin = NodeCanvasToInteractionPosition(rect.min);
        Vector2 newMax = NodeCanvasToInteractionPosition(rect.max);

        return new Rect { position = newMin, size = newMax - newMin };
    }

    public Rect GetNodeCanvasRectForRectTransform(RectTransform rectTransform)
    {
        Vector2 center = rectTransform.anchoredPosition;
        Vector2 size = rectTransform.sizeDelta;

        Vector2 min = center - (size / 2f);
        return new Rect { position = min, size = size };
    }

    public Vector2 GetWorldRootPos()
    {
        return worldRootRect.anchoredPosition;
    }

    public GameObject GetWorldRootObject()
    {
        return worldRootRect.gameObject;
    }

    public void DebugDrawRect(Rect rect)
    {
        StartCoroutine(DrawRectCoroutine(rect));
    }

    public void SetZoomLock(bool state)
    {
        zoomNodeCanvas.SetLock(state);
    }

    private IEnumerator DrawRectCoroutine(Rect rect)
    {
        GameObject box = Instantiate(debugBoxObject, worldRootRect.transform);
        RectTransform rectTransform = box.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = rect.center;
        rectTransform.sizeDelta = rect.size;

        yield return new WaitForSeconds(5f);

        Destroy(box);
    }
}