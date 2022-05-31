using TMPro;
using UnityEngine;

namespace Deynai.Markov
{
    public class Connection : MonoBehaviour
    {
        [SerializeField] private TMP_InputField weightField;
        [SerializeField] private RectTransform weightFieldRectTransform;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private BezierController bezierController;
        [SerializeField] private float lineThickness;

        public ISocket Start { get; set; }
        public ISocket End { get; set; }

        //public float Weight { get; set; }
        public RectTransform RectTransform { get; private set; }

        //private RectTransform _weightFieldRectTransform;

        private void Awake()
        {
            bezierController.SetThickness(lineThickness);
            RectTransform = rectTransform;
        }

        public void SetSameNodeOffsets()
        {
            bezierController.SetM1Offset(new Vector2(400, -300));
            bezierController.SetM2Offset(new Vector2(-400, -300));
        }

        public void SetOutLeftOffsets()
        {
            bezierController.SetM1Offset(new Vector2(400, -300));
            bezierController.SetM2Offset(new Vector2(-400, -300));
        }

        public void SetOutRightOffsets()
        {
            bezierController.SetM1Offset(new Vector2(400, 50));
            bezierController.SetM2Offset(new Vector2(-400, 50));
        }

        public void UpdateConnection(Vector2 start, Vector2 end)
        {
            bezierController.UpdateBezier(start, end);
            weightFieldRectTransform.anchoredPosition = bezierController.EvaluateBezierDistance(0.4f);
        }

        public void UpdateWeight(string input)
        {
            if (!float.TryParse(input, out float weight))
            {
                weightField.text = "0";
            }
            else
            {
                Start.Connections[End] = new ConnectionInfo(weight);
            }
        }

        public void OnEnable()
        {
            weightField.onEndEdit.AddListener(UpdateWeight);
        }

        public void OnDisable()
        {
            weightField.onEndEdit.RemoveListener(UpdateWeight);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}