using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BirdAnimation))]
public class BirdBehaviour : MonoBehaviour
{
    public enum States
    {
        LookingForPerch,
        FlyingToPerch,
        EnjoyingNicePerch,
        FlyingAway,
        Dead,
    }

    public float MaxSpeed = 2.0f;
    public float FlyingVelocityCoefficient = 0.99f;
    public float MinimumPerchDistance = 0.1f;
    public float AccelCoefficient = 2.0f;
    public float PerchLerpAlpha = 0.5f;

    public States State = States.LookingForPerch;
    public PerchSpot CurrentPerch = null;
    public BirdExit CurrentExit = null;
    public BirdCatcher CaughtBy = null;

	void Start ()
    {
        m_Animation = GetComponent<BirdAnimation>();
        var angle = (float)(m_Rng.NextDouble() * (3.14159 * 2.0));
        m_Velocity = new Vector2(
            Mathf.Cos(angle) * MaxSpeed,
            Mathf.Abs(Mathf.Sin(angle) * MaxSpeed)
        );
	}
	
	void Update ()
    {
        var flying = false;
        var canPeck = false;

        var acceleration = Vector3.zero;

        if(transform.position.magnitude > 8.0f)
        {
            DestroyObject(gameObject);
        }

        if((CaughtBy != null) && (State != States.Dead))
        {
            State = States.Dead;
            transform.parent = CaughtBy.transform;
            transform.localPosition = Vector3.zero;
        }

        var nextState = State;
        switch(State)
        {
            case States.LookingForPerch:
                flying = true;
                var perches = FindObjectsOfType<PerchSpot>()
                    .Where(perch => perch.CanPerchHere())
                    .ToArray();
                if(perches.Length > 0)
                {
                    CurrentPerch = perches[m_Rng.Next(perches.Length)];
                    m_PerchOffset = new Vector3((float) m_Rng.NextDouble() * 0.2f, 0.0f);
                    nextState = States.FlyingToPerch;
                }
                break;
            case States.FlyingToPerch:
                {
                    flying = true;
                    if (CurrentPerch.IsSafe())
                    {
                        var vecToPerch = (CurrentPerch.transform.position + m_PerchOffset) - transform.position;
                        var distance = vecToPerch.magnitude;
                        var magnitude = Mathf.Min(distance * AccelCoefficient, MaxSpeed);
                        if (distance >= MinimumPerchDistance)
                        {
                            acceleration = vecToPerch.normalized * magnitude;
                        }
                        else
                        {
                            nextState = States.EnjoyingNicePerch;
                        }
                    }
                    else
                    {
                        CurrentPerch = null;
                        nextState = States.LookingForPerch;
                    }
                } break;
            case States.EnjoyingNicePerch:
                transform.position = Vector3.Lerp(transform.position, (CurrentPerch.transform.position + m_PerchOffset), PerchLerpAlpha);
                canPeck = CurrentPerch.IsOnGround;
                if(CurrentPerch.IsSafe() == false)
                {
                    nextState = States.FlyingAway;
                    var exits = FindObjectsOfType<BirdExit>();
                    if(exits.Length > 0)
                    {
                        CurrentExit = exits[m_Rng.Next(exits.Length)];
                    }
                }
                break;
            case States.FlyingAway:
                {
                    flying = true;
                    var vecToExit = CurrentExit.transform.position - transform.position;
                    var distance = vecToExit.magnitude;
                    var magnitude = Mathf.Min(distance * AccelCoefficient, MaxSpeed);
                    if (distance >= MinimumPerchDistance)
                    {
                        acceleration = vecToExit.normalized * magnitude;
                    }
                    else
                    {
                        DestroyObject(gameObject);
                    }
                } break;
            case States.Dead:
                flying = false;
                canPeck = false;
                break;
        }
        State = nextState;

        if((flying == false) || (State == States.Dead))
        {
            acceleration = Vector3.zero;
            m_Velocity = Vector3.zero;
        }

        m_Velocity += acceleration * Time.deltaTime;
        transform.position += m_Velocity * Time.deltaTime;
        m_Velocity *= FlyingVelocityCoefficient;

        m_Animation.Flying = flying;
        m_Animation.AllowPecking = canPeck;
	}

    private BirdAnimation m_Animation;
    private Vector3 m_PerchOffset;
    private Vector3 m_Velocity = Vector3.zero;
    private System.Random m_Rng = new System.Random();
}
