using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Options : MonoBehaviour
{
    [Header("References")] [Space(10)]
    public MenuEchap menuEchap;
    public Commandes commandes;
    public Affichage affichage;

    [Header("Catégories")] [Space(10)]
    public List<GameObject> categories;
    public List<TextMeshProUGUI> text;
    public List<Image> selectedBar;
    public List<Image> hoveredBar;
    public List<GameObject> contenu;

    public Scrollbar[] scrollbars;

    [Header("Bouton Retour")] [Space(10)]
    public TextMeshProUGUI textRetour;

    public void MouseEnterBoutonRetour()
    {
        textRetour.color = menuEchap.turquoise;
    }

    public void MouseExitBoutonRetour()
    {
        textRetour.color = menuEchap.gris;
    }

    public void MouseClickBoutonRetour()
    {
        if (commandes.textConfirmer.color == menuEchap.pasCliquable &&
            affichage.textConfirmer.color == menuEchap.pasCliquable)
        {
            ExecutionBoutonRetour();
        }

        else
        {
            if (commandes.textConfirmer.color == menuEchap.gris)
            {
                menuEchap.blocageDesBoutons = true;
                commandes.panelModification.SetActive(true);
                commandes.enregistrementDuClic = "Bouton Retour";
            }
            else if (affichage.textConfirmer.color == menuEchap.gris)
            {
                menuEchap.blocageDesBoutons = true;
                affichage.panelConfirmation.SetActive(true);
                affichage.enregistrementDuClic = "Bouton Retour";
            }
        }
    }

    public void ExecutionBoutonRetour()
    {
        textRetour.color = menuEchap.gris;
        menuEchap.panelOptions.SetActive(false);

        // Désélectionne toutes les catégories
        foreach (var item in selectedBar)
        {
            item.gameObject.SetActive(false);
        }

        // Déhovered toutes les catégories
        foreach (var item in hoveredBar)
        {
            item.gameObject.SetActive(false);
        }

        // Ferme toutes les Catégories
        foreach (var item in contenu)
        {
            item.SetActive(false);
        }

        // Remet à 0 les couleurs de tous les textes
        foreach (var item in text)
        {
            item.color = menuEchap.gris;
        }

        // Sélectionne, ouvre et colore la première catégorie
        selectedBar[0].gameObject.SetActive(true);
        contenu[0].SetActive(true);
        text[0].color = menuEchap.turquoise;
    }
}
