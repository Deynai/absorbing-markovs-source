#nullable enable
using System.Collections.Generic;

namespace Deynai.Markov
{
    public interface ISocketVisualiser
    {
        public void OnSelectedOutgoingSocket(ISocket socket);
        public void OnCancelSelection();
        public void OnConnectSockets(ISocket start, ISocket end);
        public void OnSelectionMoved(ISocket selection);
        public void OnSocketMoved(ISocket socket);
        public void OnRemoveConnection(ISocket start, ISocket end);
        public void OnSocketRemoved(ISocket socket);
    }
}