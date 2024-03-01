using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEchap : MonoBehaviour
{
    public bool blocageDesBoutons;

    [Header("Références")] [Space(10)]
    public Options options;
    public PlayerController playerController;
    public PlayerManager playerManager;
    public Affichage affichage;

    [Header("Autre")] [Space(10)]
    public EventSystem eventSystem;
    public GameObject normalCamera;

    [Header("Menus")] [Space(10)]
    public GameObject panelOptions;

    [Header("Couleurs Importantes")] [Space(10)]
    public Color gris;
    public Color turquoise;
    public Color pasCliquable;
    public Color selected;

    public void OpenMenuEchap()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.enabled = false;
        normalCamera.GetComponent<CinemachineVirtualCamera>().enabled = false;

        // Enregistre tout ce qu'il faut avant une modification de l'Affichage
        for (int i = 0; i < 12; i += 1)
        {
            affichage.avantConfirmationInfos[i] = affichage.infos[i].text;
        }
    }

    public void CloseMenuEchap()
    {
        gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.enabled = true;
        normalCamera.GetComponent<CinemachineVirtualCamera>().enabled = true;

        panelOptions.SetActive(false);

        // Désélectionne toutes les catégories
        foreach (var item in options.selectedBar)
        {
            item.gameObject.SetActive(false);
        }

        // Déhovered toutes les catégories
        foreach (var item in options.hoveredBar)
        {
            item.gameObject.SetActive(false);
        }

        // Ferme toutes les Catégories
        foreach (var item in options.contenu)
        {
            item.SetActive(false);
        }

        // Remet à 0 les couleurs de tous les textes
        foreach (var item in options.text)
        {
            item.color = gris;
        }

        // Sélectionne, ouvre et colore la première catégorie
        options.selectedBar[0].gameObject.SetActive(true);
        options.contenu[0].SetActive(true);
        options.text[0].color = turquoise;

        // Ferme tous les Dropdowns ouverts
        foreach (var item in affichage.dropdowns)
        {
            item.SetActive(false);
        }

        // Active tous les boutons pour ouvrir les Dropdowns
        foreach (var item in affichage.ouvrirDropdowns)
        {
            item.SetActive(true);
        }

        // Enléve la couleur du bouton Retour et des boutons de Réinitialisation
        options.textRetour.color = gris;
        if (options.commandes.textReinitialiser.color == turquoise)
        {
            options.commandes.textReinitialiser.color = gris;
        }
        if (options.affichage.textReinitialiser.color == turquoise)
        {
            options.affichage.textReinitialiser.color = gris;
        }

        // Remet les textes titres et informations des boutons dans les options en blancs
        affichage.RemiseZeroTextes();
    }

    public void Continuer()
    {
        CloseMenuEchap();
        eventSystem.SetSelectedGameObject(null);
    }

    public void Social()
    {
        // Vide pour l'instant
    }

    public void Options()
    {
        panelOptions.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        options.scrollbars[0].value = 1;

        // Enregistre tout ce qu'il faut avant une modification de l'Afichage
        if (affichage.gameObject.activeSelf == true)
        {
            for (int i = 0; i < 12; i += 1)
            {
                affichage.avantConfirmationInfos[i] = affichage.infos[i].text;
            }
        }
    }

    public void ResultatDeLaPartie()
    {
        // Vide pour l'instant
    }

    public void Quitter()
    {
        // Action différente en fonction de si le joueur est dans une mission ou non

        if (playerManager.isInMission == false)
        {
            Application.Quit();
        }
    }
}
