using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearOfTheCat : Skill
{
    public FearOfTheCat()
    {
        skill_name = "Fear Of The Cat";
        description = "Increases the duration of Nyaa~ Scream's fear";
        cost = 55;
        unlocked = false;
    }


    public override void SkillEffect(GameObject player)
    {
        NekoMaidAttacks neko = player.GetComponent<NekoMaidAttacks>();
        neko.nyaaScreamFearDuration += ConstantsDictionary.FearOfTheCatNyaaScreamDurationIncrease;

    }

}
