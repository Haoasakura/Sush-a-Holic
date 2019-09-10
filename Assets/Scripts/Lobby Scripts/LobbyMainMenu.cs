using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace Prototype.NetworkLobby
{
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform createRoom;
        public RectTransform lobbyServerList;

        public GameObject muteButton;
        public Text connectionStatus;
        public Sprite mute_Off;
        public Sprite mute_On;

        private bool isConnected;

        public void OnEnable()
        {
            lobbyManager.escapePanel.ToggleVisibility(false);
        }

        private void Start()
        {
            StartCoroutine(checkInternetConnection((isConnected) => SetConnectionText(isConnected)));
        }

        private void Update()
        {
            if (!isConnected)
                StartCoroutine(checkInternetConnection((isConnected) => SetConnectionText(isConnected)));
        }

        public void OnClickOpenCreateRoom()
        {
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(createRoom);
        }


        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
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

        public void onClickExitGame()
        {
            Application.Quit();
        }


    }
}
