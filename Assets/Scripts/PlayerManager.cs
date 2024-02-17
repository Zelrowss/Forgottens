using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Weapons{
    Primary,
    Secondary,
    Melee,
}

[System.Serializable]
public class MarkerInfo {
    public GameObject markerObject;
    public GameObject enemy;
    public Vector3 position;
    public string type;
}

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public GameObject _weaponHolder;
    public GameObject _firstWeapon;
    public GameObject _secondWeapon;
    public GameObject _meleeWeapon;

    [Header("Markers")]
    public GameObject killMarkerPrefab;
    public GameObject positionMarkerPrefab;
    public GameObject extractionMarkerPrefab;

    [Header("HUD")]
    public Canvas playerCanvas;
    public GameObject exterminationHUD;

    [Header("Player")]
    public bool isInMission = false;
    [HideInInspector] public bool weaponChanging;
    public float health, shield, energy, ammunition;
    public List<MarkerInfo> markers;
    
    [Header("Weapons")]
    public Weapons currentWeaponsPlace;
    public GameObject currentWeapon;
    [HideInInspector] public GameObject firstWeaponObject;
    [HideInInspector] public GameObject secondWeaponObject;
    [HideInInspector] public GameObject meleeWeaponObject;

    [Header("Missions")]
    public GameObject objectif;

    // Actualise le rigg du personne (<!> très couteux en ressource <!>)
    public IEnumerator ReBuildRig(){
        yield return new WaitForSeconds(0.001f);
        GetComponentInChildren<UnityEngine.Animations.Rigging.RigBuilder>().Build();
        yield return new WaitForSeconds(0.5f);
        weaponChanging = false;
    }

    // Affiche la position de la cible d'une capture avec un délai
    IEnumerator WaitForSetCaptureMarker(){
        yield return new WaitForSeconds(1);
        CreateNewMarker(objectif.transform.position, objectif, "Kill");
    }

    IEnumerator WaitForSetObjectifMarker(){
        yield return new WaitForSeconds(1);
        CreateNewMarker(objectif.transform.position + objectif.GetComponent<DestroyableObjects>().markerOffset, null, "Position");
    }

    // Petite latence lors du changement d'arme
    IEnumerator WaitForNewWeapon(){
        yield return new WaitForSeconds(0.2f);
        ChangeWeaponsClipping(currentWeapon, 0, true);
    }

    // Changement de l'arme
    public void ChangeWeapons(bool firstActive, bool secondActive, bool meleeActive){
        if (_firstWeapon != null) if (firstWeaponObject.activeSelf != firstActive && firstWeaponObject != null) firstWeaponObject.SetActive(firstActive);
        if (_secondWeapon != null) if (secondWeaponObject.activeSelf != secondActive && secondWeaponObject != null) secondWeaponObject.SetActive(secondActive);
        if (_meleeWeapon != null) if (meleeWeaponObject.activeSelf != meleeActive && meleeWeaponObject != null) meleeWeaponObject.SetActive(meleeActive);
    }

    // Effet de disparition de l'arme
    public void ChangeWeaponsClipping(GameObject weaponObj, float value, bool lerp){
        var objectMaterials = weaponObj.GetComponentsInChildren<MeshRenderer>();

        foreach(MeshRenderer materials in objectMaterials){
            materials.sharedMaterial.SetFloat("_ClippingValue", lerp ? Mathf.Lerp(materials.sharedMaterial.GetFloat("_ClippingValue"), value, 0.19f) : value);
        }

    }

    // Créer un nouveau marker. Utilisation CreateNewMarker(<Position voulu>, <Enemi ou null>, <"Kill" ou "Extract" ou "Position">)
    public void CreateNewMarker(Vector3 position, GameObject enemyObject, string type){
        GameObject markerObject = null;
        
        if (type == "Kill") {
            markerObject = Instantiate(killMarkerPrefab, playerCanvas.transform);
        }
        else if(type == "Extract") {
            markerObject = Instantiate(extractionMarkerPrefab, playerCanvas.transform);
        }
        else if(type == "Position") {
            markerObject = Instantiate(positionMarkerPrefab, playerCanvas.transform);
        }

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);

        Vector3 directionToEnemy = position - transform.position;
        float distance = directionToEnemy.magnitude;

        markerObject.GetComponentInChildren<TextMeshProUGUI>().text = distance.ToString("F1") + "m";

        markerObject.transform.position = screenPoint;

        MarkerInfo newMarker = new MarkerInfo();
        newMarker.markerObject = markerObject;
        newMarker.type = type;
        newMarker.position = position;
        if (enemyObject != null) {
            newMarker.enemy = enemyObject;
        }
        
        markers.Add(newMarker);
    }

    // Met à jour la position du marker sur l'écran
    private void UpdateMarkers(){
        foreach (MarkerInfo marker in markers)
        {   
            if (!marker.markerObject.activeSelf) marker.markerObject.SetActive(true);
            if (marker.enemy == null && marker.type == "Kill") {
                markers.Remove(marker);
                return;
            }

            marker.markerObject.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
            float distance = Vector3.Distance(marker.position, transform.position);

            marker.markerObject.GetComponentInChildren<TextMeshProUGUI>().text = distance.ToString("F1") + "m";

            if (marker.enemy != null)
            {
                if (distance < 1000 || !marker.enemy.activeSelf) return;

                Vector3 enemyWorldPosition = marker.enemy.transform.position + marker.enemy.GetComponent<Enemy>().headPos;

                Vector3 screenPoint = Camera.main.WorldToScreenPoint(enemyWorldPosition);

                if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height)
                {
                    marker.markerObject.transform.position = new Vector3(screenPoint.x, screenPoint.y, screenPoint.z);
                }
                else
                {
                    marker.markerObject.SetActive(false);
                }
            }
            else{
                Vector3 worldPosition = marker.position;

                Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);

                if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height)
                {
                    marker.markerObject.transform.position = new Vector3(screenPoint.x, screenPoint.y, screenPoint.z);
                }
                else
                {
                    marker.markerObject.SetActive(false);
                }
            }
        }
    }

    // Supprimer un marker fonction d'un enemy
    public void RemoveMarkerByEnemy(GameObject enemy) {
        for (int i = 0; i < markers.Count; i++){
            if (markers[i].enemy != enemy) continue;
            Destroy(markers[i].markerObject);
            markers.RemoveAt(i);
            break;
        }
    }
    
    public void RemoveMarkerByPosition(Vector3 objectifPos){
        for (int i = 0; i < markers.Count; i++){
            if (markers[i].position != objectifPos) continue;
            
            Destroy(markers[i].markerObject);
            markers.RemoveAt(i);
            break;
        }
    }

    public void DeleteAllMarker(){
        for (int i = 0; i < markers.Count; i++){
            if (markers[i].type != "Kill") continue;

            Destroy(markers[i].markerObject);
            markers.RemoveAt(i);
        }  
    }

    // Start is called before the first frame update
    void Start()
    {
        markers = new List<MarkerInfo>();
        MissionController missionManager = GameObject.FindGameObjectWithTag("Mission Manager").GetComponent<MissionController>();

        switch (missionManager.currentMissionType){
            case MissionType.Capture:
                StartCoroutine(WaitForSetCaptureMarker());
                break;
            case MissionType.Sabotage:
                StartCoroutine(WaitForSetObjectifMarker());
                break;
        }

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

    // Update is called once per frame
    void Update()
    {

        if (weaponChanging){
            ChangeWeaponsClipping(currentWeapon, 1, true);
            StartCoroutine(WaitForNewWeapon());
        }
    }

    void LateUpdate()
    {
        UpdateMarkers();
    }
}
