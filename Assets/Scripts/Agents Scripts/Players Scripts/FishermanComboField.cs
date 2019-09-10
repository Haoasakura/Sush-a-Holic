using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FishermanComboField : NetworkBehaviour
{

    private void OnEnable()
    {
        Invoke("Destroy", ConstantsDictionary.comboFieldDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            PlayerAttacks typeOfPlayer = collision.gameObject.GetComponent<PlayerAttacks>();
            if (!(typeOfPlayer is FishermanAttacks))
            {
                typeOfPlayer.isInFishermanComboField = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            collision.gameObject.GetComponent<PlayerAttacks>().isInFishermanComboField = false;
        }
    }

    private void Destroy()
    {
        PlayerAttacks[] players = GameObject.FindObjectsOfType<PlayerAttacks>();
        foreach (PlayerAttacks player in players)
        {
            player.isInFishermanComboField = false;
        }
        NetworkServer.Destroy(gameObject);
    }

}
