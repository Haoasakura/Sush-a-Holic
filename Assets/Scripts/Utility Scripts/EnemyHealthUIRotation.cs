using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthUIRotation : MonoBehaviour {

    public Quaternion relativeRotation;
    private Vector3 relativePosition;

	void Start () {
        relativeRotation = transform.parent.localRotation;
	}
	
	void Update () {
        transform.rotation = relativeRotation ;
	}
}
