using Deynai.Markov.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Deynai.Markov
{
    public class DeleteNode : MonoBehaviour, Controls.INodeWindowActions
    {
        private NodeSelectionManager _selectionManager;
        private Controls.NodeWindowActions _actions;
        private INode _node;

        private void Awake()
        {
            _selectionManager = ObjectContainer.SelectionManager;
            _node = GetComponent<INode>();

            _actions = new Controls().NodeWindow;
            _actions.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _actions.Enable();
        }

        private void OnDisable()
        {
            try
            {
                _actions.Disable();
            }
            catch 
            {
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
        }

        public void OnDelete(InputAction.CallbackContext context)
        {
            if (!_selectionManager.IsSelected(_node)) return;
            _node.Remove();
        }
    }
}
