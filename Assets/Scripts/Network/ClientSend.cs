using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    #region Structure
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }
    #endregion

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }


    public static void SendDataToServer(string _pktName, string _contentJSON)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendJsonToServer))
        {
            _packet.Write(_pktName);
            _packet.Write(_contentJSON);

            SendTCPData(_packet);
        }
    }
    public static void SendDataToHost(string _pktName, string _contentJSON)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendJsonToHost))
        {
            _packet.Write(_pktName);
            _packet.Write(_contentJSON);

            SendUDPData(_packet);
        }
    }
    public static void SendDataToClient(string _pktName, string _contentJSON)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendJsonToClients))
        {
            _packet.Write(_pktName);
            _packet.Write(_contentJSON);

            SendUDPData(_packet);
        }
    }


    public static void SendDataFlashToRoom(/*GameRoom room,*/ string _pktName, string _contentJSON)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendFlashInfo))
        {
            _packet.Write(_pktName);
            _packet.Write(_contentJSON);


            SendUDPData(_packet);
        }
    }


    #endregion
}
