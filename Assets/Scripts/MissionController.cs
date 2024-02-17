using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MissionType
{
    Survie,
    Defense,
    Extermination,
    Capture,
    CaptureTheFlag,
    Sabotage,
    Infiltration,
}

public enum EnemyFaction {
    Voxis,
    Atralis,
    Astrion,
    Zenithar,
}

public class MissionController : MonoBehaviour 
{
    [Header("References")]
    public GameObject player;

    [Header("Values")]
    public MissionType currentMissionType;
    public int enemyMaxLevel, enemyMinLevel;
    public bool isFinish;
    public bool alerted = false;
    private bool sendExtractionPoint = false;
    public Vector3 extractionPoint;

    [Header("Capture")]
    public int distanceMin = 50;

    [Header("Extermination")]
    public int enemyToKill;
    public float lastEnemyKillTime;
    public int enemyKilledAmount;
    public bool help;
    public GameObject exterminationHUD;

    [Header("Sabotage")]
    public GameObject[] objectives;
    public GameObject currentObjectif;

    // Rechercher un ennemie aléatoire à une certain distance et le defini comme cible
    private void FindValidCaptureTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> validEnemies = new List<GameObject>();

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(player.transform.position, enemy.transform.position);
            
            if (distanceToEnemy > distanceMin)
            {
                validEnemies.Add(enemy);
            }
        }

        if (validEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, validEnemies.Count);
            GameObject randomEnemy = validEnemies[randomIndex];
            player.GetComponent<PlayerManager>().objectif = randomEnemy;
        }
        else
        {
            Debug.LogWarning("Aucun ennemi valide trouvé pour la capture.");
        }
    }

    private void Start() 
    {
        switch (currentMissionType)
        {
            case MissionType.Capture:
                FindValidCaptureTarget();
                break;
            case MissionType.Extermination:
                enemyKilledAmount = 0;
                GameObject exterminationHUDObj = Instantiate(exterminationHUD, player.GetComponent<PlayerManager>().playerCanvas.transform);

                foreach (var filler in GameObject.FindObjectsOfType<Image>(true)) {
                    if (filler.name != "ExterminationHUD - filler") continue;

                    if (filler.fillAmount != null) filler.fillAmount = 0;
                }

                exterminationHUDObj.GetComponentInChildren<TextMeshProUGUI>().text = "0 / 10";
                player.GetComponent<PlayerManager>().exterminationHUD = exterminationHUDObj;
                break;
            case MissionType.Sabotage:
                currentObjectif = objectives[0];
                player.GetComponent<PlayerManager>().objectif = currentObjectif;
                break;
        }
    }

    private void Update(){
        if (isFinish) {
            // Quand la mission est fini, envoie le point d'extraction
            if (!sendExtractionPoint) {
                sendExtractionPoint = true;

                player.GetComponent<PlayerManager>().CreateNewMarker(extractionPoint, null, "Extract");
            }

            // Verifie si le joueur est assez proche de l'extraction pour finir la mission
            if (Vector3.Distance(player.transform.position, extractionPoint) <= 10) {
                // Faire la logique d'extraction
            }
        }
        else {
            if (currentMissionType == MissionType.Extermination) {
                if (player.GetComponent<PlayerManager>().exterminationHUD.GetComponentInChildren<TextMeshProUGUI>().text != enemyKilledAmount + " / " + enemyToKill) {
                    player.GetComponent<PlayerManager>().exterminationHUD.GetComponentInChildren<TextMeshProUGUI>().text = enemyKilledAmount + " / " + enemyToKill;
                }

                foreach (var filler in GameObject.FindObjectsOfType<Image>(true)) {
                    if (filler.name != "ExterminationHUD - filler") continue;
                    
                    if (filler.fillAmount != (float)enemyKilledAmount / enemyToKill) filler.fillAmount = (float)enemyKilledAmount / enemyToKill;
                }

                // Si ca fait +5 que je n'ai pas tué d'ennemis lors d'une extermination alors je lui pointe l'ennemi le plus proche
                if ((Time.time - lastEnemyKillTime) >= 10 && !help && enemyKilledAmount < enemyToKill) {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                    GameObject nearestEnemy = null;
                    float minDistance = Mathf.Infinity;

                    foreach (GameObject enemy in enemies) {
                        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                        
                        if (distance < minDistance) {
                            nearestEnemy = enemy;
                            minDistance = distance;
                        }
                    }

                    if (nearestEnemy != null) {
                        player.GetComponent<PlayerManager>().CreateNewMarker(nearestEnemy.transform.position, nearestEnemy, "Kill");
                    } 
                    else {
                        Debug.LogWarning("Aucun ennemi trouvé.");
                    }

                    help = true;
                }

                // Fini la mission lorsqu'on a tuer assez d'ennemis
                if (enemyKilledAmount >= enemyToKill) {
                    player.GetComponent<PlayerManager>().DeleteAllMarker();

                    isFinish = true;
                }
            }
        }

    }
}
