using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scan : MonoBehaviour
{

    public float c = 2;
    public bool isBroadcast = false;
    public Civilization source;
    public Star target;
    public Vector3 targetPos;
    private Vector3 targetDir;

    // Use this for initialization
    void Start()
    {
        targetPos = target.transform.position;
        targetDir = Vector3.Normalize(targetPos - gameObject.transform.position);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += targetDir * Time.fixedDeltaTime * c;
        if (target != null)
        {
            if (Vector3.Distance(gameObject.transform.position, targetPos) < Time.fixedDeltaTime * c)
            {
                Destroy(gameObject,1);
                GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (source == Game.Civilizations[0].GetComponent<Civilization>())
				{
                    target.UpdateDetails();
                    if (isBroadcast || (target.owner != null && target.owner != Game.Civilizations[0].GetComponent<Civilization>()))
                    {
                        target.TargetIcon.SetActive(true);
                        target.displayedLifeformProbability = true;
                    }
                    if (Game.Instance.selectedStar == target) UI.DisplayTargetInfo();
                }
                else if (target.owner == null && !isBroadcast && !source.safeStars.Contains(target))
                    source.safeStars.Add(target);
                else if (!source.threats.Contains(target) && !source.ownedStars.Contains(target))
                    source.threats.Add(target);
                source.undiscoveredStars.Remove(target);
                Destroy(this);
            }
        }
        else
        {
            GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(gameObject,1);
            Destroy(this);
        }

    }
}
