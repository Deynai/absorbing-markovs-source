using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deynai.Markov
{
    public class CancelSocketConnection : MonoBehaviour, IPointerDownHandler
    {
        private SocketManager _socketManager;

        private void Awake()
        {
            _socketManager = ObjectContainer.SocketManager;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _socketManager.OnCancelSelection();
        }
    }
}
