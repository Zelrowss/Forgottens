using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.HighDefinition;
using Cinemachine;
using System.Collections.Generic;


//Clair: 24ebcb
//Foncé : 1ba18b

public class Affichage : MonoBehaviour
{
    [Header("Général")] [Space(10)]
    public EventSystem eventSystem;
    public string nomDuBouton;
    public TMP_Text textReinitialiser;
    public GameObject panelReinitialisation;
    public TMP_Text textConfirmer;
    public GameObject panelConfirmation;
    public string enregistrementDuClic;

    [Header("Panel Aide")] [Space(10)]
    public List<GameObject> textesAide;

    [Header("Références")] [Space(10)]
    public CameraController cameraController;
    public MenuEchap menuEchap;

    [Header("Gestion des Dropdowns")] [Space(10)]
    public GameObject[] dropdowns;
    public GameObject[] ouvrirDropdowns;

    [Header("Arrays")] [Space(10)]
    public TMP_Text[] texts;
    public TMP_Text[] titres;
    public TMP_Text[] infos;
    public string[] défautInfos;
    public string[] avantConfirmationInfos;

    [Header("Mode Fenêtre")] [Space(10)]
    public TMP_Text textFenetre;
    public bool fullscreen;

    [Header("Résolutions")] [Space(10)]
    public TMP_Text textResolution;

    [Header("Anti-Crénelage")] [Space(10)]
    public Camera cameraP;
    public TMP_Text textAntiCrenelage;

    [Header("VSYNC")] [Space(10)]
    public TMP_Text textVsync;

    [Header("FPS")] [Space(10)]
    public TMP_Text textFps;

    [Header("FOV")] [Space(10)]
    public Slider sliderFov;
    public TMP_Text textFov;

    [Header("Sensibilité Caméra")] [Space(10)]
    public Slider sliderSensibiliteCameraClassique;
    public Slider sliderSensibiliteCameraVisee;
    public TMP_Text textSensibiliteCameraClassique;
    public TMP_Text textSensibiliteCameraVisee;

    [Header("Distance de Rendu")] [Space(10)]
    public CinemachineVirtualCamera normalCamera;
    public CinemachineVirtualCamera aimCamera;
    public TMP_Text textDistanceDeRendu;
    public Slider sliderDistanceRendu;

    [Header("Luminosité")] [Space(10)]
    public Slider sliderLuminositee;
    public TMP_Text textLuminositee;

    public float test;

