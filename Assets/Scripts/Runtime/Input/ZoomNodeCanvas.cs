using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Deynai.Markov.Input
{
    public class ZoomNodeCanvas : MonoBehaviour, Controls.INodeWindowActions
    {
        private enum Zoom
        {
            In,
            Out
        }

        [SerializeField] private RectTransform panelRectTransform;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private float scrollWeight = 0.1f;
        private Canvas _canvas;

        private Controls.NodeWindowActions _actions;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();

            _actions = new Controls().NodeWindow;
            _actions.SetCallbacks(this);
        }

        public void SetLock(bool state)
        {
            if (state)
                _actions.Disable();
            else
                _actions.Enable();
        }

        private void OnEnable()
        {
            _actions.Enable();
        }

        private void OnDisable()
        {
            try
            {
                _actions.Disable();
            }
            catch { }
        }
        public void OnZoom(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (value > 0)
            {
                ZoomCanvas(mousePosition, Zoom.In);
            }
            else if (value < 0)
            {
                ZoomCanvas(mousePosition, Zoom.Out);
            }
        }

        private void ZoomCanvas(Vector2 mousePos, Zoom zoomType)
        {
            Vector2 localRectCenter = panelRectTransform.anchoredPosition;
            Vector2 startWorldMousePosition = MouseToWorldPosition(mousePos, localRectCenter, _canvas);

            _canvas.scaleFactor = zoomType == Zoom.In ?
                _canvas.scaleFactor * (1 + scrollWeight)
                : _canvas.scaleFactor / (1 + scrollWeight);

            Vector2 endWorldMousePosition = MouseToWorldPosition(mousePos, localRectCenter, _canvas);

            panelRectTransform.anchoredPosition += endWorldMousePosition - startWorldMousePosition;
        }

        private Vector2 MouseToWorldPosition(Vector2 mousePosition, Vector2 currentRectCenter, Canvas canvas)
        {
            Vector2 canvasSize = canvas.pixelRect.size / canvas.scaleFactor;
            Vector2 canvasClickPosition = mousePosition / canvas.scaleFactor;
            Vector2 centeredClickPosition = canvasClickPosition - (canvasSize / 2);
            Vector2 worldClickPosition = currentRectCenter + centeredClickPosition;

            return worldClickPosition;
        }

        public void OnDelete(InputAction.CallbackContext context)
        {
        }
    }
}
