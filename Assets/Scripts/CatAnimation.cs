using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CatAnimation : MonoBehaviour
{
    public enum States
    {
        Idle,
        PreparingToPounce,
        Pouncing,
    }

    public Sprite IdleSprite;
    public Sprite TailUpSprite;
    public Sprite PreparePounceSprite;
    public Sprite PounceSprite;

    public States State = States.Idle;

    public float MinimumTailToggleTime = 0.3f;
    public float MaximumTailToggleTime = 2.0f;

	void Start ()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
	}
	
	void Update ()
    {
        var spriteToRender = IdleSprite;

        switch(State)
        {
            case States.Idle:
                m_NextTailToggle -= Time.deltaTime;
                if(m_NextTailToggle <= 0.0f)
                {
                    m_TailIsUp = !m_TailIsUp;

                    var tailToggleTimePeriod = MaximumTailToggleTime - MinimumTailToggleTime;
                    m_NextTailToggle = (tailToggleTimePeriod * (float) m_Rng.NextDouble()) + MinimumTailToggleTime;
                }
                if(m_TailIsUp)
                {
                    spriteToRender = TailUpSprite;
                }
                break;
            case States.PreparingToPounce:
                spriteToRender = PreparePounceSprite;
                break;
            case States.Pouncing:
                spriteToRender = PounceSprite;
                break;
        }

        m_Renderer.sprite = spriteToRender;
	}

    private bool m_TailIsUp = false;
    private SpriteRenderer m_Renderer;
    private float m_NextTailToggle = 0.0f;
    private System.Random m_Rng = new System.Random();
}
