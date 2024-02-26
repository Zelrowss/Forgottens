using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JsonManager : MonoBehaviour
{
    public static JsonManager instance;
    DataPlayerMove dt_PlayerMove = new DataPlayerMove();
    private void Awake() { if (instance == null) instance = this; }




    public void DataReceive(string pktName, string _contentJson)
    {
        switch (pktName)
        {
            case "NewPlayer":
                SpawnNewPlayer(_contentJson); break;

            case "MovePlayer":
                ReceiveData_PlayerMove(_contentJson); break;


            default:
                break;
        }
    }


    void SpawnNewPlayer(string _contentJson)
    {
        RoomClient data = JsonUtility.FromJson<RoomClient>(_contentJson);

        NetworkManager.instance.SpawnNewPlayer(data.netID, data.username, data.isHost);
    }


    void ReceiveData_PlayerMove(string _contentJson)
    {
        DataPlayerMove data = JsonUtility.FromJson<DataPlayerMove>(_contentJson);

        PlayerNetManager player = NetworkManager.instance.playersList.Find(p => p.id == data.clientID);

        if (player != null) player.MoveOtherPlayer(data.position, data.rotation);

    }

}



[System.Serializable] 
public class DataPlayerMove
{
    public int clientID;
    public Vector3 position;
    public Quaternion rotation;
}



[System.Serializable]
public class RoomClient
{
    public int netID;
    public string username;
    public bool isHost;
}


[System.Serializable]
public class DataSendRoomList
{
    public List<GameRoom> roomListToSend = new List<GameRoom>();
}







// Liste d'amis
[System.Serializable]
public class DataPlayerToSend
{
    public int netID;
    public string username;
    public GameRoom room;
    public bool isOnline;
}

[System.Serializable]
public class DataSendPlayerList
{
    public List<DataPlayerToSend> playerListToSend = new List<DataPlayerToSend>();
}
