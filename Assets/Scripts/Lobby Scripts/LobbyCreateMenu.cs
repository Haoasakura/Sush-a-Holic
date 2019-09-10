using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyCreateMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform createRoom;
        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;
        
        public InputField matchNameInput;
        public InputField matchPasswordInput;
        public InputField ipInput;

        public Button CampainButton;

        public void OnEnable()
        {
            lobbyManager.escapePanel.ToggleVisibility(false);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameInputFields);

            matchPasswordInput.onEndEdit.RemoveAllListeners();
            matchPasswordInput.onEndEdit.AddListener(onEndEditGameInputFields);


        }

        public void OnClickHost()
        {
            lobbyManager.StartHost();
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();
            
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;
            
        }

        public void OnClickCreateMatchmakingGame()
        {
            string matchName = "";
            lobbyManager.StartMatchMaker();
            if (CampainButton.IsInteractable())
                matchName = matchNameInput.text + " Endless";
            else
                matchName = matchNameInput.text + " Campaign";

            lobbyManager.matchMaker.CreateMatch(matchName, (uint)lobbyManager.maxPlayers,true, matchPasswordInput.text, "", "", 0, 0,lobbyManager.OnMatchCreate);

            lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager._isMatchmaking = true;
            lobbyManager.DisplayIsConnecting();
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        public void OnClickOpenCreateRoom()
        {
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(createRoom);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameInputFields(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }
    }
}
