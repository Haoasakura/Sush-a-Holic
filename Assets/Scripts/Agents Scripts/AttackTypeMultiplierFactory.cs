using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypeMultiplierFactory : MonoBehaviour {

    public static float SelectAttackTypeMultiplier(AttackTypes attackType, EnemyTypesEnum enemyType)
    {
        //TODO finire con tutti i tipi di nemici
        if(enemyType == EnemyTypesEnum.Hamburger)
        {

        }

        return 1f;
    }
}
