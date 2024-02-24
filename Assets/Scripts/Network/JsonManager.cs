using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #############################################################################
/// ################## Script à placer sur ta scene principale ##################
/// #############################################################################
/// 
/// 
/// #############################################################################
/// #################### Ou se passe tout tes envois en JSON ####################
/// #############################################################################
/// </summary>

public class JsonSend : MonoBehaviour
{
    public static JsonSend instance;
    private void Awake() { if (instance == null) instance = this; }


    public void SendData_PlayerMove(Vector3 _playerPos, Quaternion _playerRotation, bool _sendToHost)
    {
        // Copie du patherne DataPlayerMove
        DataPlayerMove dt_PlayerMove = new DataPlayerMove();

        // Renseignement des valeurs
        dt_PlayerMove.clientID = Client.instance.myId;
        dt_PlayerMove.position = _playerPos;
        dt_PlayerMove.rotation = _playerRotation;

        // Compilation en JSON
        string contentJSON = JsonUtility.ToJson(dt_PlayerMove, false);

        // Envoi des données
        if (_sendToHost) ClientSend.SendDataToHost("dataMove", contentJSON);
        else ClientSend.SendDataToClient("dataMove", contentJSON);
    }



    public void ExempleenvoiServer()
    {
        string contentJSON = "JsonUtility...";
        ClientSend.SendDataToServer("dataMove", contentJSON);
    }
}


/// <summary>
/// #############################################################################
/// ##################### Tes réceptions en JSON du serveur #####################
/// #############################################################################
/// </summary>

public class JsonReceive : MonoBehaviour
{
    public static JsonReceive instance;
    private void Awake() { if (instance == null) instance = this; }

    public void DataReceive(string pktName, string _contentJson)
    {
        switch (pktName)
        {
            case "dataMove":
                ReceiveData_PlayerMove(_contentJson); break;

            case "dataKlaxon":
                ReceiveData_PlayerKlaxon(_contentJson); break;

            default:
                break;
        }
    }



    void ReceiveData_PlayerMove(string _contentJson)
    {
        DataPlayerMove data = JsonUtility.FromJson<DataPlayerMove>(_contentJson);


        //Exemple de récuperation de tes variables, apres tu met un Vector3.Lerp dessus ça permet de lisser le déplacement.
        //GameManager.instance.MovePlayer(data.clientID, data.position, data.rotation);
    }

    void ReceiveData_PlayerKlaxon(string _contentJson)
    {

        // c'est un exemple pour le switch
    }
}


/// <summary>
/// #############################################################################
/// #################### Tes structures de compilations JSON ####################
/// #############################################################################
/// </summary>

[System.Serializable] 
public class DataPlayerMove //Exemple de Patherne à envoyer
{
    public int clientID;
    public Vector3 position;
    public Quaternion rotation;
}

public enum JsonData
{
    none,
    movePlayer,
    klaxon,
    disconnectedPlayer,
    newPlayer,
    finishPlayer
}



//  /!\ à répliquer exactement pareil sur le client /!\

//Structures JSON

// ################## ROOMS ##################
//Liste des room en ligne
[System.Serializable]
public class DataSendRoomList
{
    public List<GameRoom> roomListToSend = new List<GameRoom>();
}

//Paterne de client dans la room
[System.Serializable]
public class RoomClient
{
    public int netID;
    public string username;
    public bool isHost;

    // Ajoute tes infos de joueurs
}


// ################## Amis ##################
// Paterne d'ami
[System.Serializable]
public class DataPlayerToSend
{
    public int netID;
    public string username;
    public GameRoom room;
    public bool isOnline;
}

// Récuperation des amis de la personne
[System.Serializable]
public class DataSendPlayerList
{
    public List<DataPlayerToSend> playerListToSend = new List<DataPlayerToSend>();
}


// ################## Crew ##################
// Paterne d'un crew
[System.Serializable]
public class DataCrewToSend
{
    public int crewID;
    public string crewName;
    public string crewTAG;
    public int crewLVL;
    public int crewMembers;
}

// Paterne de listing des crew
[System.Serializable]
public class DataSendCrewList
{
    public List<DataCrewToSend> playerListToSend = new List<DataCrewToSend>();
}
