using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Deynai.Markov;

public class GUIButtonHandler : MonoBehaviour
{
    [SerializeField] private Button newNodeButton;
    [SerializeField] private Button generateMatrixButton;

    private void OnEnable()
    {
        newNodeButton.onClick.AddListener(OnNewNode);
        generateMatrixButton.onClick.AddListener(OnGenerateMatrix);
    }

    private void OnDisable()
    {
        newNodeButton.onClick.RemoveListener(OnNewNode);
        generateMatrixButton.onClick.RemoveListener(OnGenerateMatrix);
    }

    private void OnNewNode()
    {
        Vector2 position = -ObjectContainer.CanvasService.GetWorldRootPos();
        ObjectContainer.NodeManager.CreateNewNode(position);
    }

    private void OnGenerateMatrix()
    {

    }
}
