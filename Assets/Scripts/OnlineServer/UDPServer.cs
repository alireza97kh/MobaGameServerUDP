using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPServer : MonoBehaviour
{
    private UdpClient udpServer;
    private IPEndPoint clientEndPoint;
    private Dictionary<IPEndPoint, int> clientEndpoints = new Dictionary<IPEndPoint, int>();
    private Dictionary<int, DateTime> clientLastActiveTimes = new Dictionary<int, DateTime>();
    private readonly TimeSpan clientTimeout = TimeSpan.FromSeconds(30); // Adjust the timeout as needed
    private Coroutine checkUsers;

    public void StartServer(int port)
    {
        udpServer = new UdpClient(port);
        clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

        Debug.Log("UDP Server started.");

        udpServer.BeginReceive(ReceiveCallback, null);

        checkUsers = StartCoroutine(CheckClientTimeouts());
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0); 
        byte[] receivedBytes = udpServer.EndReceive(result, ref clientEndPoint);
        string message = Encoding.ASCII.GetString(receivedBytes);
        int clientId;
        if (!clientEndpoints.TryGetValue(clientEndPoint, out clientId))
        {
            clientId = clientEndpoints.Count + 1;
            clientEndpoints.Add(clientEndPoint, clientId);
        }
        else
            clientId = clientEndpoints[clientEndPoint];
        clientLastActiveTimes[clientId] = DateTime.Now;
        OnlineServer.Instance.ReciveMessageHandler(message, clientId);
        Debug.Log("Received message: " + message);

        // Handle the received message here



        // Continue listening for more messages
        udpServer.BeginReceive(ReceiveCallback, null);
    }

    public void SendMessageToServere(int clientId, string message)
    {
        IPEndPoint clientEndPoint;
        if (clientEndpoints.ContainsValue(clientId))
        {
            clientEndPoint = clientEndpoints.First(x => x.Value == clientId).Key;
            message += '\n';
            byte[] response = Encoding.ASCII.GetBytes(message);
            udpServer.Send(response, response.Length, clientEndPoint);
        }
        else
        {
            Debug.LogError($"Client with ID {clientId} not found.");
        }

    }
    private void Update()
    {
        CheckClientTimeouts();
    }
    private IEnumerator CheckClientTimeouts()
    {
        while (true)
        {
            DateTime currentTime = DateTime.Now;
            foreach (var kvp in clientLastActiveTimes.ToArray())
            {
                if (currentTime - kvp.Value > clientTimeout)
                {
                    int clientId = kvp.Key;
                    IPEndPoint clientEndPointToRemove = clientEndpoints.FirstOrDefault(x => x.Value == clientId).Key;

                    Debug.Log($"Client {clientId} at {clientEndPointToRemove} timed out and will be removed.");

                    // Remove the client from the dictionaries
                    clientEndpoints.Remove(clientEndPointToRemove);
                    clientLastActiveTimes.Remove(clientId);
                }
            }
            yield return new WaitForSeconds(1);
        }
        
    }
    void OnDestroy()
    {
        if (udpServer != null)
            udpServer.Close();

        StopCoroutine(checkUsers);
    }

}
