using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill{

    public string skill_name;
    public string description;
    public int cost;
    public bool unlocked;
    public List<Skill> previousSkills;
    public string iconPath = "/Images/skill_icon";

    public abstract void SkillEffect(GameObject player);

    public void Unlock(GameObject player)
    {
        SkillEffect(player);
        unlocked = true;
    }

    public bool CanBeUnlocked()
    {
		if (previousSkills == null)
			return true;
        foreach(Skill skill in previousSkills){
            if (skill.unlocked)
            {
                return true;
            }
        }

        return false;
    }
}
