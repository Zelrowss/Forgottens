using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Commandes : MonoBehaviour
{
    public GameObject panelReinitialisation;
    public GameObject panelModification;
    public MenuEchap menuEchap;

    public List<Image> imagesDesTouches;
    public List<GameObject> boutonsDesTouches;
    public List<Sprite> avantConfirmationImage;
    public List<string> avantConfirmationPath;
    public List<bool> avantConfirmationEtat;
    public List<Sprite> imageDefautDesTouches;
    public List<Sprite> listeTouchesDisponibles;

    public PlayerController playerController;

    public TMP_Text textReinitialiser;
    public TMP_Text textConfirmer;
    public string enregistrementDuClic;

    public int indexListe;
    public int indexCommande;
    public string actionName;
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public Image glissade;
    public bool changementTouche;
    public string veritableTouche;
    public string toucheNouvelInput;

    private void OnGUI()
    {
        if (changementTouche == true)
        {
            Event zebi = Event.current;

            if (zebi.isKey && zebi.keyCode != KeyCode.None)
            {
                veritableTouche = zebi.keyCode.ToString();
            }
        }
    }

    public void StartRebinding()
    {
        changementTouche = true;

        // D�sactive la commande qui va �tre modifi�e
        playerController.playerInputActions.FindAction(actionName).Disable();

        // Enl�ve l'image de l'ancienne touche
        imagesDesTouches[indexListe].gameObject.SetActive(false);

        // C'est ici que la commande est modifi�e
        rebindingOperation = playerController.playerInputActions.FindAction(actionName).PerformInteractiveRebinding(indexCommande)


            .WithExpectedControlType("axis")

            .OnComplete
            (operation =>
            {
                toucheNouvelInput = playerController.playerInputActions.FindAction(actionName).
                bindings[indexCommande].ToDisplayString();

                // Active la commande qui vient d'�tre modifi�e
                playerController.playerInputActions.FindAction(actionName).Enable();

                // Vide le bouton s�lectionn� dans l'Event System
                EventSystem.current.SetSelectedGameObject(null);

                // V�rifie si la touche s�lectionn�e est le Scroll
                if (InputControlPath.ToHumanReadableString
                    (playerController.playerInputActions.FindAction(actionName).bindings[indexCommande].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice) == "Scroll/Y")
                {
                    imagesDesTouches[indexListe].sprite = listeTouchesDisponibles.Find(i => i.name == "Scroll");
                    imagesDesTouches[indexListe].gameObject.SetActive(true);
                }

                //Sinon v�rifie s'il s'agit de l'un des deux boutons de la souris
                else if (InputControlPath.ToHumanReadableString
                    (playerController.playerInputActions.FindAction(actionName).bindings[indexCommande].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice) == "Left Button")
                {
                    imagesDesTouches[indexListe].sprite = listeTouchesDisponibles.Find(i => i.name == "Left Button");
                    imagesDesTouches[indexListe].gameObject.SetActive(true);
                }
                else if (InputControlPath.ToHumanReadableString
                    (playerController.playerInputActions.FindAction(actionName).bindings[indexCommande].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice) == "Right Button")
                {
                    imagesDesTouches[indexListe].sprite = listeTouchesDisponibles.Find(i => i.name == "Right Button");
                    imagesDesTouches[indexListe].gameObject.SetActive(true);
                }

                // Sinon v�rifie si la touche existe dans notre base de donn�es
                else if (listeTouchesDisponibles.Exists(i => i.name == veritableTouche))
                {
                    // Si c'est le cas l'image de la nouvelle touche est ajout�e

                    imagesDesTouches[indexListe].sprite = listeTouchesDisponibles.Find(i => i.name == veritableTouche);
                    imagesDesTouches[indexListe].gameObject.SetActive(true);
                }
                else
                {
                    // Si ce n'est pas le cas la commande est rendue inutilisable et son image dispara�t

                    playerController.playerInputActions.FindAction(actionName).ApplyBindingOverride(indexCommande, "");

                    imagesDesTouches[indexListe].gameObject.SetActive(false);
                }

                // V�rifie s'il n'y a pas deux commandes qui ont la m�me touche
                foreach (var item in imagesDesTouches)
                {
                    if (item.sprite == imagesDesTouches[indexListe].sprite)
                    {
                        int doublon = imagesDesTouches.IndexOf(item);

                        // Si c'est le cas l'ancienne commande est rendu inutilisable et son image dispara�t

                        if (doublon != indexListe)
                        {
                            imagesDesTouches[doublon].gameObject.SetActive(false);

                            string ancienneCommande = boutonsDesTouches[doublon].GetComponent<BoutonsCommandes>()
                            .actionName;

                            int indexAncienneCommande = boutonsDesTouches[doublon].GetComponent<BoutonsCommandes>()
                            .indexCommande;

                            playerController.playerInputActions.FindAction(ancienneCommande).
                            ApplyBindingOverride(indexAncienneCommande, "");
                        }
                    }
                }

                // V�rifie si le bouton R�initialiser doit �tre cliquable ou non
                foreach (var item in imagesDesTouches)
                {
                    if (item.sprite == imageDefautDesTouches[imagesDesTouches.IndexOf(item)] &&
                        item.gameObject.activeSelf == true)
                        
                    {
                        textReinitialiser.color = menuEchap.pasCliquable;
                    }
                    else
                    {
                        textReinitialiser.color = menuEchap.gris; break;
                    }
                }

                // V�rifie si le bouton Confirmer doit �tre cliquable ou non
                for (int i = 0; i < 22; i += 1)
                {
                    if (imagesDesTouches[i].sprite == avantConfirmationImage[i] && 
                        imagesDesTouches[i].gameObject.activeSelf == avantConfirmationEtat[i])
                    {
                        textConfirmer.color = menuEchap.pasCliquable;
                    }
                    else
                    {
                        textConfirmer.color = menuEchap.gris; break;
                    }
                }

                changementTouche = false;

                glissade.sprite = imagesDesTouches[6].sprite;
            }
            )

            // Action sp�ciale requise pour les 4 touches de d�placement lorsque le joueur appuie sur la touche �chap
            .OnCancel
            (operation =>
            {
                // Vide le bouton s�lectionn� dans l'Event System
                EventSystem.current.SetSelectedGameObject(null);

                // Active les 4 commandes de d�placement
                playerController.playerInputActions.Player.Movement.Enable();

                // D�sactive la commande s�lectionn�e ainsi que son image
                playerController.playerInputActions.FindAction(actionName).ApplyBindingOverride(indexCommande, "");
                imagesDesTouches[indexListe].gameObject.SetActive(false);
                imagesDesTouches[indexListe].sprite = null;

                // V�rifie si le bouton R�initialiser doit �tre cliquable ou non
                foreach (var item in imagesDesTouches)
                {
                    if (item.sprite == imageDefautDesTouches[imagesDesTouches.IndexOf(item)] &&
                        item.gameObject.activeSelf == true)

                    {
                        textReinitialiser.color = menuEchap.pasCliquable;
                    }
                    else
                    {
                        textReinitialiser.color = menuEchap.gris; break;
                    }
                }

                // V�rifie si le bouton Confirmer doit �tre cliquable ou non
                for (int i = 0; i < 22; i += 1)
                {
                    if (imagesDesTouches[i].sprite == avantConfirmationImage[i] &&
                        imagesDesTouches[i].gameObject.activeSelf == avantConfirmationEtat[i])
                    {
                        textConfirmer.color = menuEchap.pasCliquable;
                    }
                    else
                    {
                        textConfirmer.color = menuEchap.gris; break;
                    }
                }
            }
            )

            .Start();
    }

    public void ClickReinitialiser()
    {
        if (menuEchap.blocageDesBoutons == false)
        {
            if (textReinitialiser.color == menuEchap.turquoise)
            {
                panelReinitialisation.SetActive(true);

                menuEchap.blocageDesBoutons = true;

                textReinitialiser.color = menuEchap.pasCliquable;
            }
        }
    }

    public void EnterReinitialiser()
    {
        if (textReinitialiser.color == menuEchap.gris)
        {
            textReinitialiser.color = menuEchap.turquoise;
        }
    }

    public void ExitReinitialiser()
    {
        if (textReinitialiser.color == menuEchap.turquoise)
        {
            textReinitialiser.color = menuEchap.gris;
        }
    }

    public void ClickConfirmer()
    {
        if (menuEchap.blocageDesBoutons == false)
        {
            if (textConfirmer.color == menuEchap.turquoise)
            {
                textConfirmer.color = menuEchap.pasCliquable;

                // Enregistre tout ce qu'il faut avant une modification des Commandes
                // R�cup�re les images de toutes les commandes avant une quelconque modification
                for (int i = 0; i < 22; i += 1)
                {
                    avantConfirmationImage[i] = imagesDesTouches[i].sprite;
                }

                // R�cup�re les path de toutes les commandes avant une quelconque modification
                for (int i = 0; i < 22; i += 1)
                {
                    string rechercheAction = boutonsDesTouches[i].GetComponent<BoutonsCommandes>().actionName;
                    int rechercheIndexCommande = boutonsDesTouches[i].GetComponent<BoutonsCommandes>().indexCommande;

                    // V�rifie que la commande existe bel et bien
                    if (rechercheAction != "")
                    {
                        avantConfirmationPath[i] =
                            playerController.playerInputActions.
                            FindAction(rechercheAction).bindings[rechercheIndexCommande].effectivePath;
                    }
                }

                // R�cup�re l'�tat des touches avant une quelconque modification
                for (int i = 0; i < 22; i += 1)
                {
                    avantConfirmationEtat[i] = imagesDesTouches[i].gameObject.activeSelf;
                }
            }
        }
    }

    public void EnterConfirmer()
    {
        if (textConfirmer.color == menuEchap.gris)
        {
            textConfirmer.color = menuEchap.turquoise;
        }
    }

    public void ExitConfirmer()
    {
        if (textConfirmer.color == menuEchap.turquoise)
        {
            textConfirmer.color = menuEchap.gris;
        }
    }

    public void NonConfirmation()
    {
        for (int i = 0; i < 22; i += 1)
        {
            imagesDesTouches[i].sprite = avantConfirmationImage[i];

            string rechercheActionName = boutonsDesTouches[i].GetComponent<BoutonsCommandes>().actionName;
            int rechercheIndexCommande = boutonsDesTouches[i].GetComponent<BoutonsCommandes>().indexCommande;

            if (rechercheActionName != "")
            {
                playerController.playerInputActions.FindAction(rechercheActionName).
                ApplyBindingOverride(rechercheIndexCommande, avantConfirmationPath[i]);
            }

            if (avantConfirmationEtat[i] == true)
            {
                imagesDesTouches[i].gameObject.SetActive(true);
            }
            else
            {
                imagesDesTouches[i].gameObject.SetActive(false);
            }
        }

        textConfirmer.color = menuEchap.pasCliquable;

        // V�rifie si le bouton R�initialiser doit �tre cliquable ou non
        foreach (var item in imagesDesTouches)
        {
            if (item.sprite == imageDefautDesTouches[imagesDesTouches.IndexOf(item)] &&
                item.gameObject.activeSelf == true)

            {
                textReinitialiser.color = menuEchap.pasCliquable;
            }
            else
            {
                textReinitialiser.color = menuEchap.gris; break;
            }
        }

        glissade.sprite = imagesDesTouches[6].sprite;
    }
}
