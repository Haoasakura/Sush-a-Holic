using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotatingOcto : MonoBehaviour
{
    public float rotation_speed = 3f;

    void Update()
    {
        GetComponent<RectTransform>().Rotate(0f,0f,-rotation_speed);
    }
}
