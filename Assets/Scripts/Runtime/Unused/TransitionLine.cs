using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deynai.Markov
{
    public class TransitionLine : MonoBehaviour
    {
        [SerializeField] private TMP_InputField weightField;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RawImage _lineImage;
        [SerializeField] private float _lineThickness;

        public ISocket Start { get; set; }
        public ISocket End { get; set; }
        public float Weight { get; set; }
        public RectTransform RectTransform { get; private set; }

        private RectTransform _weightFieldRectTransform;

        private void Awake()
        {
            RectTransform = _rectTransform;
            weightField.gameObject.SetActive(false);
            _weightFieldRectTransform = weightField.GetComponent<RectTransform>();
        }

        public void UpdateLine(Vector2 start, Vector2 end)
        {
            Vector2 delta = end - start;
            float angle = Mathf.Atan2(delta.y, delta.x);

            _rectTransform.sizeDelta = new Vector2(delta.magnitude, _lineThickness);
            _rectTransform.anchoredPosition = start + delta / 2f;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
            _weightFieldRectTransform.localRotation = Quaternion.Euler(0, 0, -Mathf.Rad2Deg * angle);
        }

        public void UpdateWeightField()
        {
            //_weightFieldRectTransform.anchoredPosition = _rectTransform.anchoredPosition;
        }

        public void SetActiveWeightField(bool val)
        {
            weightField.gameObject.SetActive(val);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}

