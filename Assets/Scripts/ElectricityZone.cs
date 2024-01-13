using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricityZone : MonoBehaviour
{
    public List<GameObject> enemyInZone = new List<GameObject>();
    public int limitedEnemy;

    void OnTriggerEnter(Collider other){
        if (other.CompareTag("Enemy") && enemyInZone.Count <= limitedEnemy){
            enemyInZone.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.CompareTag("Enemy") && enemyInZone.Count <= limitedEnemy){
            enemyInZone.Remove(other.gameObject);
        }
    }

    void Update(){

        if (enemyInZone.Count > 0){
            for (int i = 0; i < enemyInZone.Count; i++){
                if (enemyInZone[i].gameObject == null) enemyInZone.Remove(enemyInZone[i]);
            }
        }
        
    }
}
