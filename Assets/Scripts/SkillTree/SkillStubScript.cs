using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.
using UnityEngine.UI;

public class SkillStubScript : MonoBehaviour {

	private SkillTreeManager stm;

	public void SetSTM(SkillTreeManager s)
	{
		stm = s;
	}

	/*
	public void SelectSkill(int num)
	{
		stm.skillNumber = num;
	}
	*/

	public void UnlockMe(int num)
	{
		stm.skillNumber = num;
		stm.UnlockSkill ();
	}

	public void ShowText(int num)
	{
		string desc = stm.GetSkillDescription(num);
		GameObject descBg = gameObject.transform.Find ("DescriptionBackground").gameObject;
		Text skillDesc = descBg.transform.Find("SkillDescription").gameObject.GetComponent<Text>();
		skillDesc.text = desc;
		descBg.SetActive (true);
	}

	public void HideText()
	{
		GameObject descBg = gameObject.transform.Find ("DescriptionBackground").gameObject;
		descBg.SetActive (false);
	}
}
