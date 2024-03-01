using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootableObject", menuName = "Forgotten/LootableObject")]
public class LootManager : ScriptableObject
{

    public GameObject model;
    public int value;

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
                WeaponsManager weaponsManager = playerManager.firstWeaponObject.GetComponent<WeaponsManager>();
                
                if (weaponsManager.totalAmmo >= playerManager.maxFirstMunition) return;

                if (weaponsManager.totalAmmo + value > playerManager.maxFirstMunition) {
                    weaponsManager.totalAmmo = playerManager.maxFirstMunition;
                }
                else{
                    weaponsManager.totalAmmo += value;
                }
                Destroy(this);
            }
        }
        else if (model.CompareTag("Ammo2")) {
            if (other.gameObject.CompareTag("Player")) {
                PlayerManager playerManager = other.gameObject.GetComponentInParent<PlayerManager>();
                WeaponsManager weaponsManager = playerManager.secondWeaponObject.GetComponent<WeaponsManager>();
                
                if (weaponsManager.totalAmmo >= playerManager.maxFirstMunition) return;

                if (weaponsManager.totalAmmo + value > playerManager.maxFirstMunition) {
                    weaponsManager.totalAmmo = playerManager.maxFirstMunition;
                }
                else{
                    weaponsManager.totalAmmo += value;
                }
                Destroy(this);
            }
        }
    }
}
