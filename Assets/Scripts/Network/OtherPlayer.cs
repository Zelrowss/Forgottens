using TMPro;
using UnityEngine;

public class OtherPlayer : MonoBehaviour
{
    public GameObject UserNameRotation; //Le GameObject qui comporte le pseudo
    public TMP_Text UserNameTag; //Le pseudo affiché
    public Animator animator;
    Transform player; //Player
    public PlayerNetManager actualPlayer;
    public GameObject PlayerSkin;

    Vector3 LastEuler;
    Vector3 OldPos;
    Vector2 deltaPos2DLerp;


    private void Start()
    {
        player = Camera.main.transform;
    }

    private void Update()
    {
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
            LastEuler = transform.eulerAngles;
            angle /= -Time.deltaTime * 40;
            animator.SetFloat("Horizontal", -angle);
        }
    }
}
