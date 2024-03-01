using UnityEngine;
using UnityEngine.EventSystems;

public class CloseDropdown : MonoBehaviour, IPointerExitHandler
{
    public Affichage affichage;

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);

        gameObject.SetActive(false);

        foreach (var item in affichage.ouvrirDropdowns)
        {
            item.SetActive(true);
        }

        foreach (var item in affichage.titres)
        {
            item.color = Color.white;
        }

        foreach (var item in affichage.infos)
        {
            item.color = Color.white;
        }
    }

    //Clair : 24ebcb
    //Foncé : 1ba18b
    //Gris : 677A7C
}
