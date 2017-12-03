using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public float Lifetime = 0.2f;
    public float DriftSpeed = 0.3f;

    public bool StillEffective = true;

    public LadyController Attacker;

	public void Update ()
    {
        var drift = new Vector3(DriftSpeed, 0.0f, 0.0f);
        transform.localPosition += drift * Time.deltaTime;

        Lifetime -= Time.deltaTime;
        if(Lifetime <= 0.0f)
        {
            DestroyObject(gameObject);
        }
	}

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (StillEffective)
        {
            var cat = other.GetComponent<CatBehaviour>();
            if (cat != null)
            {
                StillEffective = false;
                cat.LaunchIntoOrbit(this);
            }
        }
    }
}
