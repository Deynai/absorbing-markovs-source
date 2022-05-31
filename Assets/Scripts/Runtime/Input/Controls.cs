// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Runtime/Input/Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Deynai.Markov.Input
{
    public class @Controls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Controls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""NodeWindow"",
            ""id"": ""8b6e2520-1da6-425b-b774-5ffd79c244f5"",
            ""actions"": [
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""b0727567-2fe4-4fff-9f75-ae8a372cb255"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Delete"",
                    ""type"": ""Button"",
                    ""id"": ""99768223-e84b-41aa-a334-afd27134a635"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""efabc09a-a964-4334-a75d-4c6c65b4927b"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Nodes"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8c53244e-fd73-452b-af31-5261b9470b94"",
                    ""path"": ""<Keyboard>/delete"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Nodes"",
                    ""action"": ""Delete"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Nodes"",
            ""bindingGroup"": ""Nodes"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // NodeWindow
            m_NodeWindow = asset.FindActionMap("NodeWindow", throwIfNotFound: true);
            m_NodeWindow_Zoom = m_NodeWindow.FindAction("Zoom", throwIfNotFound: true);
            m_NodeWindow_Delete = m_NodeWindow.FindAction("Delete", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // NodeWindow
        private readonly InputActionMap m_NodeWindow;
        private INodeWindowActions m_NodeWindowActionsCallbackInterface;
        private readonly InputAction m_NodeWindow_Zoom;
        private readonly InputAction m_NodeWindow_Delete;
        public struct NodeWindowActions
        {
            private @Controls m_Wrapper;
            public NodeWindowActions(@Controls wrapper) { m_Wrapper = wrapper; }
            public InputAction @Zoom => m_Wrapper.m_NodeWindow_Zoom;
            public InputAction @Delete => m_Wrapper.m_NodeWindow_Delete;
            public InputActionMap Get() { return m_Wrapper.m_NodeWindow; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(NodeWindowActions set) { return set.Get(); }
            public void SetCallbacks(INodeWindowActions instance)
            {
                if (m_Wrapper.m_NodeWindowActionsCallbackInterface != null)
                {
                    @Zoom.started -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnZoom;
                    @Zoom.performed -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnZoom;
                    @Zoom.canceled -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnZoom;
                    @Delete.started -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnDelete;
                    @Delete.performed -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnDelete;
                    @Delete.canceled -= m_Wrapper.m_NodeWindowActionsCallbackInterface.OnDelete;
                }
                m_Wrapper.m_NodeWindowActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Zoom.started += instance.OnZoom;
                    @Zoom.performed += instance.OnZoom;
                    @Zoom.canceled += instance.OnZoom;
                    @Delete.started += instance.OnDelete;
                    @Delete.performed += instance.OnDelete;
                    @Delete.canceled += instance.OnDelete;
                }
            }
        }
        public NodeWindowActions @NodeWindow => new NodeWindowActions(this);
        private int m_NodesSchemeIndex = -1;
        public InputControlScheme NodesScheme
        {
            get
            {
                if (m_NodesSchemeIndex == -1) m_NodesSchemeIndex = asset.FindControlSchemeIndex("Nodes");
                return asset.controlSchemes[m_NodesSchemeIndex];
            }
        }
        public interface INodeWindowActions
        {
            void OnZoom(InputAction.CallbackContext context);
            void OnDelete(InputAction.CallbackContext context);
        }
    }
}
