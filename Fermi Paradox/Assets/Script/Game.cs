using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public partial class Game : MonoBehaviour
{
    public static List<GameObject> stars = new List<GameObject>();
    public static List<GameObject> Civilizations = new List<GameObject>();

    //string date = "02-11";
    //public static long RandomSeed = (CreateMD5(DateTime.Now.ToString("MM-dd")) == "0f3610e186e2d4f6b36ca5c230833787" || CreateMD5(DateTime.Now.ToString("MM-dd")) == "b16ecc563180ea1a0ac4b3aa273383cd") ? 61276564485 : Convert.ToInt64(UnityEngine.Random.Range(0, 78364164095));


    public int ScanResourceCost = 500;
    public int BroadcastResourceCost = 1100;
    public int FleetResourceCost = 5000;
    public int AttackResourceCost = 3250;
    public int ShipResourceProduction = 375;

    public int CivilizationCount = 5;

    public GameObject ScanSignal;
    public GameObject ShipFleet;
    public GameObject AttackProjectile;

    public GameObject Civilization;

    public GameObject ColonyDestroyedIcon;
    public GameObject Selection;

    public static Game Instance;

    public Star selectedStar;

    public delegate bool ActionDelegate(Star Source, Star target);
    public static ActionDelegate action = null;

    public static string CreateMD5(string input)
    {
        using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }

    void Start()
    {
        
        Civilizations = new List<GameObject>();
        //UnityEngine.Random.InitState(Game.RandomSeed);
        Instance = gameObject.GetComponent<Game>();
        for (int i = 0; i < CivilizationCount; i++)
        {
            var c = Instantiate(Instance.Civilization);
            Civilizations.Add(c);
            Game.stars[i].GetComponent<Star>().Colonize(c.GetComponent<Civilization>());
        }
    }

    void Awake()
    {

    }

    void Update()
    {
        Select();
        if (selectedStar != null && selectedStar.owner == Civilizations[0].GetComponent<Civilization>()) UI.DisplayTargetInfo();
    }

    //public Func<Star, Civilization, Star> _GetNearestStar = (star, civ) => civ != null ? civ.ownedStars.OrderBy(s => Vector3.Distance(star.transform.position, s.transform.position)).First() : stars.OrderBy(s => Vector3.Distance(star.transform.position, s.transform.position)).First().GetComponent<Star>();
    //public Func<Vector3, Civilization, Star> _GetNearestStarAtPos = (pos, civ) => civ != null ? civ.ownedStars.OrderBy(s => Vector3.Distance(pos, s.transform.position)).First() : stars.OrderBy(s => Vector3.Distance(pos, s.transform.position)).First().GetComponent<Star>();
    public static Func<Star, Civilization, Star> _GetNearestStar = (star, civ) => GetNearestStar(star, civ);
    public static Func<Vector3, Civilization, Star> _GetNearestStarAtPos = GetNearestStarAtPos;

    public static Star GetNearestStar(Star star, Civilization civ = null)
    {
        List<Star> targets = new List<Star>();
        if (civ != null)
        {
            targets = civ.ownedStars.FindAll(s => s != star);
        }
		else 
        {
            targets = stars.ConvertAll(s => s.GetComponent<Star>()).FindAll(s => s != star);
        }

        return targets.Any() ? targets.Aggregate((a, b) => (Vector3.Distance(star.transform.position, a.transform.position) < Vector3.Distance(star.transform.position, b.transform.position)) ? a : b).GetComponent<Star>() : star;
    }

    public static Star GetNearestStarAtPos(Vector3 pos, Civilization civ = null)
    {
        List<Star> targets = new List<Star>();
        if (civ != null && civ.ownedStars.Any())
        {
            targets = civ.ownedStars;
        }
        else
        {
            targets = stars.ConvertAll(s => s.GetComponent<Star>());
        }
        return targets.Any() ? targets.Aggregate((a, b) => (Vector3.Distance(pos, a.transform.position) < Vector3.Distance(pos, b.transform.position)) ? a : b).GetComponent<Star>() : null;
    }

    public void Select()
    {
		try
		{
            if (Input.GetMouseButtonDown(0))
            {
                if (action == null)
                {

                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.TryGetComponent(out Star target))
                    {
                        Selection.GetComponent<ParticleSystem>().Clear();
                        Debug.Log("selected");
                        selectedStar = target;
                        Selection.transform.position = target.transform.position;
                        if(!Selection.GetComponent<ParticleSystem>().isPlaying) Selection.GetComponent<ParticleSystem>().Play();

                        if (target.owner == Civilizations[0].GetComponent<Civilization>()) UI.Instance.DisplayActions();
                        else UI.Instance.HideActions();

                        UI.DisplayTargetInfo();
                    }
                }
                else
                {
                    //Selection.SetActive(false);
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) && hit.collider.TryGetComponent(out Star target))
                    {
                        if (target != selectedStar && (target.owner != Civilizations[0].GetComponent<Civilization>() || action == new Game.ActionDelegate(Civilizations[0].GetComponent<Civilization>().TrySendFleet)))
                        {
                            Debug.Log("action");
                            action(selectedStar, target);
                            action = null;
                            UI.ClearActionDescription();
                        }
                    }
                }
            }
        }
        catch(Exception e)
		{
            Debug.Log(e);
		}
    }

    public static void Scan(Star Source, Star target)
    {
        //if (Source.owner.resources >= ScanResourceCost)
        {
            //Source.owner.resources -= ScanResourceCost;
            //GameObject p = Instantiate(Instance.Selection,target.transform.position,target.transform.rotation);
            GameObject p = Instantiate(Instance.ScanSignal, Source.transform.position, Quaternion.FromToRotation(Source.transform.position, target.transform.position));
            p.SetActive(true);
            if (p.TryGetComponent(out Scan s))
            {
                s.target = target;
                s.source = Source.owner;
                if (Source.owner.gameObject == Civilizations[0]) s.GetComponent<ParticleSystem>().Play();
            }

        }
    }
    public static void Broadcast(Star Source, Star target)
    {
        //if (Source.owner.resources >= BroadcastResourceCost)
        {
            //Source.owner.resources -= BroadcastResourceCost;
            //GameObject p = Instantiate(Instance.Selection,target.transform.position,target.transform.rotation);
            Broadcaster.StartBroadcast(Source, target);

        }
    }
    public static void SendFleet(Star Source, Star target)
    {
        //if (Source.owner.resources >= FleetResourceCost)
        {
            //Source.owner.resources -= FleetResourceCost;
            //GameObject p = Instantiate(Instance.Selection,target.transform.position,target.transform.rotation);
            GameObject p = Instantiate(Instance.ShipFleet, Source.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)), Quaternion.FromToRotation(Source.transform.position, target.transform.position));
            p.SetActive(true);
            if (p.TryGetComponent(out Ship s))
            {
                s.target = target;
                s.origin = Source;
            }

        }
    }
    public static void LaunchAttack(Star Source, Star target)
    {
        //if (Source.owner.resources >= AttackResourceCost)
        {
            //Source.owner.resources -= AttackResourceCost;
            //GameObject p = Instantiate(Instance.Selection,target.transform.position,target.transform.rotation);
            GameObject p = Instantiate(Instance.AttackProjectile, Source.transform.position + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), 0, UnityEngine.Random.Range(-0.5f, 0.5f)), Quaternion.FromToRotation(Source.transform.position, target.transform.position));
            p.SetActive(true);
            if (p.TryGetComponent(out Projectile pr)) pr.target = target;
        }
    }

}
