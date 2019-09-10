using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPacksManager : NetworkBehaviour {

    public GameObject yellowRamen;
    public GameObject orangeRamen;
    public GameObject greenRamen;


    [SerializeField]
    public GameObject[] spawns;

    [SerializeField]
    public int[] healthPacksTypes;

    [SerializeField]
    private GameObject[] healthPacks;

    // Use this for initialization
    void Start () {
        healthPacks = new GameObject[spawns.Length];
		for(int i = 0; i< spawns.Length; i++)
        {
            GameObject healthPack = getHealthPack(healthPacksTypes[i]);
            healthPack.transform.position = spawns[i].transform.position;
            healthPack.GetComponent<HealthPacks>().manager = this;
            healthPacks[i] = healthPack;
            CmdSpawnHealthPack(healthPack);


        }
	}
    [Command]
    public void CmdSpawnHealthPack(GameObject healthPack) {
        NetworkServer.Spawn(healthPack);
    }

    private GameObject getHealthPack(int type)
    {
        if(type == 0)
        {
            return GameObject.Instantiate(yellowRamen);
        }
        if (type == 1)
        {
            return GameObject.Instantiate(orangeRamen);
        }
        if (type == 2)
        {
            return GameObject.Instantiate(greenRamen);
        }
        return null;
    }


    public void RespawnHealthPack(GameObject healthPack) {
        CmdRespawnHealthPack(healthPack);
    }

    [Command]
    public void CmdRespawnHealthPack(GameObject healthPack)
    {
        StartCoroutine("Respawn", healthPack);
    }

    public IEnumerator Respawn(GameObject healthPack)
    {
        int index = System.Array.IndexOf(healthPacks, healthPack);
        NetworkServer.Destroy(healthPack);
        yield return new WaitForSeconds(ConstantsDictionary.RespawnTime);
        GameObject newHealthPack = getHealthPack(healthPacksTypes[index]);
        newHealthPack.transform.position = spawns[index].transform.position;
        newHealthPack.GetComponent<HealthPacks>().manager = this;
        healthPacks[index] = newHealthPack;
        NetworkServer.Spawn(newHealthPack);
    }

}
