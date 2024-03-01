using UnityEngine;

public class Démarrage : MonoBehaviour
{
    public int width, height;

    private void Start()
    {
        width = Display.main.systemWidth; 
        height = Display.main.systemHeight;

        Screen.SetResolution(1920, 1080, true);
    }
}
