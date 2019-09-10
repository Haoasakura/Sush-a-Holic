using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItsGoodForYou : Skill
{
    public ItsGoodForYou()
    {
        skill_name = "It's Good For You";
        description = "Increases the duration of Omega-3 Protection";
        cost = 15;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.omega3ProtectionDuration += ConstantsDictionary.ItsGoodForYouOmega3DurationIncrease;
    }
}