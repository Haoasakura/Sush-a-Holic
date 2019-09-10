using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlessingOfTheArk : Skill
{
    public BlessingOfTheArk()
    {
        skill_name = "Blessing Of The Ark";
        description = "Increases duration of Sushi's Divine Protection";
        cost = 65;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.sushiDivineProtectionDuration += ConstantsDictionary.BlessingOfTheArkDurationIncrease;
    }
}