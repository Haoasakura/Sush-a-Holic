using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyHook : Skill
{
    public HeavyHook()
    {
        skill_name = "Heavy Hook";
        description = "The enemy selected during I Got Ya! is also stunned";
        cost = 30;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.iGotYaStun = true;
    }
}