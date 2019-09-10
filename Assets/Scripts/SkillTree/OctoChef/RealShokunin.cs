using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealShokunin : Skill
{
    public RealShokunin()
    {
        skill_name = "Real Shokunin";
        description = "Increases the damage of Hack'n'Sushi by "+ConstantsDictionary.RealShokuninIncreaseInDamage+"%";
        cost = 15;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.hackNSushiIncreaseInDamage += ConstantsDictionary.RealShokuninIncreaseInDamage;
    }
}
