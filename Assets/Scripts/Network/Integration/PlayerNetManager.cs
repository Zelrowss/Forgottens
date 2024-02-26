using System.Collections;
using UnityEngine;

public class PlayerNetManager : MonoBehaviour
{

    //      Variable Constante pour envoyer:
    //      0.016f = 60x/s(env)
    //      0.021f = 45x/s(env)
    //      0.033f = 30x/s(env)
    //      0.068f = 15x/s(env)
    const float requestInterval = 0.021f;

    DataPlayerMove syncMove = new DataPlayerMove();
    public Transform armature;
    Vector3 oldAndNewMove;
    Quaternion oldAndNewRot;

    public int id;
    public string username;
    public bool isHost;
    public bool isLocalPlayer;






    private void Start()
    {
        if (isLocalPlayer)
        {
            ClientSend.SendDataToServer("NewPlayer", "");
            StartCoroutine(SendLocalPlayerMove());
        }
    }


    private void Update()
    {
        if (isLocalPlayer) return;
        transform.position = Vector3.Lerp(transform.position, oldAndNewMove, Time.deltaTime*10);
        transform.rotation = Quaternion.Lerp(transform.rotation, oldAndNewRot, Time.deltaTime*10);
    }


    public void MoveOtherPlayer(Vector3 pos, Quaternion rot)
    {
        if (isLocalPlayer) return;

        oldAndNewMove = pos;
        oldAndNewRot = rot;
    }


    //Boucle qui envoi les déplacement du joueur
    private IEnumerator SendLocalPlayerMove()
    {
        while (true)
        {
            if(transform.position != oldAndNewMove || armature.rotation != oldAndNewRot)
                ClientSend.SendDataFlashToRoom("MovePlayer", PreparMoveToSend());

            yield return new WaitForSeconds(requestInterval);
        }
    }

    string PreparMoveToSend()
    {
        syncMove.clientID = id;

        syncMove.position = transform.position;
        syncMove.rotation = armature.rotation;

        return JsonUtility.ToJson(syncMove, false);
    }
}
