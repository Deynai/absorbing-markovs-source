#nullable enable

using System.Collections.Generic;

namespace Deynai.Markov
{
    public class SocketManager
    {
        private enum State
        {
            none,
            selected,
        }

        private ISocket? _selectedSocket;
        private State _state;

        private ISocketVisualiser _visualiser;
        public SocketManager(ISocketVisualiser visualiser)
        {
            _visualiser = visualiser;
        }

        public void OnClickedSocket(ISocket socket)
        {
            if (socket.Outgoing) { ClickedOutgoingSocket(socket); }
            else { ClickedIncomingSocket(socket); }
        }

        public void OnSocketMoved(ISocket socket)
        {
            _visualiser.OnSocketMoved(socket);
        }

        public void OnSelectionMoved(ISocket socket)
        {
            _visualiser.OnSelectionMoved(socket);
        }

        public void OnCancelSelection()
        {
            Deselect();
        }

        private void ClickedOutgoingSocket(ISocket socket)
        {
            if (_state == State.selected) 
            { 
                Deselect();
            }

            _selectedSocket = socket;
            _state = State.selected;

            _visualiser.OnSelectedOutgoingSocket(socket);
        }

        private void ClickedIncomingSocket(ISocket socket)
        {
            if (_state == State.none) { return; }

            if (_selectedSocket != null)
            {
                Connect(socket);
                _selectedSocket = null;
                _state = State.none;
                Deselect();
            }
        }

        private void Connect(ISocket socket)
        {
            if (_selectedSocket == null) return;
            if (_selectedSocket.TryConnect(socket))
            {
                _visualiser.OnConnectSockets(_selectedSocket, socket);
            }
        }

        public void OnSocketRemoved(ISocket socket)
        {
            Deselect();
            _visualiser.OnSocketRemoved(socket);
        }

        public void OnRemoveConnection(ISocket start, ISocket end)
        {
            Deselect();
            _visualiser.OnRemoveConnection(start, end);
        }

        private void Deselect()
        {
            _selectedSocket = null;
            _state = State.none;

            _visualiser.OnCancelSelection();
        }
    }
}