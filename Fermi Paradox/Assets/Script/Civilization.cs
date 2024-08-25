using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public partial class Civilization : MonoBehaviour
{
    public List<Star> ownedStars = new List<Star>();
    public int resources = 2000;
    public int totalProduction;
    public List<Star> threats = new List<Star>();
    public List<Star> safeStars = new List<Star>();
    public List<Star> undiscoveredStars = new List<Star>();
    Star target;
    void Start()
    {
        foreach(var s in Game.stars)
		{
            undiscoveredStars.Add(s.GetComponent<Star>());
		}
    }
    float t;
    float d = 2;
    float dmax = 5;
    void Update()
    {
        t += Time.deltaTime;
        if (t > d)
        {
            t = 0.0f;
            d = UnityEngine.Random.Range(1f, dmax);
            if (Game.Civilizations[0].GetComponent<Civilization>() != this)
            {
				try 
                {
                    if (ownedStars.Count == 0)
                    {
                        gameObject.name += "(Destroyed)";
                        Game.Civilizations[Game.Civilizations.IndexOf(gameObject)] = null;
                        Destroy(this);
                    } 
                    else if (threats.Count > 0 && resources > Game.Instance.AttackResourceCost + Game.Instance.FleetResourceCost * 2)
                    {
                        dmax = 3;

                        switch (UnityEngine.Random.Range(0, 10))
                        {
                            case < 1:
                                target = ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)];
                                if (resources > Game.Instance.FleetResourceCost * 2)
                                    TrySendFleet(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                break;
                            case < 6:
                                attack:
                                target = threats[UnityEngine.Random.Range(0, threats.Count)];
                                if (TryAttack(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target))
                                    threats.Remove(target);
                                break;
                            case < 9:
                                target = threats[UnityEngine.Random.Range(0, threats.Count)];
                                if (Game.Civilizations.FindAll(x => x != null).Count() > 2) TryBroadcast(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                else goto attack;
                                break;
                            default:
                                if (resources > Game.Instance.FleetResourceCost * 3 && safeStars.Count > 0)
                                {
                                    target = safeStars[UnityEngine.Random.Range(0, safeStars.Count)];
                                    TrySendFleet(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                }
                                else
                                {
                                    target = threats[UnityEngine.Random.Range(0, threats.Count)];
                                    TryBroadcast(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                }
                                break;
                        }
                    }
                    else if (safeStars.Count > 2)
                    {
                        dmax = 6;
                        switch (UnityEngine.Random.Range(0, 10))
                        {
                            case < 2:
                                target = ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)];
                                TrySendFleet(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                break;
                            case < 5:
                                target = safeStars[UnityEngine.Random.Range(0, safeStars.Count)];
                                TrySendFleet(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                break;
                            case < 8:
                                    if (undiscoveredStars.Count > 0)
                                    {
                                        target = undiscoveredStars[UnityEngine.Random.Range(0, undiscoveredStars.Count)];
                                        TryScan(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                                    }
                                    break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        dmax = 4;
                        if (undiscoveredStars.Count > 0)
                        {
                            target = undiscoveredStars[UnityEngine.Random.Range(0, undiscoveredStars.Count)];
                            TryScan(ownedStars[UnityEngine.Random.Range(0, ownedStars.Count)], target);
                        }
                    }
                }
                catch (Exception e)
				{
                    Debug.LogWarning(e);
				}
            }
        }
        totalProduction = 0;
        foreach (var s in ownedStars)
		{
            totalProduction += s.resourceProduction;
		}
    }

    public bool TryScan(Star source, Star target) 
    {
        if (resources >= Game.Instance.ScanResourceCost)
        {
            Game.Scan(source, target);
            resources -= Game.Instance.ScanResourceCost;
            return true;
        }
        else return false;
    }
    public bool TryBroadcast(Star source, Star target)
    {
        if (resources >= Game.Instance.BroadcastResourceCost)
        {
            Game.Broadcast(source, target);
            resources -= Game.Instance.BroadcastResourceCost;
            return true;
        }
        else return false;
    }
    public bool TrySendFleet(Star source, Star target)
    {
        if (resources >= Game.Instance.FleetResourceCost)
        {
            Game.SendFleet(source, target);
            resources -= Game.Instance.FleetResourceCost;
            return true;
        }
        else return false;
    }
    public bool TryAttack(Star source, Star target)
    {
        if (resources >= Game.Instance.AttackResourceCost)
        {
            Game.LaunchAttack(source, target);
            resources -= Game.Instance.AttackResourceCost;
            return true;
        }
        else return false;
    }


}