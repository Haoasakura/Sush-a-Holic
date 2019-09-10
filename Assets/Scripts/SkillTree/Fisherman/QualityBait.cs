using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityBait : Skill {

    public QualityBait()
    {
        skill_name = "Quality Bait";
        description = "Increases the area in which enemies are forced to target the Fisherman in I Got Ya!";
        cost = 55;
        unlocked = false;
    }

    public override void SkillEffect(GameObject player)
    {
        FishermanAttacks fisherman = player.GetComponent<FishermanAttacks>();
        fisherman.iGotYaDmgArea += ConstantsDictionary.QualityBaitIncreaseInArea;
    }
}
