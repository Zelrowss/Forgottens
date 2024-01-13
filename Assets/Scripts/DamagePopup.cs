using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using System;

public class DamagePopup : MonoBehaviour
{

    private TextMeshPro textMesh;
    private float disableTimer;
    private Color textColor;

    private void Awake(){
        textMesh = GetComponent<TextMeshPro>();
    }

    public static DamagePopup Create(GameObject Object, Vector3 position, Quaternion rotation, float damageAmount, float criticalMultiplier){
        GameObject damagePopupGO = Instantiate(Object, position, rotation);

        DamagePopup damagePopup = damagePopupGO.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount, criticalMultiplier);

        return damagePopup;
    }

    public void Setup(float damageAmount, float criticalMultiplier){
        textMesh.SetText(damageAmount.ToString());
        
        disableTimer = 1;

        if (criticalMultiplier == 0f){
            textColor = Color.white;
            textMesh.fontSize = 1.5f;
        }
        else if (criticalMultiplier == 1.5f){
            textColor = new Color(1f, 1f, 0f);
            textMesh.fontSize = 2f;
        }
        else if (criticalMultiplier == 3f){
            textColor = new Color(1, 0.647f, 0);
            textMesh.fontSize = 2.5f;
        }
        else if (criticalMultiplier == 6f){
            textColor = new Color(1f, 0f, 0f);
            textMesh.fontSize = 3f;
        }
        else if (criticalMultiplier == 12){
            textColor = new Color(0.502f, 0, 0.502f);
            textMesh.fontSize = 3.5f;
        }

        textMesh.color = textColor;
    }

    private void Update(){
        disableTimer -= Time.deltaTime;
        if (disableTimer < 0){
            float disableSpeed = 3;
            textColor.a -= disableSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0){
                Destroy(gameObject);
            }
        }
    }

}
