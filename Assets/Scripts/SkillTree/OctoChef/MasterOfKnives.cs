using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterOfKnives : Skill
{
    public MasterOfKnives()
    {
        skill_name = "Master Of Knives";
        description = "Increases the number of knives thrown in Hack'n'Sushi";
        cost = 30;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.hackNSushiBeforeMasterOfKnives = false;
    }
}
