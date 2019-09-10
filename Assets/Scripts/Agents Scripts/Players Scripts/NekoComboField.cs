using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NekoComboField : NetworkBehaviour {


    private void OnEnable()
    {
        Invoke("Destroy", ConstantsDictionary.comboFieldDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            PlayerAttacks typeOfPlayer = collision.gameObject.GetComponent<PlayerAttacks>();
            if(!(typeOfPlayer is NekoMaidAttacks))
            {
                typeOfPlayer.isInNekoComboField = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            collision.gameObject.GetComponent<PlayerAttacks>().isInNekoComboField = false;
        }
    }


    private void Destroy()
    {
        PlayerAttacks[] players =  GameObject.FindObjectsOfType<PlayerAttacks>();
        foreach(PlayerAttacks player in players)
        {
            player.isInNekoComboField = false;
        }
        NetworkServer.Destroy(gameObject);
    } 
}
