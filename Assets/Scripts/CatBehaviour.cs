using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CatAnimation), typeof(BirdCatcher), typeof(ThreatToPerch))]
public class CatBehaviour : MonoBehaviour {
    public enum States
    {
        Idle,
        Hunting,
        PreparingToAttack,
        Attacking,
        TakingCatch,
        GoingToOrbit,
    }

    public float MaxSpeed = 2.0f;
    public float HorizontalDrag = 0.7f;
    public float PounceRange = 1.0f;
    public float CrouchDuration = 1.0f;
    public float JumpDuration = 1.5f;

    public States State = States.Idle;

    public CatExit CurrentExit = null;

	void Start ()
    {
        m_Animation = GetComponent<CatAnimation>();
        m_Catcher = GetComponentInChildren<BirdCatcher>();
        m_Threat = GetComponentInChildren<ThreatToPerch>();
        m_StartingHeight = transform.position.y;
        m_HorizontalVelocity = (transform.position.x < 0.0f) ? MaxSpeed : -MaxSpeed;
        JumpToHeight((float)m_Rng.NextDouble() * 5.0f);
	}
	
	void Update ()
    {
        var readyToCatch = false;

        var nextState = State;
        switch(State)
        {
            case States.Idle:
                m_Animation.State = CatAnimation.States.Idle;
                var targets = GameObject.FindGameObjectsWithTag(GameConstants.BirdTag);
                var closest = targets
                    .OrderBy(tgt => (tgt.transform.position - transform.position).sqrMagnitude)
                    .FirstOrDefault();
                if(closest != null)
                {
                    m_Target = closest;
                    nextState = States.Hunting;
                }
                break;
            case States.Hunting:
                if (m_Target != null)
                {
                    m_Animation.State = CatAnimation.States.Idle;
                    if (m_OnGround)
                    {
                        var deltaX = m_Target.transform.position.x - transform.position.x;
                        if (Mathf.Abs(deltaX) > PounceRange)
                        {
                            m_HorizontalVelocity = Mathf.Clamp(deltaX, -MaxSpeed, MaxSpeed);
                        }
                        else
                        {
                            nextState = States.PreparingToAttack;
                            m_CrouchTimer = CrouchDuration;
                        }
                    }
                }
                else
                {
                    nextState = States.Idle;
                }
                break;
            case States.PreparingToAttack:
                if (m_Target != null)
                {
                    m_Animation.State = CatAnimation.States.PreparingToPounce;
                    m_CrouchTimer -= Time.deltaTime;
                    if (m_CrouchTimer <= 0.0f)
                    {
                        nextState = States.Attacking;
                        var requiredJumpHeight = m_Target.transform.position.y - transform.position.y;
                        var horizontalDistance = m_Target.transform.position.x - transform.position.x;
                        m_HorizontalVelocity = horizontalDistance / JumpDuration;
                        JumpToHeight(requiredJumpHeight);
                    }
                }
                else
                {
                    nextState = States.Idle;
                }
                break;
            case States.Attacking:
                m_Animation.State = CatAnimation.States.Pouncing;
                if (m_Catcher.Caught == null)
                {
                    if (m_OnGround)
                    {
                        nextState = States.Hunting;
                    }
                    else
                    {
                        readyToCatch = true;
                    }
                }
                else
                {
                    nextState = States.TakingCatch;
                    ScoreSystem.FindMe().BirdLost();
                }
                break;
            case States.TakingCatch:
                if(CurrentExit == null)
                {
                    var exits = FindObjectsOfType<CatExit>();
                    var nearest = exits
                        .OrderBy(exit => (exit.transform.position - transform.position).sqrMagnitude)
                        .FirstOrDefault();
                    if(nearest != null)
                    {
                        CurrentExit = nearest;
                    }
                }
                if((CurrentExit != null) && m_OnGround)
                {
                    m_Animation.State = CatAnimation.States.Idle;
                    var vecToExit = CurrentExit.transform.position - transform.position;
                    if (Mathf.Abs(vecToExit.x) > 0.2f)
                    {
                        m_HorizontalVelocity = Mathf.Clamp(vecToExit.x, -MaxSpeed, MaxSpeed);
                    }
                    else
                    {
                        DestroyObject(gameObject);
                    }
                }
                break;
            case States.GoingToOrbit:
                var rotationsPerSecond = 2.0f;
                transform.rotation *= Quaternion.Euler(0.0f, 0.0f, rotationsPerSecond * 360.0f * Time.deltaTime);
                if(transform.position.magnitude > 8.0f)
                {
                    DestroyObject(gameObject);
                }
                break;
        }
        /*
        if(State != nextState)
        {
            Debug.Log(string.Format("changed state {0} to {1}", State, nextState));
        }
        */
        State = nextState;

        m_Catcher.ReadyToCatch = readyToCatch;

        m_VerticalVelocity += GameConstants.Gravity * Time.deltaTime;
        if(transform.position.y < m_StartingHeight)
        {
            m_VerticalVelocity = 0.0f;
        }
        var newHeight = transform.position.y + m_VerticalVelocity * Time.deltaTime;
        if((newHeight <= m_StartingHeight) && (m_VerticalVelocity <= 0.0f))
        {
            newHeight = m_StartingHeight;
            if (m_OnGround == false)
            {
                // Debug.Log("landed on ground");
                m_OnGround = true;
            }
        }
        else if(newHeight > m_StartingHeight)
        {
            if (m_OnGround)
            {
                // Debug.Log("left ground");
                m_OnGround = false;
            }
        }
        var newX = transform.position.x + m_HorizontalVelocity * Time.deltaTime;
        if (m_OnGround)
        {
            m_HorizontalVelocity *= HorizontalDrag;
        }
        transform.position = new Vector3(
            newX, newHeight, transform.position.z
        );
	}

    private void JumpToHeight(float height)
    {
        m_VerticalVelocity = (height / JumpDuration) - (0.5f * GameConstants.Gravity * JumpDuration);
    }

    public void LaunchIntoOrbit(AttackEffect cause)
    {
        if (State != States.GoingToOrbit)
        {
            var directionAway = (cause.Attacker.transform.position.x < transform.position.x) ? 1.0f : -1.0f;
            var angle = Mathf.Deg2Rad * 70.0f;
            var launchSpeed = 15.0f;
            var launchVector = new Vector3(
                directionAway * Mathf.Cos(angle) * launchSpeed,
                Mathf.Sin(angle) * launchSpeed
            );
            m_VerticalVelocity = launchVector.y;
            m_HorizontalVelocity = launchVector.x;
            m_Threat.ThreatLevel = 0.0f;

            State = States.GoingToOrbit;

            ScoreSystem.FindMe().CatLaunchedToOrbit();
        }
    }

    private bool m_OnGround = true;
    private float m_StartingHeight;
    private float m_CrouchTimer = 0.0f;
    private GameObject m_Target = null;
    private float m_VerticalVelocity = 0.0f;
    private float m_HorizontalVelocity = 0.0f;

    private BirdCatcher m_Catcher;
    private ThreatToPerch m_Threat;
    private CatAnimation m_Animation;

    private System.Random m_Rng = new System.Random();
}
