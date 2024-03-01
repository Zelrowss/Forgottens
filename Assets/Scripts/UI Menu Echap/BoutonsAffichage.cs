using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class BoutonsAffichage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Affichage affichage;

    public TMP_Text text, textTitre, textInfo;
    public string nomDuBouton;

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        affichage.nomDuBouton = nomDuBouton;
        affichage.ClickDesBoutons();

        if (text != null)
        {
            text.color = Color.white;
        }
        if (textTitre != null && nomDuBouton != "VSYNC")
        {
            textTitre.color = Color.white;
        }
        if (textInfo != null && nomDuBouton != "VSYNC")
        {
            textInfo.color = Color.white;
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (text != null)
        {
            text.color = Color.black;
        }
        if (textTitre != null)
        {
            textTitre.color = Color.black;
        }
        if (textInfo != null)
        {
            textInfo.color = Color.black;
        }
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (text != null)
        {
            text.color = Color.white;
        }
        if (textTitre != null)
        {
            textTitre.color = Color.white;
        }
        if (textInfo != null)
        {
            textInfo.color = Color.white;
        }
    }
}
