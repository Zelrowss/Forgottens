using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private WeaponsManager _weaponManager;

    void Awake()
    {
        _weaponManager = GameObject.FindObjectOfType<WeaponsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Color trailColor = Color.white; // Couleur par défaut

        switch (_weaponManager.currentElement)
        {
            case Element.Ice:
                trailColor = new Color(0, 119, 190);
                break;
            case Element.Fire:
                trailColor = new Color(255, 0, 0);
                break;
            case Element.Air:
                trailColor = new Color(135, 206, 235);
                break;
            case Element.Earth:
                trailColor = new Color(139, 69, 19);
                break;
            case Element.Lava:
                trailColor = new Color(255, 69, 0);
                break;
            case Element.Steam:
                trailColor = new Color(255, 255, 255, 128);
                break;
            case Element.Mud:
                trailColor = new Color(139, 115, 91);
                break;
            case Element.Haze:
                trailColor = new Color(245, 245, 245);
                break;
            case Element.Viral:
                trailColor = new Color(0, 255, 0);
                break;
            case Element.Wind:
                trailColor = new Color(255, 255, 255, 128);
                break;
            case Element.Blast:
                trailColor = new Color(255, 255, 0);
                break;
            case Element.Corrosive:
                trailColor = new Color(185, 122, 87);
                break;
            case Element.Electricity:
                trailColor = new Color(255, 215, 0);
                break;
        }

        ChangeTrailColor(trailColor);
    }

    void ChangeTrailColor(Color color)
    {
        GetComponent<TrailRenderer>().material.SetColor("_Color", color);
        GetComponent<TrailRenderer>().material.SetColor("_EmissionColor", color);
        var particleSystem = _weaponManager.muzzleFlash.GetComponentsInChildren<ParticleSystem>();
        _weaponManager.elementaryColor = color;

        for (int i = 0; i < particleSystem.Length; i++)
        {
            particleSystem[i].startColor = color;
        }
    }
}
