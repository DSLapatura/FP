using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broadcaster : MonoBehaviour
{
    public static void StartBroadcast(Star origin, Star target)
    {
        if (origin.owner.gameObject == Game.Civilizations[0]) target.gameObject.transform.GetChild(1).gameObject.SetActive(true);
        List<Civilization> Civs = new List<Civilization>(Game.Civilizations.FindAll(x => x != null).ConvertAll(c => c.GetComponent<Civilization>()));
        foreach (var c in Civs)
        {
            if (c != origin.owner && UnityEngine.Random.Range(1, Game.Civilizations.Count) == 1)
            {
                Star st = c.ownedStars[UnityEngine.Random.Range(0, c.ownedStars.Count - 1)];
                GameObject p = Instantiate(Game.Instance.ScanSignal, st.transform.position, Quaternion.FromToRotation(st.transform.position, target.transform.position));
                p.SetActive(true);
                if (p.TryGetComponent(out Scan s))
                {
                    s.target = target;
                    s.source = c;
                    s.isBroadcast = true;
                }
                if (UnityEngine.Random.Range(1, 4) == 1)
                {
                    GameObject q = Instantiate(Game.Instance.ScanSignal, st.transform.position, Quaternion.FromToRotation(st.transform.position, target.transform.position));
                    q.SetActive(true);
                    if (q.TryGetComponent(out Scan sc))
                    {
                        sc.target = origin;
                        sc.source = c;
                        sc.isBroadcast = true;
                    }
                }
            }
        }
    }

}
