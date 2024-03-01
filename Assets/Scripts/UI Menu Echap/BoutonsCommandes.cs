using UnityEngine;

public class BoutonsCommandes : MonoBehaviour
{
    public Commandes commandes;

    public int indexCommande;
    public int indexListe;
    public string actionName;

    public void PreparationRebinding()
    {
        commandes.indexListe = indexListe;
        commandes.indexCommande = indexCommande;
        commandes.actionName = actionName;

        commandes.StartRebinding();
    }
}
