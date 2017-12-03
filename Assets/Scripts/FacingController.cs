using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingController : MonoBehaviour {
    public enum Facings
    {
        Left,
        Right,
    }

    public Facings Facing = Facings.Right;
    public bool AutoFromVelocity = true;

    public float MinAutoTurnVelocity = 0.01f;

    public float GetXComponent()
    {
        return (Facing == Facings.Left) ? -1.0f : 1.0f;
    }

    public void FaceToward(Vector3 position)
    {
        Facing = (position.x < transform.position.x) ? Facings.Left : Facings.Right;
    }

    public void ToggleFacing()
    {
        Facing = (Facing == Facings.Left) ? Facings.Right : Facings.Left;
    }

    public void Start()
    {
        m_LastPosition = transform.position;
    }
	
	public void Update ()
    {
        if(AutoFromVelocity)
        {
            var velocity = transform.position - m_LastPosition;
            if(velocity.x > MinAutoTurnVelocity)
            {
                Facing = Facings.Right;
            }
            else if(velocity.x < -MinAutoTurnVelocity)
            {
                Facing = Facings.Left;
            }
        }

        var scaleX = (Facing == Facings.Right) ? 1.0f : -1.0f;
        transform.localScale = new Vector3(
            scaleX, transform.localScale.y, transform.localScale.z
        );

        m_LastPosition = transform.position;
	}

    private Vector3 m_LastPosition;
}
