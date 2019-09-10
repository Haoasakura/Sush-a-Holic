using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyTentacles : Skill
{
    public SlipperyTentacles()
    {
        skill_name = "Slippery Tentacles";
        description = "Increases speed of movement by "+ConstantsDictionary.SlipperyTentaclesIncreaseInSpeed+"% ";
        cost = 10;
        unlocked = false;
    }

    public new bool CanBeUnlocked()
    {
        return true;
    }

    public override void SkillEffect(GameObject player)
    {
        PlayerController octo = player.GetComponent<PlayerController>();
        octo.m_normal_speed += (octo.m_normal_speed * ConstantsDictionary.SlipperyTentaclesIncreaseInSpeed )/ 100;
    }
}
