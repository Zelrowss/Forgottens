using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DestroyableObjects : MonoBehaviour
{
    public float maxHealth = 100;
    public float health = 100;
    public Image healthBar;
    public Vector3 markerOffset = new Vector3(0, 2, 0);

    void Start() {
        health = maxHealth;
        if (healthBar != null) healthBar.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null){
            if (healthBar.fillAmount != (health / maxHealth)) healthBar.fillAmount = (health / maxHealth);
        }

        if (health <= 0) {

            if (gameObject.CompareTag("SabotageObjectif")) {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().RemoveMarkerByPosition(gameObject.transform.position + markerOffset);

                MissionController _missionController = GameObject.FindGameObjectWithTag("Mission Manager").GetComponent<MissionController>();

                if (!_missionController.alerted) _missionController.alerted = true;

                for (int i = 0; i < _missionController.objectives.Count(); i++){
                    if (_missionController.objectives[i] != gameObject) continue;

                    if (i + 1 < _missionController.objectives.Count()){
                        _missionController.currentObjectif = _missionController.objectives[i + 1];
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().CreateNewMarker(_missionController.currentObjectif.transform.position + markerOffset, null, "Position");
                        break;
                    }
                    else{
                        _missionController.isFinish = true;
                    }
                }
            }
            
            Destroy(gameObject);
        }
    }
}
