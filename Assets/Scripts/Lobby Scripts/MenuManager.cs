using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour {

    public GameObject muteButton;
    public Text connectionStatus;
    public Sprite mute_Off;
    public Sprite mute_On;

    private bool isConnected;

    void Start () {
        StartCoroutine(checkInternetConnection((isConnected) =>SetConnectionText(isConnected)));
    }

    void Update()
    {
        if(!isConnected)
            StartCoroutine(checkInternetConnection((isConnected) => SetConnectionText(isConnected)));
    } 

    public void OnClickMuteButton()
    {
        if (muteButton.GetComponent<Image>().sprite == mute_Off)
        {
            muteButton.GetComponent<Image>().sprite = mute_On;
        }
        else
        {
            muteButton.GetComponent<Image>().sprite = mute_Off;
        }
    }

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    private void SetConnectionText(bool isConnected)
    {
        if (isConnected)
        {
            connectionStatus.text = "Connected";
            this.isConnected = true;
        }
        else
        {
            connectionStatus.text = "Not Connected";
            this.isConnected = false;
        }
    }

    public void OnCloseGameButton()
    {
        Application.Quit();
    }

}
