using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyEscapePanel : MonoBehaviour
    {
        public bool isInGame = true;

        protected bool isDisplayed = true;
        public Image panelImage;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }


        void Update()
        {
            if (!isInGame)
                return;

            if (Input.GetKeyDown(KeyCode.P))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}