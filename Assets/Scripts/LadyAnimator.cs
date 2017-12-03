using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LadyAnimator : MonoBehaviour
{
    public Sprite[] IdleSprites;
    public Sprite DashSprite;
    public Sprite JumpSprite;
    public Sprite AttackSprite;

    public enum States
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
    }

    public float IdleSpritePeriod = 1.1f;

    public States State = States.Idle;

	void Start ()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_NextIdleSprite = IdleSpritePeriod;
	}
	
	void Update ()
    {
        var spriteToRender = m_Renderer.sprite;

        switch(State)
        {
            case States.Idle:
                m_NextIdleSprite -= Time.deltaTime;
                if(m_NextIdleSprite <= 0.0f)
                {
                    m_NextIdleSprite += IdleSpritePeriod;
                    m_IdleSpriteIndex = (m_IdleSpriteIndex + 1) % IdleSprites.Length;
                }
                spriteToRender = IdleSprites[m_IdleSpriteIndex];
                break;
            case States.Moving:
                spriteToRender = DashSprite;
                break;
            case States.Jumping:
                spriteToRender = JumpSprite;
                break;
            case States.Attacking:
                spriteToRender = AttackSprite;
                break;
        }

        m_Renderer.sprite = spriteToRender;
	}

    private SpriteRenderer m_Renderer;
    private int m_IdleSpriteIndex = 0;
    private float m_NextIdleSprite = 0.0f;
}
