using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReelItIn : Skill
{
    public ReelItIn()
    {
        skill_name = "Reel It In!";
        description = "Increases the range of the hook of I Got Ya!";
        cost = 15;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.iGotYaHookRange = ConstantsDictionary.ReelItInHookRange;
    }
}