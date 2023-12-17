using System;
using System.Net.Sockets;

namespace Dobeil
{
    public enum SendMessageProtocol
    {
        UDP = 0,
        TCP = 1
    }

    public class ClientInfoInTCP
    {
        public int ClientId { get; }
        public TcpClient TcpClient { get; }
        public NetworkStream Stream { get; }
        public byte[] Buffer { get; }

        public ClientInfoInTCP(int clientId, TcpClient tcpClient, NetworkStream stream)
        {
            ClientId = clientId;
            TcpClient = tcpClient;
            Stream = stream;
            Buffer = new byte[1024];
        }
    }

    public class OnlineServerMessageClass
    {
        public string message;
        public int clientId;
        public Action<int> callBack;
        public OnlineServerMessageClass(string message, int clientId, Action<int> callBack)
        {
            this.message = message;
            this.clientId = clientId;
            this.callBack = callBack;
        }
    }

}