using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LingeringToxin : Skill
{
    public LingeringToxin()
    {
        skill_name = "Lingering Toxin";
        description = "Increases the duration of Purple Rain's poison";
        cost = 55;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.purpleRainPoisonDuration = ConstantsDictionary.LigeringToxinPoisonDuration;
    }
}

