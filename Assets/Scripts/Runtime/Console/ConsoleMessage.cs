using TMPro;
using UnityEngine;

public class ConsoleMessage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageTMP;

    public void SetMessage(string message)
    {
        messageTMP.text = message;
    }
}