    private void Start()
    {
        fullscreen = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RemiseZeroTextes()
    {
        // Remet les textes des boutons dans les options en blancs
        foreach (var item in texts)
        {
            item.color = Color.white;
        }
        foreach (var item in titres)
        {
            item.color = Color.white;
        }
        foreach (var item in infos)
        {
            item.color = Color.white;
        }
    }

    public void ClickDesBoutons()
    {
        // Mode Fenêtre
        if (nomDuBouton == "Plein Ecran")
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            textFenetre.text = "Plein Ecran";
            fullscreen = true;
        }
        else if (nomDuBouton == "Plein Ecran Fenêtré")
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            textFenetre.text = "Plein Ecran Fenêtré";
            fullscreen = true;
        }
        else if (nomDuBouton == "Fenêtré")
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            textFenetre.text = "Fenêtré";
            fullscreen = false;
        }

        // Résolutions
        else if (nomDuBouton == "1280 x 720")
        {
            Screen.SetResolution(1280, 720, fullscreen);
            textResolution.text = "1280 x 720";
        }
        else if (nomDuBouton == "1366 x 768")
        {
            Screen.SetResolution(1366, 768, fullscreen);
            textResolution.text = "1366 x 768";
        }
        else if (nomDuBouton == "1680 x 1050")
        {
            Screen.SetResolution(1680, 1050, fullscreen);
            textResolution.text = "1680 x 1050";
        }
        else if (nomDuBouton == "1920 x 1080")
        {
            Screen.SetResolution(1920, 1080, fullscreen);
            textResolution.text = "1920 x 1080";
        }
        else if (nomDuBouton == "2560 x 1440")
        {
            Screen.SetResolution(2560, 1440, fullscreen);
            textResolution.text = "2560 x 1440";
        }
        else if (nomDuBouton == "2560 x 1600")
        {
            Screen.SetResolution(2560, 1600, fullscreen);
            textResolution.text = "2560 x 1600";
        }
        else if (nomDuBouton == "3840 x 2160")
        {
            Screen.SetResolution(3840, 2160, fullscreen);
            textResolution.text = "3840 x 2160";
        }

        // Anti Crénelage
        else if (nomDuBouton == "No Anti-Aliasing")
        {
            cameraP.GetComponent<HDAdditionalCameraData>().antialiasing = 
                HDAdditionalCameraData.AntialiasingMode.None;
            textAntiCrenelage.text = "No Anti-Aliasing";
        }
        else if (nomDuBouton == "Approximatif Rapide")
        {
            cameraP.GetComponent<HDAdditionalCameraData>().antialiasing = 
                HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
            textAntiCrenelage.text = "Approximatif Rapide";
        }
        else if (nomDuBouton == "Temporel")
        {
            cameraP.GetComponent<HDAdditionalCameraData>().antialiasing = 
                HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
            textAntiCrenelage.text = "Temporel";
        }
        else if (nomDuBouton == "Morphologique")
        {
            cameraP.GetComponent<HDAdditionalCameraData>().antialiasing = 
                HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            textAntiCrenelage.text = "Morphologique";
        }

        // VSYNC
        else if (nomDuBouton == "VSYNC")
        {
            if (textVsync.text == "Désactivé")
            {
                QualitySettings.vSyncCount = 1;
                textVsync.text = "Activé";
            }
            else
            {
                QualitySettings.vSyncCount = 0;
                textVsync.text = "Désactivé";
            }
        }

        // FPS
        else if (nomDuBouton == "Illimité")
        {
            Application.targetFrameRate = -1;
            textFps.text = "Illimité";
        }
        else if (nomDuBouton == "120")
        {
            Application.targetFrameRate = 120;
            textFps.text = "120";
        }
        else if (nomDuBouton == "60")
        {
            Application.targetFrameRate = 60;
            textFps.text = "60";
        }
        else if (nomDuBouton == "40")
        {
            Application.targetFrameRate = 40;
            textFps.text = "40";
        }
        else if (nomDuBouton == "20")
        {
            Application.targetFrameRate = 20;
            textFps.text = "20";
        }

        // Flou Cinétique
        else if (nomDuBouton == "Flou Cinétique")
        {
            
        }

        // Grain de Film
        else if (nomDuBouton == "Grain de Film")
        {

        }

        // Langues
        else if (nomDuBouton == "Langues")
        {

        }

        VerificationReinitialisation();
        VerificationConfirmation();
    }

    // FOV
    public void SliderFov()
    {
        cameraController.baseFOV = sliderFov.value;
        aimCamera.m_Lens.FieldOfView = sliderFov.value - 10;

        textFov.text = sliderFov.value.ToString();

        VerificationReinitialisation();
        VerificationConfirmation();
    }

    // Sensibilité Caméra Classique
    public void SliderSensibiliteCameraClassique()
    {
        cameraController.normalSensitivity = sliderSensibiliteCameraClassique.value;
        textSensibiliteCameraClassique.text = sliderSensibiliteCameraClassique.value.ToString();

        VerificationReinitialisation();
        VerificationConfirmation();
    }

    // Sensibilité Caméra Visée
    public void SliderSensibiliteCameraVisee()
    {
        cameraController.aimSensitivity = sliderSensibiliteCameraVisee.value;
        textSensibiliteCameraVisee.text = sliderSensibiliteCameraVisee.value.ToString();

        VerificationReinitialisation();
        VerificationConfirmation();
    }

    // Distance de Rendu
    public void SliderDistanceDeRendu()
    {
        normalCamera.m_Lens.FarClipPlane = sliderDistanceRendu.value;
        textDistanceDeRendu.text = sliderDistanceRendu.value.ToString();

        VerificationReinitialisation();
        VerificationConfirmation();
    }
    
    // Slider Luminosité
    public void SliderLuminositee()
    {
        
    }

    public void ClickReinitialiserAffichage()
    {
        if (textReinitialiser.color == menuEchap.turquoise)
        {
            panelReinitialisation.SetActive(true);
            menuEchap.blocageDesBoutons = true;
        }
    }

    public void DeclenchementReinitialiser()
    {
        // Mode Fenêtre ==> Plein Ecran
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        textFenetre.text = "Plein Ecran";
        fullscreen = true;

        // Résolution de l'Ecran ==> 1920 x 1080
        Screen.SetResolution(1920, 1080, fullscreen);
        textResolution.text = "1920 x 1080";

        // Anti-Crénelage ==> Morphologique
        cameraP.GetComponent<HDAdditionalCameraData>().antialiasing =
                HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
        textAntiCrenelage.text = "Morphologique";

        // VSYNC ==> Activé
        QualitySettings.vSyncCount = 1;
        textVsync.text = "Activé";

        // FPS ==> Détecte les FPS de l'écran du joueur et sélectionne la valeur disponible la plus proche
        Application.targetFrameRate = -1;
        textFps.text = "Illimité";

        // FOV ==> 40 pour la classique et 30 pour la visée
        sliderFov.value = 40;
        cameraController.baseFOV = sliderFov.value;
        aimCamera.m_Lens.FieldOfView = sliderFov.value - 10;
        textFov.text = sliderFov.value.ToString();

        // Sensibiliyé Caméra Classique ==> 50
        sliderSensibiliteCameraClassique.value = 50;
        cameraController.normalSensitivity = sliderSensibiliteCameraClassique.value;
        textSensibiliteCameraClassique.text = sliderSensibiliteCameraClassique.value.ToString();

        // Sensibilité Caméra Visée ==> 10
        sliderSensibiliteCameraVisee.value = 10;
        cameraController.aimSensitivity = sliderSensibiliteCameraVisee.value;
        textSensibiliteCameraVisee.text = sliderSensibiliteCameraVisee.value.ToString();

        // Distance de Rendu ==> 500
        sliderDistanceRendu.value = 500;
        normalCamera.m_Lens.FarClipPlane = sliderDistanceRendu.value;
        textDistanceDeRendu.text = sliderDistanceRendu.value.ToString();

        // Flou Cinétique

        // Grain de Film

        // Luminosité

        // Langues

        textReinitialiser.color = menuEchap.pasCliquable;
        textConfirmer.color = menuEchap.pasCliquable;
    }

    // Vérifie si le bouton Réinitialiser doit apparaître on non
    public void VerificationReinitialisation()
    {
        textReinitialiser.color = menuEchap.pasCliquable;

        for (int i = 0; i < 13; i += 1)
        {
            if (infos[i].text != défautInfos[i])
            {
                textReinitialiser.color = menuEchap.gris; break;
            }
        }
    }

    public void ClickConfirmerAffichage()
    {
        if (textConfirmer.color == menuEchap.turquoise)
        {
            textConfirmer.color = menuEchap.pasCliquable;

            // Enregistre tout ce qu'il faut avant une modification de l'Affichage
            for (int i = 0; i < 12; i += 1)
            {
                avantConfirmationInfos[i] = infos[i].text;
            }
        }
    }

    public void NonConfirmation()
    {
        for (int i = 0; i < 13;i += 1)
        {
            // Aucun slider là dedans
            if (i < 5)
            {
                if (i == 3)
                {
                    nomDuBouton = "VSYNC";
                    ClickDesBoutons();
                }
                else
                {
                    nomDuBouton = avantConfirmationInfos[i];
                    ClickDesBoutons();
                }
            }

            // Ici malheureusement c'est mélangé faut faire au cas par cas
            else
            {
                // FOV
                if (i == 5)
                {
                    sliderFov.value = float.Parse(avantConfirmationInfos[i]);
                    SliderFov();
                }
                // Sensibilité Caméra
                else if (i == 6)
                {
                    sliderSensibiliteCameraClassique.value = float.Parse(avantConfirmationInfos[i]);
                    SliderSensibiliteCameraClassique();
                }
                // Sensibilité Visée
                else if (i == 7)
                {
                    sliderSensibiliteCameraVisee.value = float.Parse(avantConfirmationInfos[i]);
                    SliderSensibiliteCameraVisee();
                }
                // Distance de Rendu
                else if (i == 8)
                {
                    sliderDistanceRendu.value = float.Parse(avantConfirmationInfos[i]);
                    SliderDistanceDeRendu();
                }
                
                else if (i == 9)
                {
                    nomDuBouton = "Flou Cinétique";
                    ClickDesBoutons();
                }

                else if (i == 10)
                {
                    nomDuBouton = "Grain de Film";
                    ClickDesBoutons();
                }

                // Slider Luminosité
                else if (i == 11)
                {

                }

                else if (i == 12)
                {
                    nomDuBouton = avantConfirmationInfos[i];
                    ClickDesBoutons();
                }
            }
        }

        textConfirmer.color = menuEchap.pasCliquable;
    }

    // Vérifie si le bouton Confirmer doit apparaître ou non
    public void VerificationConfirmation()
    {
        textConfirmer.color = menuEchap.pasCliquable;

        for (int i = 0; i < 13; i += 1)
        {
            if (infos[i].text != avantConfirmationInfos[i])
            {
                textConfirmer.color = menuEchap.gris; break;
            }
        }
    }

    public void EnterReinitialiserAffichage()
    {
        if (textReinitialiser.color == menuEchap.gris)
        {
            textReinitialiser.color = menuEchap.turquoise;
        }
    }

    public void ExitReinitialiserAffichage()
    {
        if (textReinitialiser.color == menuEchap.turquoise)
        {
            textReinitialiser.color = menuEchap.gris;
        }
    }
    
    public void EnterConfirmerAffichage()
    {
        if (textConfirmer.color == menuEchap.gris)
        {
            textConfirmer.color = menuEchap.turquoise;
        }
    }

    public void ExitConfirmerAffichage()
    {
        if (textConfirmer.color == menuEchap.turquoise)
        {
            textConfirmer.color = menuEchap.gris;
        }
    }
}