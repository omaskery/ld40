using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCatcher : MonoBehaviour
{
    public BirdBehaviour Caught = null;
    public bool ReadyToCatch = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((Caught == null) && ReadyToCatch)
        {
            var bird = other.GetComponent<BirdBehaviour>();
            if (bird != null)
            {
                bird.CaughtBy = this;
                Caught = bird;
            }
        }
    }
}
