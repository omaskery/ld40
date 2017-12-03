using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LadyAnimator), typeof(FacingController))]
public class LadyController : MonoBehaviour
{
    public float MaxMoveSpeed = 4.0f;
    public float AirControlCoefficient = 0.3f;
    public float FloorDrag = 0.8f;
    public float JumpHeight = 10.0f;
    public float JumpDuration = 1.0f;
    public float JumpReleaseAttenuation = 0.8f;
    public float AttackAcceleration = 10.0f;

    public Transform LeftBound;
    public Transform RightBound;

    public AttackEffect AttackEffectPrefab;
    public AttackOrigin AttackOriginMarker;

	public void Start ()
    {
        m_Facing = GetComponent<FacingController>();
        m_Animator = GetComponent<LadyAnimator>();
        m_StartingHeight = transform.position.y;
	}
	
	public void Update ()
    {
        var xInput = Input.GetAxis(GameConstants.HorizontalInput);
        var xAcceleration = xInput * MaxMoveSpeed;
        var jump = Input.GetButtonDown(GameConstants.JumpInput);
        var stillJump = Input.GetButton(GameConstants.JumpInput) && (jump == false);
        var attack = Input.GetButtonDown(GameConstants.AttackInput);

        var yVelocity = m_Velocity.y + GameConstants.Gravity * Time.deltaTime;

        if(m_OnGround == false)
        {
            xAcceleration *= AirControlCoefficient;
        }

        if(attack && (m_CurrentAttackEffect == null))
        {
            m_CurrentAttackEffect = Instantiate(AttackEffectPrefab, AttackOriginMarker.transform);
            m_CurrentAttackEffect.Attacker = this;
            xAcceleration += m_Facing.GetXComponent() * AttackAcceleration;
        }

        if(jump && m_OnGround)
        {
            yVelocity = (JumpHeight / JumpDuration) - (0.5f * GameConstants.Gravity * JumpHeight);
        }

        var xVelocity = m_Velocity.x + xAcceleration * Time.deltaTime;
        if((stillJump == false) && (m_OnGround == false) && (m_Velocity.y > 0.0f))
        {
            yVelocity *= JumpReleaseAttenuation;
        }
        m_Velocity = new Vector3(xVelocity, yVelocity, m_Velocity.z);

        var newPosition = transform.position + m_Velocity * Time.deltaTime;
        if((newPosition.x < LeftBound.position.x) || (newPosition.x > RightBound.position.x))
        {
            newPosition = new Vector3(
                Mathf.Clamp(newPosition.x, LeftBound.position.x, RightBound.position.x),
                newPosition.y,
                newPosition.z
            );
            m_Velocity = new Vector3(
                0.0f, m_Velocity.y, m_Velocity.z
            );
        }
        if(newPosition.y < m_StartingHeight)
        {
            newPosition = new Vector3(newPosition.x, m_StartingHeight, newPosition.z);
            if (m_OnGround == false)
            {
                // simulate slow down of landing
                m_Velocity = new Vector3(
                    m_Velocity.x * (FloorDrag / 2),
                    m_Velocity.y,
                    m_Velocity.z
                );
                m_OnGround = true;
            }
        }
        else
        {
            if (m_OnGround)
            {
                m_OnGround = false;
            }
        }
        transform.position = newPosition;

        if (m_OnGround)
        {
            var drag = FloorDrag;
            if(Mathf.Abs(xInput) < 0.1f)
            {
                drag /= 2.0f;
            }
            m_Velocity *= drag;
        }

        var animationState = LadyAnimator.States.Idle;

        if(m_CurrentAttackEffect != null)
        {
            animationState = LadyAnimator.States.Attacking;
        }
        else if(m_OnGround == false)
        {
            animationState = LadyAnimator.States.Jumping;
        }
        else if(Mathf.Abs(m_Velocity.x) > 0.1f)
        {
            animationState = LadyAnimator.States.Moving;
        }

        m_Animator.State = animationState;
	}

    private LadyAnimator m_Animator;
    private FacingController m_Facing;

    private AttackEffect m_CurrentAttackEffect = null;

    private Vector3 m_Velocity;
    private bool m_OnGround = true;
    private float m_StartingHeight;
}
