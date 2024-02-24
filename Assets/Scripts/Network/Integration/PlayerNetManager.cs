using UnityEngine;

/// <summary>
/// #############################################################################
/// ################### Script à placer sur tous tes joueurs ####################
/// #############################################################################
/// </summary>
public class PlayerNetManager : MonoBehaviour
{
    public int id;
    public string username;
    public bool isHost;

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
    }
}
