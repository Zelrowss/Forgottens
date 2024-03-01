using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class BoutonsPanelConfirmation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Affichage affichage;
    public Commandes commandes;
    public BoutonCategories boutonCategories;
    public PlayerManager playerManager;

    public TMP_Text text;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.black;
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.white;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        text.color = Color.white;
    }

    public void OuiReinitialiser()
    {
        commandes.menuEchap.blocageDesBoutons = false;

        if (commandes.gameObject.activeSelf == true)
        {
            foreach (var item in commandes.imagesDesTouches)
            {
                item.sprite = commandes.imageDefautDesTouches[commandes.imagesDesTouches.IndexOf(item)];
                item.gameObject.SetActive(true);
            }

            commandes.playerController.playerInputActions.RemoveAllBindingOverrides();

            commandes.panelReinitialisation.SetActive(false);

            commandes.glissade.sprite = commandes.imagesDesTouches[6].sprite;

            commandes.textConfirmer.color = commandes.menuEchap.pasCliquable;
        }

        else if (affichage.gameObject.activeSelf == true)
        {
            affichage.DeclenchementReinitialiser();
            affichage.panelReinitialisation.SetActive(false);
            affichage.textReinitialiser.color = affichage.menuEchap.pasCliquable;
        }
    }

    public void NonReinitialiser()
    {
        commandes.menuEchap.blocageDesBoutons = false;

        if (commandes.gameObject.activeSelf == true)
        {
            commandes.textReinitialiser.color = commandes.menuEchap.gris;
            commandes.panelReinitialisation.SetActive(false);
        }

        else if (affichage.gameObject.activeSelf == true)
        {
            affichage.panelReinitialisation.SetActive(false);
        }
    }

    public void OuiModification()
    {
        EventSystem.current.SetSelectedGameObject(null);
        commandes.menuEchap.blocageDesBoutons = false;

        if (commandes.gameObject.activeSelf == true)
        {
            commandes.panelModification.SetActive(false);
            commandes.NonConfirmation();

            if (commandes.enregistrementDuClic == "Bouton Retour")
            {
                commandes.menuEchap.options.ExecutionBoutonRetour();
            }
            else if (commandes.enregistrementDuClic == "Catégorie N° 0")
            {
                boutonCategories.index = 0;
                boutonCategories.ExecutionBoutonCategories();
            }
            else if (commandes.enregistrementDuClic == "Catégorie N° 1")
            {
                boutonCategories.index = 1;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (commandes.enregistrementDuClic == "Catégorie N° 2")
            {
                boutonCategories.index = 2;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (commandes.enregistrementDuClic == "Catégorie N° 3")
            {
                boutonCategories.index = 3;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (commandes.enregistrementDuClic == "Bouton Echap")
            {
                playerManager.ExecutionToucheEchap();
            }
        }

        else if (affichage.gameObject.activeSelf == true)
        {
            affichage.panelConfirmation.SetActive(false);
            affichage.NonConfirmation();

            if (affichage.enregistrementDuClic == "Bouton Retour")
            {
                affichage.menuEchap.options.ExecutionBoutonRetour();
            }
            else if (affichage.enregistrementDuClic == "Catégorie N° 0")
            {
                boutonCategories.index = 0;
                boutonCategories.ExecutionBoutonCategories();
            }
            else if (affichage.enregistrementDuClic == "Catégorie N° 1")
            {
                boutonCategories.index = 1;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (affichage.enregistrementDuClic == "Catégorie N° 2")
            {
                boutonCategories.index = 2;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (affichage.enregistrementDuClic == "Catégorie N° 3")
            {
                boutonCategories.index = 3;
                boutonCategories.ExecutionBoutonCategories();
                boutonCategories.index = 0;
            }
            else if (affichage.enregistrementDuClic == "Bouton Echap")
            {
                playerManager.ExecutionToucheEchap();
            }
        }
    }

    public void NonModification()
    {
        EventSystem.current.SetSelectedGameObject(null);
        commandes.menuEchap.blocageDesBoutons = false;

        if (commandes.gameObject.activeSelf == true)
        {
            commandes.panelModification.SetActive(false);
        }
        else if (affichage.gameObject.activeSelf == true)
        {
            affichage.panelConfirmation.SetActive(false);
        }
    }
}
