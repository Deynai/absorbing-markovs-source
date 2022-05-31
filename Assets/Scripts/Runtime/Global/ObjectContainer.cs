
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Deynai.Markov
{
    [DefaultExecutionOrder(-999)]
    public class ObjectContainer : MonoBehaviour
    {
        public static NodeManager NodeManager { get; private set; }
        public static CanvasService CanvasService { get; private set; }
        public static SocketManager SocketManager { get; private set; }
        public static NodeSelectionManager SelectionManager { get; private set; }
        public static ConsoleService ConsoleService { get; private set; }

        [SerializeField] private GameObject canvasServiceObject;

        [SerializeField] private GameObject socketVisualiserObject;

        [SerializeField] private GameObject consoleServiceObject;


        private void Awake()
        {
            ConstructServices();
        }

        private void ConstructServices()
        {
            CanvasService = canvasServiceObject.GetComponent<CanvasService>();

            NodeFactory factory = new NodeFactory();
            NodeManager = new NodeManager(factory);

            SelectionManager = new NodeSelectionManager();

            ISocketVisualiser socketVisualiser = socketVisualiserObject.GetComponent<ISocketVisualiser>();
            SocketManager = new SocketManager(socketVisualiser);

            ConsoleService = consoleServiceObject.GetComponent<ConsoleService>();
        }
    }
}