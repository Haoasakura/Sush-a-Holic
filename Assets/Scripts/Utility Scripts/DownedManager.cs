using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DownedManager : NetworkBehaviour {

    [SyncVar]
    public int nekoAlive = 2;
    [SyncVar]
    public int octoAlive = 2;
    [SyncVar]
    public int fishermanAlive = 2;

    [SyncVar]
    public float nekoDownedTimer = 0;
    [SyncVar]
    public float octoDownedTimer = 0;
    [SyncVar]
    public float fishermanDownedTimer = 0;

    public PlayerController neko;
    public PlayerController octo;
    public PlayerController fisherman;

    public float timeToRespawn = 5f;

    public void AddPlayer(bool isNeko, bool isOcto, bool isFish)
    {
        if (isNeko)
        {
            nekoAlive = 1;
            return;
        }
        if (isOcto)
        {
            octoAlive = 1;
            return;
        }
        if (isFish)
        {
            fishermanAlive = 1;
            return;
        }
    }

    public IEnumerator NekoDownedTimer()
    {

        while (nekoDownedTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            nekoDownedTimer -= 1;
        }

        neko.RpcDie();
        nekoAlive = 2;
        //StartCoroutine("ReviveAll");

    }

    public IEnumerator OctoDownedTimer()
    {

        while (octoDownedTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            octoDownedTimer -= 1;
        }

        octo.RpcDie();
        octoAlive = 2;
        //StartCoroutine("ReviveAll");

    }

    public IEnumerator FishDownedTimer()
    {

        while (fishermanDownedTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            fishermanDownedTimer -= 1;
        }

        fisherman.RpcDie();
        fishermanAlive = 2;
        //StartCoroutine("ReviveAll");

    }
    [ClientRpc]
    public void RpcChangeDowned(bool isNeko, bool isOcto, bool isFish, int value)
    {
        if (isLocalPlayer)
        {
            if (isNeko)
            {
               nekoAlive = value;
            }
            if (isOcto)
            {
               octoAlive = value;
            }
            if (isFish)
            {
                fishermanAlive = value;

            }
        }

    }

    private IEnumerator ReviveAll()
    {
        if ((nekoAlive == 0 || nekoAlive == 2) && (octoAlive == 0 || octoAlive == 2) && (fishermanAlive == 0 || fishermanAlive == 2))
        {
            StopCoroutine("FishDownedTimer");
            StopCoroutine("OctoDownedTimer");
            StopCoroutine("NekoDownedTimer");
            yield return new WaitForSeconds(timeToRespawn);
            RpcChangeDowned(true, true, true, 1);
            if (neko != null)
                neko.RpcReviveFromDead();
            if (octo != null)
                octo.RpcReviveFromDead();
            if (fisherman != null)
                fisherman.RpcReviveFromDead();
        }
    }

}
