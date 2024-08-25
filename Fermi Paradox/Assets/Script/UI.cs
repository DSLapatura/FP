using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
	public static bool actionsActive;
	Image[] actions;
	public static Text TargetInfo;
	public static Text ActionDescription;
	public static Text Info;
	public static Text ActionsTitle;

	public static UI Instance;

	public GameObject panel;
	float a;
	void Start()
	{
		Instance = gameObject.GetComponent<UI>();
		actions = Array.ConvertAll(GameObject.FindGameObjectsWithTag("Actions"), c => c.GetComponent<Image>());
		TargetInfo = GameObject.Find("/UI/TargetBG/TargetInfo").GetComponent<Text>();
		ActionDescription = GameObject.Find("/UI/TargetBG/ActionDescription").GetComponent<Text>();
		Info = GameObject.Find("/UI/InfoBG/Info").GetComponent<Text>();
		ActionsTitle = GameObject.Find("/UI/TargetBG/ActionsTitle").GetComponent<Text>();
		Array.ForEach(gameObject.GetComponentsInChildren<Image>(), i => i.CrossFadeAlpha(0, 0f, true));
		Array.ForEach(gameObject.GetComponentsInChildren<Text>(), i => i.CrossFadeAlpha(0, 0f, true));

		Array.ForEach(gameObject.GetComponentsInChildren<Image>(),i => i.CrossFadeAlpha(1, 0.2f, true));
		Array.ForEach(gameObject.GetComponentsInChildren<Text>(), i => i.CrossFadeAlpha(1, 0.2f, true));
		HideActions();
	}

	void Update()
	{
		Info.text = $"总储量:{Game.Civilizations[0].GetComponent<Civilization>().resources}\n总产量:{Game.Civilizations[0].GetComponent<Civilization>().totalProduction}\n拥有的恒星系:{Game.Civilizations[0].GetComponent<Civilization>().ownedStars.Count}";
	}
	
	public void DisplayActions()
	{
		actionsActive = true;
		ActionsTitle.gameObject.SetActive(true);
		foreach (var i in actions)
		{
			//i.CrossFadeAlpha(1, 0.1f, true);
			i.gameObject.SetActive(true);
		}
	}
	public void HideActions()
	{
		actionsActive = false;
		ClearActionDescription();
		ActionsTitle.gameObject.SetActive(false);
		foreach (var i in actions)
		{
			//i.CrossFadeAlpha(0, 0.1f, true);
			i.gameObject.SetActive(false);
		}
	}

	public static void DisplayTargetInfo()
	{
		string text = "";
		if (Game.Instance.selectedStar != null)
		{
			string resource, production, lifeform;

			resource = Game.Instance.selectedStar.displayedTotalResource == -1 ? "未知" : Game.Instance.selectedStar.displayedTotalResource.ToString();
			production = Game.Instance.selectedStar.displayedresourceProduction == -1 ? "未知" : Game.Instance.selectedStar.displayedresourceProduction.ToString();
			lifeform = Game.Instance.selectedStar.displayedLifeformProbability ? "//可能存在文明//" : "";
			text = "资源储量:" + resource 
			+ "\n资源产量:" + production 
			+ "\n" + lifeform;
		}
		TargetInfo.text = text;
	}

	public static void HoverOnScanButton()
	{
		if (actionsActive && (Game.action == null || Game.action == new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryScan))) ActionDescription.text = "观测\n对选中的恒星系进行观测, 以获取其资源总储量, 预期资源产量等信息\n\n资源消耗:" + Game.Instance.ScanResourceCost.ToString();
	}
	public static void HoverOnBroadcastButton()
	{
		if (actionsActive && (Game.action == null || Game.action == new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryBroadcast))) ActionDescription.text = "广播\n向其他文明发送目标恒星系的信息, 使其成为其他文明的攻击目标\n\n注意:广播有小概率使其他文明攻击发送者所处的恒星系\n\n资源消耗:" + Game.Instance.BroadcastResourceCost.ToString();
	}
	public static void HoverOnFleetButton()
	{
		if (actionsActive && (Game.action == null || Game.action == new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TrySendFleet))) ActionDescription.text = "舰队\n舰队可占领其他恒星系以采集其资源, 舰队也可发送至已占领的恒星系, 进一步增加其资源产量\n\n若舰队驶入被其他文明占领的恒星系则会被击毁\n\n资源消耗:" + Game.Instance.FleetResourceCost.ToString();
	}
	public static void HoverOnAttackButton()
	{
		if (actionsActive && (Game.action == null || Game.action == new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryAttack))) ActionDescription.text = "打击\n朝目标发射以相对论速度飞行的射弹, 彻底摧毁目标恒星系\n\n资源消耗:" + Game.Instance.AttackResourceCost.ToString();
	}
	public static void ClearActionDescription()
	{
		if (Game.action == null) ActionDescription.text = "";
	}

	public void ScanButton()
	{
		if (actionsActive)
		{
			Game.action = new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryScan);
			UI.HoverOnScanButton();
		}
	}
	public void BroadcastButton()
	{
		if (actionsActive)
		{
			Game.action = new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryBroadcast);
			UI.HoverOnBroadcastButton();
		}
	}
	public void FleetButton()
	{
		if (actionsActive)
		{
			Game.action = new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TrySendFleet);
			UI.HoverOnFleetButton();
		}
	}
	public void AttackButton()
	{
		if (actionsActive)
		{
			Game.action = new Game.ActionDelegate(Game.Civilizations[0].GetComponent<Civilization>().TryAttack);
			UI.HoverOnAttackButton();
		}
	}

	IEnumerator LoadLevelAfterDelay(float delay, string scene)
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene(scene);
	}
	public void RestartButton()
	{
		StartCoroutine(LoadLevelAfterDelay(2f, "SampleScene"));
		panel.GetComponent<Image>().CrossFadeAlpha(1, 1, true);
		Array.ForEach(GameObject.FindObjectsOfType(typeof(MonoBehaviour)),x => 
		{ 
			//Destroy(x, 1); 
		});
	}
	public void ExitButton()
	{
		Application.Quit();
	}
}



