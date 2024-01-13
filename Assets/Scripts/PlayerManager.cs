using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapons{
    Primary,
    Secondary,
    Melee,
}

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public GameObject _weaponHolder;
    public GameObject _firstWeapon;
    public GameObject _secondWeapon;
    public GameObject _meleeWeapon;

    [Header("Player")]
    public bool isInMission = false;
    [HideInInspector] public bool weaponChanging;
    public float health, shield;
    
    [Header("Weapons")]
    public Weapons currentWeaponsPlace;
    public GameObject currentWeapon;
    [HideInInspector] public GameObject firstWeaponObject;
    [HideInInspector] public GameObject secondWeaponObject;
    [HideInInspector] public GameObject meleeWeaponObject;

    // Start is called before the first frame update
    void Start()
    {

        if (_firstWeapon != null){
            firstWeaponObject = Instantiate(_firstWeapon, _weaponHolder.transform);
            ChangeWeaponsClipping(firstWeaponObject, 0, false);

            if (_secondWeapon != null){
                secondWeaponObject = Instantiate(_secondWeapon, _weaponHolder.transform);
                ChangeWeaponsClipping(secondWeaponObject, 1, false);
            }
            if (_meleeWeapon != null) {
                meleeWeaponObject = Instantiate(_meleeWeapon, _weaponHolder.transform);
                ChangeWeaponsClipping(meleeWeaponObject, 1, false);
            }

            currentWeapon = firstWeaponObject;
            currentWeaponsPlace = Weapons.Primary;

            ChangeWeapons(true, false, false);
        }
        else if (_secondWeapon != null){
            secondWeaponObject = Instantiate(_secondWeapon, _weaponHolder.transform);
            ChangeWeaponsClipping(secondWeaponObject, 0, false);
            if (meleeWeaponObject != null){
                meleeWeaponObject = Instantiate(_meleeWeapon, _weaponHolder.transform);
                ChangeWeaponsClipping(meleeWeaponObject, 1, false);
            }

            currentWeapon = secondWeaponObject;
            currentWeaponsPlace = Weapons.Secondary;

            ChangeWeapons(false, true, false);
        }
        else if (_meleeWeapon != null){
            meleeWeaponObject = Instantiate(_meleeWeapon, _weaponHolder.transform);
            ChangeWeaponsClipping(meleeWeaponObject, 0, false);

            currentWeapon = meleeWeaponObject;
            currentWeaponsPlace = Weapons.Melee;

            ChangeWeapons(false, false, true);
        }

        StartCoroutine(ReBuildRig());
    }

    public IEnumerator ReBuildRig(){
        yield return new WaitForSeconds(0.001f);
        GetComponentInChildren<UnityEngine.Animations.Rigging.RigBuilder>().Build();
        yield return new WaitForSeconds(0.5f);
        weaponChanging = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (weaponChanging){
            ChangeWeaponsClipping(currentWeapon, 1, true);
            StartCoroutine(WaitForNewWeapon());
        }
            
    }

    IEnumerator WaitForNewWeapon(){
        yield return new WaitForSeconds(0.2f);
        ChangeWeaponsClipping(currentWeapon, 0, true);
    }

    public void ChangeWeapons(bool firstActive, bool secondActive, bool meleeActive){
        if (_firstWeapon != null) if (firstWeaponObject.activeSelf != firstActive && firstWeaponObject != null) firstWeaponObject.SetActive(firstActive);
        if (_secondWeapon != null) if (secondWeaponObject.activeSelf != secondActive && secondWeaponObject != null) secondWeaponObject.SetActive(secondActive);
        if (_meleeWeapon != null) if (meleeWeaponObject.activeSelf != meleeActive && meleeWeaponObject != null) meleeWeaponObject.SetActive(meleeActive);
    }

    public void ChangeWeaponsClipping(GameObject weaponObj, float value, bool lerp){
        var objectMaterials = weaponObj.GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer materials in objectMaterials){
            materials.sharedMaterial.SetFloat("_ClippingValue", lerp ? Mathf.Lerp(materials.sharedMaterial.GetFloat("_ClippingValue"), value, 0.19f) : value);
        }

    }
}
