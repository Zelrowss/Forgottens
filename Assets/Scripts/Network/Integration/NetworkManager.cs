using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #############################################################################
/// ################## Script à placer sur ta scene principale ##################
/// #############################################################################
/// </summary>
public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    //public Dictionary<int, PlayerNetManager> playersList = new Dictionary<int, PlayerNetManager>();
    public List<PlayerNetManager> playersList = new List<PlayerNetManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }


    public void SpawnNewPlayer(int _PlayerID, string _Username, bool _IsHost)
    {
        PlayerNetManager player;

        if (_PlayerID == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab).GetComponent<PlayerNetManager>();
            player.isLocalPlayer = true;
            MissionController.instance.player = player.transform.gameObject;
        }
        else
            player = Instantiate(playerPrefab).GetComponent<PlayerNetManager>();

        player.id = _PlayerID;
        player.username = _Username;
        player.isHost = _IsHost;

        instance.playersList.Add(player);
    }

    public void RespawnPlayer()
    {

    }

}
