using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SushiCoin : NetworkBehaviour {

    public int amount;


    private void OnEnable()
    {
        Invoke("Destroy", ConstantsDictionary.DurationTime+100f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        
        if (collision.gameObject.CompareTag(Tags.player)) {
            if (!collision.gameObject.GetComponent<PlayerController>().isLocalPlayer)
                return;
            collision.gameObject.GetComponent<PlayerController>().AddSushiCoins(amount);
            NetworkServer.Destroy(gameObject);
        }
    }

    private void Destroy()
    {
        NetworkServer.Destroy(gameObject);
    }
}
