using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public partial class menu : MonoBehaviour
{
	//bool init = false;
	public GameObject game;
	public GameObject panel;
	void Start()
	{
		Application.targetFrameRate = 60;
	}

	void Update()
	{
		StartGame();
	}
	public void GameInit()
	{
		panel.GetComponent<Image>().CrossFadeAlpha(0, 2, true);
		//init = true;
	}
	public void StartGame()
	{
		if (Input.anyKeyDown)
		{
			FindObjectOfType<Game>(true).gameObject.SetActive(true);
			FindObjectOfType<UI>(true).gameObject.SetActive(true);
			foreach (var i in GetComponentsInChildren<Image>())
			{
				i.CrossFadeAlpha(0, 1, true);
			}
			Destroy(gameObject,2);
			Destroy(this);
		}

	}
}



