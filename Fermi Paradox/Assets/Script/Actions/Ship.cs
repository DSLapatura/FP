using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制飞船的移动和到达
/// </summary>
/// <remarks>注释者：唐天乙 2021/5/21</remarks>
public class Ship : MonoBehaviour
{

    [Tooltip("The civilization the ship belongs")]
    public Civilization civil;
    [Tooltip("The maximum speed of this ship")]
    public float maxVel = 3;
    [Tooltip("Time the ship takes to acceleraate to maximum speed")]
    public float accelerationTime = 1;
    private float velocity = 0;
    [Tooltip("The star to go for")]
    public Star target;

    public Star origin;
    public Vector3 targetPos;
    private Vector3 targetDir;
    //[Tooltip("The technology the ship has")]
    //public float science;
    void Start()
    {
        civil = origin.owner;
        targetPos = target.transform.position;
        targetDir = Vector3.Normalize(targetPos - gameObject.transform.position);
        //science = civil.science;
    }

    /// <summary>
    /// 控制飞船移动，并判断是否到达目的地（需重写）
    /// </summary>
    void FixedUpdate()
    {
        if (target != null)
        {
            Move();
            if (gameObject.transform.position == targetPos)
            {
                arrive(target);
            }
        }
        else
        {
            target = Game.GetNearestStarAtPos(targetPos, civil);
            if(target != null)
			{
                targetPos = target.transform.position;
                targetDir = Vector3.Normalize(targetPos - gameObject.transform.position);
            }
            else
			{
                targetPos = targetDir * 100;
                gameObject.GetComponent<TrailRenderer>().widthMultiplier -= Time.fixedDeltaTime;
                Destroy(gameObject, 1f);
            }
        }
        //Destroy(gameObject);
    }

    void Move()
	{
        if (Vector3.Distance(gameObject.transform.position, targetPos) < velocity * velocity / 2 / maxVel * accelerationTime)//if smaller than acceleration distance
            velocity -= maxVel * Time.deltaTime / accelerationTime;//decelerate
        else
        {
            if (velocity < maxVel)
                velocity += maxVel * Time.deltaTime / accelerationTime;//accelerate
        }
        if (Vector3.Distance(gameObject.transform.position, targetPos) > Time.fixedDeltaTime * velocity + 0.05f)//if not touching target, with an acceptable range
            transform.position += targetDir * Time.fixedDeltaTime * velocity;
        else
            transform.position = targetPos;
    }
    private void OnDestroy()
    {
    }
    /// <summary>
    /// 当飞船到达目标星系，建立殖民地
    /// </summary>
    /// <param name="t">需要前往的目标星系</param>
    /// <remarks>注释者：s5ehfr9 2021/5/13</remarks>
    private void arrive(Star t)
    {
        if (t == origin)
            civil.resources += Game.Instance.FleetResourceCost / 2;
        else if (t.owner == civil)
            t.resourceProduction += Game.Instance.ShipResourceProduction;
        else if (t.owner == null)
        {
            t.Colonize(civil);
            //if (!origin.ChildStar.Contains(t)) origin.ChildStar.Add(t);
            //t.ParentStar = origin;
        }
        else 
        {
            if (civil == Game.Civilizations[0].GetComponent<Civilization>())
            {
                target.TargetIcon.SetActive(true);
                target.displayedLifeformProbability = true;
            }
			else
			{
                civil.undiscoveredStars.Remove(t);
                civil.safeStars.Remove(t);
                civil.threats.Add(t);
            }
        }
        Destroy(gameObject, 1f);
        Destroy(this);
    }
}

