using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Star : MonoBehaviour
{
    public int totalResource = 0;
    public int resourceProduction = 0; //How many resources can this star product (every second), may decrease over time
    public int productionDecrement = 0;

    public int displayedTotalResource = -1;
    public int displayedresourceProduction = -1;
    public bool displayedLifeformProbability = false;

    public GameObject ColonyIcon;
    public GameObject TargetIcon;

    //public List<Star> ParentStar = new List<Star>();
    //public Star ParentStar = null;
    //public List<Star> ChildStar = new List<Star>();
    public Civilization owner = null; //The owner(colonizer) civilization of the star, set to null if it's not colonized by others

   

    float t;
    void Start()
    {
        totalResource = UnityEngine.Random.Range(60000, 80000);
        resourceProduction = UnityEngine.Random.Range(200, 300);
        productionDecrement = UnityEngine.Random.Range(1, 4);
    }

    void Update()
    {
        t += Time.deltaTime;
        if (t > 1.0f)
        {
            t = 0.0f;
            if (owner != null)
            {

                if (totalResource > resourceProduction)
                {
                    totalResource -= resourceProduction;
                    owner.resources += resourceProduction;
                    if (resourceProduction > 100)
                    {
                        resourceProduction -= productionDecrement;
                    }
                }
                else
                {
                    owner.resources += totalResource;
                    totalResource = 0;
                }

                if(owner == Game.Civilizations[0].GetComponent<Civilization>())
				{
                    displayedTotalResource = totalResource;
                    displayedresourceProduction = resourceProduction;
                    displayedLifeformProbability = false;
                }
            }
        }
    }

    void OnDestroy()
    {

    }
    public void UpdateDetails()
    {
        displayedTotalResource = totalResource;
        displayedresourceProduction = resourceProduction;
        //displayedLifeformProbability = owner != Game.Civilizations[0].GetComponent<Civilization>() && owner != null;
    }
    public void Destruct()
	{
        Game.stars.Remove(gameObject);
        foreach (var c in Game.Civilizations)
        {
            if (c != null && c.TryGetComponent<Civilization>(out Civilization civ))
            {
                civ.undiscoveredStars.Remove(this);
                civ.safeStars.Remove(this);
                civ.threats.Remove(this);
            }
        }
        if (owner != null) owner.ownedStars.Remove(this);
        if (Game.Instance.selectedStar == this)
        {
            Game.Instance.selectedStar = null;
            Game.action = null;
            Game.Instance.Selection.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            UI.Instance.HideActions();
        }
        //if (gameObject.transform.GetChild(0).gameObject.activeSelf) Instantiate(Game.Instance.ColonyDestroyedIcon, transform.position, transform.rotation);
        //Array.ForEach(gameObject.GetComponentsInChildren<ParticleSystem>(), x => x.Stop(true, ParticleSystemStopBehavior.StopEmitting));
        GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Destroy(this);
        Destroy(gameObject, 1);
    }

    public void Colonize(Civilization c)
    {
        owner = c;
        c.ownedStars.Add(this);
        c.undiscoveredStars.Remove(this);
        c.safeStars.Remove(this);
        if (c == Game.Civilizations[0].GetComponent<Civilization>()) 
        {
            ColonyIcon.SetActive(true);
            //gameObject.GetComponentsInChildren<ParticleSystem>()[0].Play();
            UpdateDetails();
        }
    }

}
