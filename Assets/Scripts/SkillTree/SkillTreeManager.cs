using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour {

	private List<Skill> skillTree;
	private PlayerController pc;
	private GameObject player;
	private GameObject[] skillStubs = new GameObject[9];
	private GameObject notification;
	private Image notificationImage;
	public int skillNumber;

	void Start()
	{
		notification = GameObject.Find ("SkillAvailableNotification");
		notificationImage = notification.GetComponent<Image> ();
	}

	public void CheckForAvailableSkill()
	{
		foreach(Skill s in skillTree){
			if(!s.unlocked && s.CanBeUnlocked() && pc.HasEnoughSushiCoins(s.cost) && notificationImage.color.a == 0f){
				notificationImage.color = new Color (notificationImage.color.r, notificationImage.color.g, notificationImage.color.b, 255f);
			}
		}
	}

	public void AssignToSkillTreeManager(List<Skill> tree, PlayerController owner)
	{
		skillTree = tree;
		pc = owner;
		player = owner.gameObject;
	}
		
	//Il button della GUI chiama questo metodo passandogli
	//il numero corrispondente alla skill che devono sbloccare
	//settata con il SelectSkill cliccando la skill corrispondente
	public void UnlockSkill()
	{
		if (skillNumber == -1) {
			return;
		}
		else
		{
			Skill current = skillTree [skillNumber];

			if(!current.unlocked && current.CanBeUnlocked())
			{
				if(pc.HasEnoughSushiCoins (current.cost))
				{
					pc.SpendSushiCoins (current.cost);
					current.Unlock (player);
					Debug.Log ("unlock successful");
					GameObject unlockedShade = skillStubs[skillNumber].transform.Find ("UnlockedShade").gameObject;
					GameObject unlockedText = skillStubs[skillNumber].transform.Find ("UnlockedText").gameObject;
					GameObject costText = skillStubs [skillNumber].transform.Find ("CostText").gameObject;
					GameObject sushiIcon = skillStubs [skillNumber].transform.Find ("SushiImage").gameObject;
					unlockedShade.SetActive (true);
					unlockedText.SetActive (true);
					costText.SetActive (false);
					sushiIcon.SetActive (false);
					skillStubs [skillNumber].GetComponent <Button> ().enabled = false;
					notificationImage.color = new Color (notificationImage.color.r, notificationImage.color.g, notificationImage.color.b, 0f);
					CheckForAvailableSkill ();
				}
			}
		}
	}

	public void UpdateCost()
	{
		for (int i = 0; i < 9; i++) {
			Text cost = skillStubs [i].transform.Find ("CostText").gameObject.GetComponent <Text>();
			cost.text = skillTree [i].cost.ToString ();
		}
	}

	public void setStubs()
	{
		for (int i = 0; i < 9; i++) {
			skillStubs [i] = GameObject.Find ("SkillStub (" + i.ToString () + ")");
			skillStubs [i].GetComponent <SkillStubScript> ().SetSTM (this);
		}
		//GameObject.Find ("Buy").GetComponent <SkillStubScript>().SetSTM (this);
	}

	public string GetSkillDescription(int num)
	{
		return skillTree [num].description;
	}
}
