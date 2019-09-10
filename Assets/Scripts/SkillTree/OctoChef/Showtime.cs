using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Showtime : Skill
{
    public Showtime()
    {
        skill_name = "Showtime!";
        description = "Increases damage and duration of Sushi Disco Dance";
        cost = 65;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        OctoChefAttacks octo = player.GetComponent<OctoChefAttacks>();
        octo.sushiDiscoDancePercentageDamageIncrease = ConstantsDictionary.ShowtimeDiscoDancePercentageDamageIncrease;
        octo.sushiDiscoDanceDuration = ConstantsDictionary.ShowTimeDiscoDanceDuration;
    }
}