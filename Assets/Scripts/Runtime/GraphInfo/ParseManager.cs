using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    public class ParseManager : MonoBehaviour
    {
        [SerializeField] private GameObject graphInfoGUI;
        [SerializeField] private FinalStateProbabilityGUI finalStateGUI;

        public static List<GraphParser> _parsers = new List<GraphParser>();

        public static GraphParser GenerateTransitionMatrix()
        {
            GraphParser parser = new GraphParser();
            _parsers.Add(parser);
            parser.GenerateTransitionMatrix();
            return parser;
        }

        public void OnGenerateMatrix()
        {
            if (graphInfoGUI.activeSelf)
            {
                graphInfoGUI.SetActive(false);
                ObjectContainer.CanvasService.SetZoomLock(false);
                return;
            }

            GraphParser parser = GenerateTransitionMatrix();

            if (parser.Valid)
            {
                graphInfoGUI.SetActive(true);
                finalStateGUI.gameObject.SetActive(true);
                finalStateGUI.Initialise(parser);
                ObjectContainer.CanvasService.SetZoomLock(true);
            }
        }
    }
}
