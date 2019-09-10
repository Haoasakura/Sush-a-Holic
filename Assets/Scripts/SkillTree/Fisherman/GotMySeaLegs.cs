using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotMySeaLegs : Skill
{
    public GotMySeaLegs()
    {
        skill_name = "Got My Sea Legs";
        description = "The fisherman evades "+(ConstantsDictionary.GotMySeasLegsEvadedAttacksPercentage) +"% of the enemy attacks";
        cost = 55;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        health.evadedAttacksPercentage = ConstantsDictionary.GotMySeasLegsEvadedAttacksPercentage;
        health.hasSeaLegs = true;
    }
}