using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        SceneManager.LoadScene("ExterminationNet", LoadSceneMode.Additive);
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        //Destroy(NetworkManager.players[_id].gameObject);
        //NetworkManager.players.Remove(_id);
    }

    public static void ServerSendJsonToClient(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonManager.instance.DataReceive(_pktName, _JsonContent);
    }

    public static void ReturnHostJsonToClient(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonManager.instance.DataReceive(_pktName, _JsonContent);
    }

    public static void ReturnClientJsonToHost(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonManager.instance.DataReceive(_pktName, _JsonContent);
    }

    public static void ReceiveFlashInfo(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonManager.instance.DataReceive(_pktName, _JsonContent);
    }
}