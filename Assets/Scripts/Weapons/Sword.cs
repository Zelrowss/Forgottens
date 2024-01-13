using UnityEngine;

public class Sword : MonoBehaviour
{
    private void OnTriggerEnter(Collider others){
        if (others.gameObject.CompareTag("Enemy")){
            //Infliger des dégats
        }
    }
}
