using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSpawner : MonoBehaviour {

    public GameObject star;
    private Transform starParent;
    public int starCount;
    public float starFieldSizeX, starFieldSizeY;
    [Range(1, 50)]
    public float starFieldDepth;
    public Texture2D densityTex;
    [Range(0.01f, 10)]
    public float densityMapMultiplier = 1;
    [Range(0, 10)]
    public float starToStardustRatio;
    public GameObject starDust;
    public int civilizationCount = 2;

    void Start()
    {
        Game.stars = new List<GameObject>();
        #region spawning
        if (densityTex == null)//uniform distribution
        {
            starParent = star.transform.parent;
            for (int i = 0; i <= starCount; i++)
            {
                Game.stars.Add(Instantiate(star, (transform.position + new Vector3(Random.Range(starFieldSizeX, -starFieldSizeX), Random.Range(starFieldSizeY, -starFieldSizeY), Random.Range(0f, -1f) * starFieldDepth)), Quaternion.identity, starParent));
            }
        }
        else
        {//distribute according to density map
            #region star spawning
            starParent = star.transform.parent;
            Vector2Int samplePos;
            for (int i = 0; i <= starCount; i++)
            {
            there:
                samplePos = new Vector2Int((int)(Random.value * densityTex.width), (int)(Random.value * densityTex.height));//pick a random position
                if (Random.value < densityTex.GetPixel(samplePos.x, samplePos.y).maxColorComponent * densityMapMultiplier)//spawn with prabability stored in the pixel
				{
                    Vector3 pos = (transform.position + new Vector3((float)samplePos.x / (float)densityTex.width * 2 * starFieldSizeX - starFieldSizeX, Random.Range(0.5f, -0.5f) * Mathf.Sqrt(densityTex.GetPixel(samplePos.x, samplePos.y).maxColorComponent) * starFieldDepth - 0.5f * starFieldDepth, (float)samplePos.y / (float)densityTex.height * 2 * starFieldSizeY - starFieldSizeY));
                    if (Physics.CheckSphere(pos, 0.3f)) 
                        goto there;//there are things in this world that you're not meant to see
                    Game.stars.Add(Instantiate(star, pos, Quaternion.identity, starParent));
                }

                else
                    goto there;//didn't expect a goto, right?
            }
            #endregion star spawning

            #region star dust
            starParent = starDust.transform.parent;
            for (int i = 0; i < (int)(starCount * starToStardustRatio); i++)
            {
            here:
                samplePos = new Vector2Int((int)(Random.value * densityTex.width), (int)(Random.value * densityTex.height));
                if (Random.value < densityTex.GetPixel(samplePos.x, samplePos.y).maxColorComponent * densityMapMultiplier)
                   Instantiate(starDust, (transform.position + new Vector3((float)samplePos.x / (float)densityTex.width * 2 * starFieldSizeX - starFieldSizeX, Random.Range(0.5f, -0.5f) * Mathf.Sqrt(densityTex.GetPixel(samplePos.x, samplePos.y).maxColorComponent) * starFieldDepth - 0.5f * starFieldDepth, (float)samplePos.y / (float)densityTex.height * 2 * starFieldSizeY - starFieldSizeY)), Quaternion.identity, starParent);
                else
                    goto here;
            }
            #endregion star dust
            #endregion spawning

        }
        //foreach (var s in Game.stars) s.SetActive(true);
        GameObject.Find("TitleScreen").GetComponent<menu>().GameInit();
        //Game.stars[0].GetComponent<Star>().Colonize(Game.Civilizations[0].GetComponent<Civilization>());
    }


void Update ()
    {
		
	}
}
