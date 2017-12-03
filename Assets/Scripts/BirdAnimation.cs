using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class BirdAnimation : MonoBehaviour {
    public bool Flying = false;
    public bool AllowPecking = false;

    public Sprite IdleSprite;
    public Sprite PeckSprite;
    public Sprite FlapSprite1;
    public Sprite FlapSprite2;

    public float MinimumPeckPeriod = 0.5f; // seconds
    public float MaximumPeckPeriod = 5.0f; // seconds
    public float PeckDuration = 0.2f; // seconds
    public float FlapDuration = 0.3f; // seconds

    public float MinimumFacingTogglePeriod = 0.5f; // seconds
    public float MaximumFacingTogglePeriod = 5.0f; // seconds

	public void Start ()
    {
        m_Renderer = GetComponent<SpriteRenderer>();
	}
	
	public void Update ()
    {
        var spriteToShow = IdleSprite;
        var horizontalScaling = transform.localScale.x;

        if(Flying)
        {
            m_FlapTimer -= Time.deltaTime;
            if (m_FlapTimer <= 0.0f)
            {
                m_FlapState = !m_FlapState;
                m_FlapTimer = FlapDuration;
            }

            spriteToShow = m_FlapState ? FlapSprite1 : FlapSprite2;
            horizontalScaling = 1.0f; // force facing the correct direction during flight
        }
        else
        {
            if (AllowPecking)
            {
                m_NextPeck -= Time.deltaTime;
                if (m_NextPeck <= 0.0f)
                {
                    if (m_IsPecking == false)
                    {
                        m_IsPecking = true;

                        m_NextPeck = PeckDuration;
                    }
                    else
                    {
                        m_IsPecking = false;

                        var peckTimeRange = Math.Abs(MaximumPeckPeriod - MinimumPeckPeriod);
                        m_NextPeck = (peckTimeRange * (float)m_Rng.NextDouble()) + MinimumPeckPeriod;
                    }
                }

                if (m_IsPecking)
                {
                    spriteToShow = PeckSprite;
                }
            }

            m_NextFacingToggle -= Time.deltaTime;
            if(m_NextFacingToggle <= 0.0f)
            {
                horizontalScaling *= -1;

                var facingToggleTimeRange = Math.Abs(MaximumFacingTogglePeriod - MinimumFacingTogglePeriod);
                m_NextFacingToggle = (facingToggleTimeRange * (float) m_Rng.NextDouble()) + MinimumFacingTogglePeriod;
            }
        }

        m_Renderer.sprite = spriteToShow;
        transform.localScale = new Vector3(horizontalScaling, transform.localScale.y, transform.localScale.z);
	}

    private float m_NextPeck = 0.0f;
    private float m_NextFacingToggle = 0.0f;
    private bool m_FlapState = false;
    private float m_FlapTimer = 0.0f;
    private bool m_IsPecking = false;
    private SpriteRenderer m_Renderer;
    private System.Random m_Rng = new System.Random();
}
