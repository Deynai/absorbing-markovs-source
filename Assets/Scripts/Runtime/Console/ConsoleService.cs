using System.Collections.Generic;
using UnityEngine;

public class ConsoleService : MonoBehaviour
{
    [SerializeField] private GameObject consoleMessagePrefab;
    [SerializeField] private Transform consoleTransform;

    private List<GameObject> allConsoleMessageObjects = new List<GameObject>();

    public void Warning(string message)
    {
        GameObject newMessageObject = Instantiate(consoleMessagePrefab, consoleTransform);
        ConsoleMessage newMessage = newMessageObject.GetComponent<ConsoleMessage>();
        newMessage.SetMessage(message);
        allConsoleMessageObjects.Add(newMessageObject);
    }

    public void ClearConsole()
    {
        foreach (var messageObject in allConsoleMessageObjects)
        {
            Destroy(messageObject);
        }

        allConsoleMessageObjects.Clear();
    }
}
