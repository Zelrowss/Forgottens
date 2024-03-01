using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public class ProcElement{
    public Element procElement;
    public int procElementsNumbers;
    public float lastProcTime;
}

public class Enemy : MonoBehaviour
{
    [Header("Speed")]
    public float moveSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float sprintSpeed;

    [Header("Value")]
    public float maxHealth;
    public float maxShield;
    public float health;
    public float shield;
    public Element weakness;
    public List<ProcElement> currentProcs = new List<ProcElement>();
    public bool cantMove;
    public bool isConfused;

    [Header("Value (bis)")]
    private float lastDecrementTime;

    [Header("References")]
    public Image healthBar, shieldBar;
    public Sprite[] ElementsSprite;
    public Image[] ElementImages;
    private WeaponsManager _weaponManager;
    public GameObject electricityZone;

    void Awake(){
        
    }


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        shield = maxShield;
    }

    // Update is called once per frame
    void Update()
    {
        CantMove();
        if (_weaponManager == null) _weaponManager = GameObject.FindObjectOfType<WeaponsManager>();

        if (health <= 0 && shield <= 0){
            Destroy(gameObject);
        }
        
        healthBar.fillAmount = health / maxHealth;
        shieldBar.fillAmount = shield / maxShield;

        if (currentProcs.Count != 0){
            for (int i = 0; i < currentProcs.Count; i++){
                ProcElement proc = currentProcs[i];

                switch(proc.procElement){
                    case Element.Ice:
                        if (_weaponManager.elementalStrength <= 100) moveSpeed /= (2 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 200) moveSpeed /= (2.5f + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 300) moveSpeed /= (3 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 400) moveSpeed /= (3.5f + (0.1f * proc.procElementsNumbers));
                        break;
                    case Element.Fire:
                        StartCoroutine(DamageOverTime(gameObject, _weaponManager.finalDamage / 2, 1));

                        break;
                    case Element.Earth:
                        cantMove = true;
                        if (_weaponManager.elementalStrength <= 100) StartCoroutine(RemoveAfterTime(cantMove, 0.5f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 200) StartCoroutine(RemoveAfterTime(cantMove, 1f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 300) StartCoroutine(RemoveAfterTime(cantMove, 1.5f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 400) StartCoroutine(RemoveAfterTime(cantMove, 2f + (0.1f * proc.procElementsNumbers)));
                        break;
                    case Element.Steam:
                        if (_weaponManager.elementalStrength <= 100) StartCoroutine(RemoveAfterTime(isConfused, 0.5f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 200) StartCoroutine(RemoveAfterTime(isConfused, 1f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 300) StartCoroutine(RemoveAfterTime(isConfused, 1.5f + (0.1f * proc.procElementsNumbers)));
                        else if (_weaponManager.elementalStrength <= 400) StartCoroutine(RemoveAfterTime(isConfused, 2f + (0.1f * proc.procElementsNumbers)));
                        break;
                    case Element.Mud:
                        if (_weaponManager.elementalStrength <= 100) moveSpeed /= (3 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 200) moveSpeed /= (3.5f + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 300) moveSpeed /= (3 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 400) moveSpeed /= (3.5f + (0.1f * proc.procElementsNumbers));
                        break;
                    case Element.Lava:
                        StartCoroutine(DamageOverTime(gameObject, _weaponManager.finalDamage, 1));
                        break;
                    case Element.IceLava:
                        if (_weaponManager.elementalStrength <= 100) moveSpeed /= (2 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 200) moveSpeed /= (2.5f + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 300) moveSpeed /= (3 + (0.1f * proc.procElementsNumbers));
                        else if (_weaponManager.elementalStrength <= 400) moveSpeed /= (3.5f + (0.1f * proc.procElementsNumbers));

                        StartCoroutine(DamageOverTime(gameObject, _weaponManager.finalDamage / 2, 1));

                        break;
                    case Element.Electricity:
                        ElectricityZone electricityZoneComponent = GetComponentInChildren<ElectricityZone>();
                        if (_weaponManager.elementalStrength <= 100){
                            electricityZone.transform.localScale = new Vector3(3, 3, 3);
                            electricityZoneComponent.limitedEnemy = 3;
                        }
                        else if (_weaponManager.elementalStrength <= 200){
                            electricityZone.transform.localScale = new Vector3(4, 4, 4);
                            electricityZoneComponent.limitedEnemy = 5;
                        }
                        else if (_weaponManager.elementalStrength <= 300){
                            electricityZone.transform.localScale = new Vector3(5, 5, 5);
                            electricityZoneComponent.limitedEnemy = 7;
                        }
                        else if (_weaponManager.elementalStrength <= 400){
                            electricityZone.transform.localScale = new Vector3(4, 4, 4);
                            electricityZoneComponent.limitedEnemy = 9;
                        }

                        for (int j = 0; j < electricityZoneComponent.enemyInZone.Count; j++){
                            StartCoroutine(DamageOverTime(electricityZoneComponent.enemyInZone[j], _weaponManager.finalDamage / 2, 1));
                        }

                        break;


                }

                if (Time.time - proc.lastProcTime >= _weaponManager.elementalDuration){
                    
                    if (proc.procElementsNumbers > 0){      
                        
                        if (Time.time - lastDecrementTime >= 1){
                            proc.procElementsNumbers--;
                            lastDecrementTime = Time.time;

                        }

                    }
                    else{
                        for (int j = 0; j < ElementImages.Length; j++){
                            if (ElementImages[j].sprite.name == proc.procElement.ToString()){
                                ElementImages[j].sprite = null;
                                ElementImages[j].enabled = false;
                                break;
                            }
                        }

                        currentProcs.Remove(proc);
                    }
                }
            }
        }

    }

    IEnumerator RemoveAfterTime(bool toRemove, float delay){
        yield return new WaitForSeconds(delay);
        toRemove = false;
    }

    IEnumerator DamageOverTime(GameObject gameObject, float damageAmout, float delay){
        yield return new WaitForSeconds(delay);
        Enemy gameobjectComponent = gameObject.GetComponent<Enemy>();

        if (gameobjectComponent.shield > 0) gameobjectComponent.shield -= damageAmout / 2;
        else gameobjectComponent.health -= damageAmout / 2;
    }

    void CantMove()
    {
        if(cantMove == true)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<NavMeshAgent>().speed = 0;
        }
        else if(cantMove == false)
        {
            GetComponent<Animator>().enabled = true;
            GetComponent<NavMeshAgent>().speed = 3;
        }
    }
}
