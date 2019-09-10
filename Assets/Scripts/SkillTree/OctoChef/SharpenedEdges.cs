using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpenedEdges : Skill
{
    public SharpenedEdges()
    {
        skill_name = "Sharpened Edges";
        description = "Knives thrown by Hack'n'Sushi disappear only after hitting the second enemy";
        cost = 55;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.enemiesHitWithKnife = ConstantsDictionary.SharpenedEdgesEnemiesHit;
    }
}