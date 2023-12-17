using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using UnityEngine;
using Dobeil;
using System.Threading;

public class TCPServer : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient tcpClient;
    private Thread tcpListenerThread;
    private int port = 0;
    public Dictionary<int, ClientInfoInTCP> connectedList = new Dictionary<int, ClientInfoInTCP>();
    public void StartServer(int _port)
    {
        port = _port;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }
    private void ListenForIncommingRequests()
    {
        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Debug.Log("TCP Server started.");
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using (tcpClient = tcpListener.AcceptTcpClient())
                {
                    Debug.Log("New User Just Joined");
                    using (NetworkStream stream = tcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            string clientMessage = Encoding.ASCII.GetString(incommingData);
                            OnlineServer.Instance.ReciveMessageHandler(clientMessage,
                                -1,
                                (clientId) =>
                                {
                                    connectedList.Add(clientId, new ClientInfoInTCP(clientId, tcpClient, stream));
                                });
                        }
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.LogError("SocketException " + socketException.ToString());
        }
    }

    public void SendMessageToServer(int clientId, string message)
    {
        if (tcpClient != null && tcpClient.Connected && connectedList.ContainsKey(clientId))
        {
            Debug.Log($"Sending Message {message} to {clientId}");
            ClientInfoInTCP clientInfo = connectedList[clientId];
            message += '\n';
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            clientInfo.Stream.Write(messageBytes, 0, messageBytes.Length);
        }

    }

    void OnDestroy()
    {
        if (tcpListener != null)
        {
            tcpListener.Stop();
        }

        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }
}
