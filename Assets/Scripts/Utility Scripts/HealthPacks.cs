using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPacks : NetworkBehaviour {

    public float healthPercentage;
    public HealthPacksManager manager;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.player))
        {
            PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
            if((player.currentHealth > 0) && (player.currentHealth < player.maxHealth))
            {
                player.Heal(0, true, healthPercentage);
                manager.StartCoroutine("Respawn", this.gameObject);
                //manager.CmdRespawnHealthPack(gameObject);
                //CmdHealAndRespawn(player.gameObject, healthPercentage);
            }
        }
    }

    [Command]
    public void CmdHealAndRespawn(GameObject player, float percentage)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        manager.StartCoroutine("Respawn", this.gameObject);
    }

    /*public override void OnNetworkDestroy() {
        base.OnNetworkDestroy();
        Debug.Log("Health Picked");
        Destroy(gameObject);
    }*/
}
