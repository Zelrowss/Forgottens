using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  /!\ à répliquer exactement pareil sur le serveur /!\
[System.Serializable]
public class GameRoom : MonoBehaviour
{
    public List<RoomClient> roomClients = new List<RoomClient>();

    public int RoomID;
    public string roomName;
    public bool isPrivate;
}