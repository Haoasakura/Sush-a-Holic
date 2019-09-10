using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Prototype.NetworkLobby 
{
    public class LobbyInfoPanel : MonoBehaviour
    {
        public Text infoText;
        public Text buttonText;
        public Button singleButton;

        public LobbyManager lobbyManager;
        public RectTransform mainMenu;

        public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk)
        {
            infoText.text = info;

            buttonText.text = buttonInfo;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { /*lobbyManager.ChangeTo(mainMenu);*/ gameObject.SetActive(false); });

            gameObject.SetActive(true);
        }
    }
}