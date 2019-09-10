using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickSkin : Skill
{
    public ThickSkin()
    {
        skill_name = "Thick Skin";
        description = "Reduces damage taken of"+(ConstantsDictionary.ThickSkinDamageReductionPercentage*100)+"%";
        cost = 10;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.hasThickSkin = true;
    }
}
