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
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }
}
