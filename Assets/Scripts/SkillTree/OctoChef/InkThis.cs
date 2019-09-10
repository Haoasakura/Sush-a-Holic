using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkThis : Skill
{
    public InkThis()
    {
        skill_name = "Ink This!";
        description = "Increases the damage dealt by Purple Rain's poison";
        cost = 30;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.purpleRainPoisonDamage = ConstantsDictionary.InkThisPoisonDamage;
    }
}