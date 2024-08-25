using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float c = 3;
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
                Array.ForEach(gameObject.GetComponentsInChildren<TrailRenderer>(), t => t.widthMultiplier -= Time.fixedDeltaTime / 4);
                Destroy(gameObject, 4f);
                //Destroy(target.gameObject);
                target.Destruct();
            }
        }
        else
        {
            Array.ForEach(gameObject.GetComponentsInChildren<TrailRenderer>(), t => t.widthMultiplier -= Time.fixedDeltaTime);
            Destroy(gameObject, 1f);
        }

    }
}
