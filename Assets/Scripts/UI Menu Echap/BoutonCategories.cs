using System.ComponentModel.Design;
using UnityEngine;

public class BoutonCategories : MonoBehaviour
{
    [Header("References")] [Space(10)]
    public Options options;
    public MenuEchap menuEchap;

    public int index;

    private void Start()
    {
        index = options.categories.FindIndex(i => i.name == gameObject.name);
    }

    public void MouseEnter()
    {
        options.hoveredBar[index].gameObject.SetActive(true);
    }

    public void MouseExit()
    {
        options.hoveredBar[index].gameObject.SetActive(false);
    }

    public void MouseClick()
    {
        if (options.commandes.textConfirmer.color == menuEchap.pasCliquable &&
            options.affichage.textConfirmer.color == menuEchap.pasCliquable)
        {
            ExecutionBoutonCategories();
        }

        else
        {
            if (options.commandes.textConfirmer.color == menuEchap.gris)
            {
                menuEchap.blocageDesBoutons = true;
                options.commandes.panelModification.SetActive(true);
                options.commandes.enregistrementDuClic = "Catégorie N° " + index;
            }
            else if (options.affichage.textConfirmer.color == menuEchap.gris)
            {
                menuEchap.blocageDesBoutons = true;
                options.affichage.panelConfirmation.SetActive(true);
                options.affichage.enregistrementDuClic = "Catégorie N° " + index;
            }
        }
    }

    public void ExecutionBoutonCategories()
    {
        foreach (var item in options.selectedBar)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in options.text)
        {
            item.color = menuEchap.gris;
        }

        foreach (var item in options.contenu)
        {
            item.SetActive(false);
        }

        options.text[index].color = menuEchap.turquoise;
        options.selectedBar[index].gameObject.SetActive(true);
        options.contenu[index].SetActive(true);

        options.scrollbars[index].value = 1;

        // Enregistre tout ce qu'il faut avant une modification de l'Affichage
        if (options.affichage.gameObject.activeSelf == true)
        {
            for (int i = 0; i < 12; i += 1)
            {
                options.affichage.avantConfirmationInfos[i] = options.affichage.infos[i].text;
            }
        }

        // Enregistre tout ce qu'il faut avant une modification des Commandes
        if (options.commandes.gameObject.activeSelf == true)
        {
            // Récupére les images de toutes les commandes avant une quelconque modification
            for (int i = 0; i < 22; i += 1)
            {
                options.commandes.avantConfirmationImage[i] = options.commandes.imagesDesTouches[i].sprite;
            }

            // Récupére les path de toutes les commandes avant une quelconque modification
            for (int i = 0; i < 22; i += 1)
            {
                string rechercheAction = options.commandes.boutonsDesTouches[i].GetComponent<BoutonsCommandes>().actionName;
                int rechercheIndexCommande = options.commandes.boutonsDesTouches[i].GetComponent<BoutonsCommandes>().indexCommande;

                // Vérifie que la commande existe bel et bien
                if (rechercheAction != "")
                {
                    options.commandes.avantConfirmationPath[i] =
                        options.commandes.playerController.playerInputActions.
                        FindAction(rechercheAction).bindings[rechercheIndexCommande].effectivePath;
                }
            }

            // Récupére l'état des touches avant une quelconque modification
            for (int i = 0; i < 22; i += 1)
            {
                options.commandes.avantConfirmationEtat[i] = options.commandes.imagesDesTouches[i].gameObject.activeSelf;
            }
        }
    }
}
