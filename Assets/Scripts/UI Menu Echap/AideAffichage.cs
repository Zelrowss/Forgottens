using UnityEngine;
using UnityEngine.EventSystems;

public class AideAffichage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Affichage affichage;
    public int index;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        affichage.textesAide[index].SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        affichage.textesAide[index].SetActive(false);
    }
}
