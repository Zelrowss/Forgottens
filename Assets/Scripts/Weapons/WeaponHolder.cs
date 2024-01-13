using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponHolder : MonoBehaviour
{

    [Header("References")]
    private PlayerManager _playerManager;

    [Header("Objects")]
    public GameObject currentWeaponPos;
    public GameObject raycastDest;
    public GameObject firstWeaponPos;
    public GameObject secondWeaponPos;
    public TwoBoneIKConstraint rightHand;
    public TwoBoneIKConstraint leftHand;

    void Awake(){
        _playerManager = GetComponentInParent<PlayerManager>();
    }
    

    void Update(){

        rightHand.data.target = _playerManager.currentWeapon.GetComponent<WeaponsManager>().rightHandPos;
        leftHand.data.target = _playerManager.currentWeapon.GetComponent<WeaponsManager>().leftHandPos;
        
        if (_playerManager.currentWeaponsPlace == Weapons.Primary){
            currentWeaponPos = firstWeaponPos;         
        }
        else if (_playerManager.currentWeaponsPlace == Weapons.Secondary){
            currentWeaponPos = secondWeaponPos;
        }
        else if (_playerManager.currentWeaponsPlace == Weapons.Melee){
            currentWeaponPos = null;
        }
    }
}
