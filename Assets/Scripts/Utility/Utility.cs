using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static string[] DeCodeMessage(string message)
    {
        return message.Split(':');
    }
    public static string EnCodeMessage(string[] message)
    {
        return string.Join(":", message);
    }

    public static string EnCodeMessage(List<string> message)
    {
        return string.Join(":", message);
    }
}
