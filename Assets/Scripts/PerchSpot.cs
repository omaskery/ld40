using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PerchSpot : MonoBehaviour {
    public float ThreatLevel = 0.0f;
    public float MaxSafeThreatLevel = 10.0f;
    public float MaxThreatLevel = 20.0f;
    public float DecayPerSecond = 1.0f;
    public bool IsOnGround = false;

    public bool CanPerchHere()
    {
        return IsSafe();
    }

    public bool IsSafe()
    {
        return (ThreatLevel < MaxSafeThreatLevel);
    }

    public void Update()
    {
        var wasSafe = IsSafe();

        m_Threats.RemoveAll(threat => threat == null);

        var threatPerSecond = m_Threats
            .Select(threat => threat.ThreatLevel)
            .Sum();
        ThreatLevel += (threatPerSecond - DecayPerSecond) * Time.deltaTime;
        ThreatLevel = Mathf.Clamp(ThreatLevel, 0.0f, MaxThreatLevel);

        if(wasSafe != IsSafe())
        {
            if(wasSafe == false)
            {
                // Debug.Log("perch no longer safe");
            }
            else
            {
                // Debug.Log("perch now safe");
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        var threat = collision.GetComponent<ThreatToPerch>();
        if(threat != null)
        {
            m_Threats.Add(threat);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        var threat = collision.GetComponent<ThreatToPerch>();
        if(threat != null)
        {
            m_Threats.Remove(threat);
        }
    }

    private List<ThreatToPerch> m_Threats = new List<ThreatToPerch>();
}
