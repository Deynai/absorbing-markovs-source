using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Deynai.Markov
{
    public class StateElementUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateNameTMP;
        [SerializeField] private TextMeshProUGUI probabilityTMP;
        [SerializeField] private TextMeshProUGUI expectedStepsTMP;

        public void SetName(string name)
        {
            stateNameTMP.text = name;
        }

        public void SetProbability(string probability)
        {
            probabilityTMP.text = probability;
        }

        public void SetExpectedSteps(string expectedSteps)
        {
            expectedStepsTMP.text = expectedSteps;
        }
    }
}
