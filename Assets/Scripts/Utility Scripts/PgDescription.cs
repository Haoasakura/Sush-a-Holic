using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PgDescription : MonoBehaviour {

    public GameObject[] descriptions;
    private int current = 0;

    public void NextDescription()
    {
        descriptions[current].SetActive(false);
        current++;
        if (current >= 3)
        {
            current = 0;
        }
        descriptions[current].SetActive(true);
    }
    public void PreviousDescription()
    {
        descriptions[current].SetActive(false);
        current--;
        if (current < 0)
        {
            current = 2;
        }
        descriptions[current].SetActive(true);
    }
}
