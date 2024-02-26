using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawnPlayer : MonoBehaviour
{

    private void Awake()
    {
        StartCoroutine(spawnPlayer());
    }

    //Boucle qui envoi les déplacement du joueur
    private IEnumerator spawnPlayer()
    {

        NetworkManager.instance.SpawnNewPlayer(Client.instance.myId, UIManager.instance.usernameField.text, false);


        yield return new WaitForSeconds(1);
    }
}
