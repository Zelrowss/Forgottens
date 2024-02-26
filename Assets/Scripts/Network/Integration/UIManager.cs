using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// #############################################################################
/// ################## Script à placer sur ta scene principale ##################
/// #############################################################################
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public TMP_InputField usernameField;
    public TMP_InputField adress;
    public TMP_InputField port;


    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    public void ConnectToServer()
    {
        Client.instance.ip = adress.text;
        Client.instance.port = int.Parse(port.text);

        startMenu.SetActive(false);
        usernameField.interactable = false;
        Client.instance.ConnectToServer();
    }
}
