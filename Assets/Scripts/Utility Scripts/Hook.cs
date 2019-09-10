using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Hook : NetworkBehaviour {

    public Transform playerToReach;
    public float velocity = 10f;
    private Transform tr;

    private void Start()
    {
        tr = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D coll in hitColliders)
        {
            if (coll.gameObject == playerToReach.gameObject)
            {
                //TODO effetto animazione qui
                NetworkServer.Destroy(gameObject);
                return;
            }
        }

        Vector3 direction = (playerToReach.position - tr.position).normalized;
        tr.position = transform.position + (direction * velocity * Time.deltaTime);
    }
}
