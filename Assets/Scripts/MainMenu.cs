using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField nameChanger;

    public void Quit(){
        Application.Quit();
    }

    public void BTN_ChooseUserName(){
        ClientSend.SendDataToServer("ChoseUserName", nameChanger.text);
        SceneManager.LoadScene("SceneTest");
    }

}
