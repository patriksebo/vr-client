using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject menu;
    public GameObject lobby;
    public GameObject message;
    public InputField usernameField;
    public InputField avatarUrlField;
    public Dropdown avatar;
    public Dropdown arvr;
    public Dropdown environment;
    public Dropdown microphones;
    public Text lobbynames;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        avatar.onValueChanged.AddListener(delegate {
            DropdownValueChanged(avatar);
        });
    }

    public void ConnectToServer()
    {
        ErrorMessage();
        Client.instance.ConnectToServer();
    }

    public void Deploy()
    {
        Client.instance.Deploy(environment.value);
    }

    public void MenuToLobby()
    {
        menu.SetActive(false);
        lobby.SetActive(true);
    }

    public void ErrorMessage()
    {
        message.SetActive(true);
    }
    public void ErrorMessageClose()
    {
        message.SetActive(false);
    }
    
    void DropdownValueChanged(Dropdown dropdown)
    {
        if (dropdown.value == 3)
        {
            avatarUrlField.gameObject.SetActive(true);
        }
        else
        {
            avatarUrlField.gameObject.SetActive(false);
        }
    }
}
