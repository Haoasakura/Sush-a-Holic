using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBeakIsBigger : Skill
{
    public MyBeakIsBigger()
    {
        skill_name = "My Beak Is Bigger";
        description = "Increases the area in which enemies are poisoned by Purple Rain";
        cost = 15;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.purpleRainPoisonArea = ConstantsDictionary.MyBeakIsBiggerPoisonArea;
    }
}