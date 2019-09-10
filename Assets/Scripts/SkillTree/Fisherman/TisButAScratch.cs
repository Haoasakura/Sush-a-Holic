using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TisButAScratch : Skill
{
    public TisButAScratch()
    {
        skill_name = "'Tis But A Scratch";
        description = "Increases the fisherman's HP";
        cost = 20;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.maxHealth += ConstantsDictionary.TisButAScratchHealthIncrease;
    }
}