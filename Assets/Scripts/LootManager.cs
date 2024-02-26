using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootableObject", menuName = "Forgotten/LootableObject")]
public class LootManager : ScriptableObject
{

    public GameObject model;
    public float value;

    public void OnTriggerEnter(Collider other) {
        if (model.CompareTag("HealthBall")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (playerManager.health >= playerManager.maxHealth) return;

                if (playerManager.health + value > playerManager.maxHealth) {
                    playerManager.health = playerManager.maxHealth;
                }
                else{
                    playerManager.health += value;
                }
                Destroy(this);
            }
        }
        else if (model.CompareTag("EnergyBall")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (playerManager.energy >= playerManager.maxEnergy) return;

                if (playerManager.energy + value > playerManager.maxEnergy) {
                    playerManager.energy = playerManager.maxEnergy;
                }
                else{
                    playerManager.energy += value;
                }
                Destroy(this);
            }
        }
        else if (model.CompareTag("Ammo1")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (playerManager.firstMunition >= playerManager.maxFirstMunition) return;

                if (playerManager.firstMunition + value > playerManager.maxFirstMunition) {
                    playerManager.firstMunition = playerManager.maxFirstMunition;
                }
                else{
                    playerManager.firstMunition += value;
                }
                Destroy(this);
            }
        }
        else if (model.CompareTag("Ammo2")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (playerManager.secondMunition >= playerManager.maxSecondMunition) return;

                if (playerManager.secondMunition + value > playerManager.maxSecondMunition) {
                    playerManager.secondMunition = playerManager.maxSecondMunition;
                }
                else{
                    playerManager.secondMunition += value;
                }
                Destroy(this);
            }
        }
        else if (model.CompareTag("Ammo3")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                
                if (playerManager.thirdMunition >= playerManager.maxThirdMunition) return;

                if (playerManager.thirdMunition + value > playerManager.maxThirdMunition) {
                    playerManager.thirdMunition = playerManager.maxThirdMunition;
                }
                else{
                    playerManager.thirdMunition += value;
                }
                Destroy(this);
            }
        }
    }
}
