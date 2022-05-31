using Deynai.Markov.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deynai.Markov
{
    public class DeleteConnection : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Connection connection;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Right) return;
            connection.Start.TryDisconnect(connection.End);
        }
    }
}
