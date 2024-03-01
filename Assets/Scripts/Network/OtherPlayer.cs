using TMPro;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public GameObject UserNameRotation; //Comporte le pseudo pour la rotation
    public TMP_Text UserNameTag; //Pseudo de ce joueur
    public Animator animator;
    Transform player; //Player Local
    public PlayerNetManager actualPlayer; //Informations de ce player
    public GameObject PlayerSkin; //Skin de ce player

    Vector3 LastEuler;
    Vector3 OldPos;
    Vector2 deltaPos2DLerp;


    private void Start()
    {
        player = Camera.main.transform;
    }

    private void Update()
    {

        //  Check de distance entre le local Player et ce player
        //  pour activer ou désactiver son skin ou son pseudo.
        //  Permet aussi d'afficher le UserName et l'orienté
        if (player != null && actualPlayer != null)
        {
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist < 10f)
            {
                UserNameRotation.SetActive(true);
                UserNameTag.text = actualPlayer.username;
                UserNameRotation.transform.LookAt(player);
            }
            else UserNameRotation.SetActive(false);

            
            if (dist < 150f) PlayerSkin.SetActive(true);
            else PlayerSkin.SetActive(false);

        }
        else UserNameRotation.SetActive(false);




        //  Regarde le déplacement d'un joueur avec sa vitesse
        //  pour le convertir en float pour l'animation qui
        //  convient avec la bonne vitesse
        if (animator != null)
        {
            Vector3 deltaPos = OldPos - transform.position;

            Vector3 moveDirection = transform.InverseTransformDirection(deltaPos);
            moveDirection /= -Time.deltaTime;
            Vector2 deltaPos2D = new Vector2(Mathf.Round(moveDirection.x), Mathf.Round(moveDirection.z));
            deltaPos2DLerp = Vector2.Lerp(deltaPos2DLerp, deltaPos2D, 2 * Time.deltaTime);
            animator.SetFloat("Vertical", deltaPos2DLerp.y);
            OldPos = transform.position;

            var angle = transform.eulerAngles.y - LastEuler.y;
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            angle /= -Time.deltaTime * 40;
            animator.SetFloat("Horizontal", -angle);
            LastEuler = transform.eulerAngles;
        }
    }
}
