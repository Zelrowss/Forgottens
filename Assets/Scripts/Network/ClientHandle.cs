using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

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
    }

    public static void NewPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        //NetworkManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(NetworkManager.players[_id].gameObject);
        NetworkManager.players.Remove(_id);
    }

    public static void ServerSendJsonToClient(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonReceive.instance.DataReceive(_pktName, _JsonContent);
    }

    public static void ReturnHostJsonToClient(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonReceive.instance.DataReceive(_pktName, _JsonContent);
    }

    public static void ReturnClientJsonToHost(Packet _packet)
    {
        string _pktName = _packet.ReadString();
        string _JsonContent = _packet.ReadString();

        JsonReceive.instance.DataReceive(_pktName, _JsonContent);



    }
}