using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Deynai.Markov
{
    public class FinalStateProbabilityGUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown startingStateDropdown;
        [SerializeField] private GameObject stateElementPrefab;
        [SerializeField] private GameObject stateElementParent;
        [SerializeField] private TextMeshProUGUI expectedStepsTMP;

        [SerializeField] private RectTransform contentTransform;
        [SerializeField] private float baseContentHeight;
        [SerializeField] private float baseContentWidth;

        private List<StateElementUI> _currentStateElements = new List<StateElementUI>();

        private List<StateProbability> _currentFSPList;
        private double _currentExpectedSteps;

        private FinalStateProbability _fsp;

        public void Initialise(GraphParser parser)
        {
            _fsp = new FinalStateProbability(parser);
            if (!_fsp.Valid) return;

            InitialiseStartingStateDropdown();

            UpdateData(0);
            RefreshStateElements();
        }

        private void InitialiseStartingStateDropdown()
        {
            startingStateDropdown.ClearOptions();
            List<string> options = new List<string>();

            for (int i = 0; i < _fsp.TransientCount; i++)
            {
                options.Add(_fsp.GetNameForIndex(i));
            }

            startingStateDropdown.AddOptions(options);
        }

        private void InstantiateStateElements()
        {
            if (_currentStateElements.Count == _currentFSPList.Count) return;

            if (_currentStateElements.Count < _currentFSPList.Count)
            {
                int diff = _currentFSPList.Count - _currentStateElements.Count;
                for (int i = 0; i < diff; i++)
                {
                    GameObject newStateElement = Instantiate(stateElementPrefab, stateElementParent.transform);
                    StateElementUI newStateElementUI = newStateElement.GetComponent<StateElementUI>();
                    _currentStateElements.Add(newStateElementUI);
                }
            }
            else if (_currentStateElements.Count > _currentFSPList.Count)
            {
                for (int i = _currentStateElements.Count - 1; i >= _currentFSPList.Count; i--)
                {
                    Destroy(_currentStateElements[i].gameObject);
                }
                _currentStateElements = _currentStateElements.GetRange(0, _currentFSPList.Count);
            }
        }

        private void RefreshStateElements()
        {
            if (_currentFSPList == null) return;

            if (_currentStateElements.Count != _currentFSPList.Count)
                InstantiateStateElements();

            for (int i = 0; i < _currentFSPList.Count; i++)
            {
                _currentStateElements[i].SetName(_currentFSPList[i].StateName);
                _currentStateElements[i].SetProbability(_currentFSPList[i].Probability.ToString("0.####"));
                _currentStateElements[i].gameObject.SetActive(true);
            }

            expectedStepsTMP.text = _currentExpectedSteps.ToString("0.####");
            RefreshContentSize();
        }

        private void RefreshContentSize()
        {
            float contentHeight = baseContentHeight + _currentStateElements.Count * stateElementPrefab.GetComponent<RectTransform>().sizeDelta.y;
            contentTransform.sizeDelta = new Vector2(baseContentWidth, contentHeight);
        }
        private void UpdateData(int index)
        {
            UpdateFSPList(index);
            UpdateExpectedSteps(index);
        }

        private void UpdateFSPList(int index)
        {
            if (_fsp.AbsorbingCount == 0) return;
            _currentFSPList = _fsp.GetStateProbabilities(index);
        }

        private void UpdateExpectedSteps(int index)
        {
            if (_fsp.TransientCount == 0)
            {
                _currentExpectedSteps = 0;
                return;
            }

            _currentExpectedSteps = _fsp.GetStateExpectedSteps(index);
        }

        public void OnSelectedNewStartingState()
        {
            if (!_fsp.Valid) return;
            UpdateData(startingStateDropdown.value);
            RefreshStateElements();
        }
    }
}
