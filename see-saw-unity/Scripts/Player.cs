using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    float m_fSamplePosTimer;    // how often to save the current position as previous

    public Vector3 AdditionalForceOnLaunch = new Vector2(10000, 1000);

    Vector2 m_vDefaultForceCharControl = new Vector2(200, 200);

    Vector3 m_vMovementForward; // used to calculate camera offset
    Vector3 m_vPrevPos;
    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    void Start()
    {
        m_fSamplePosTimer = 0.0f;

        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = m_vPrevPos = rigidbody.position;
        m_vMovementForward = rigidbody.transform.forward;
    }

    void Update()
    {
        if (Game.g_bLaunchStarted)
        {
	        if (Input.GetButton("Character Control Up"))
	        {
	            rigidbody.AddForce(m_vDefaultForceCharControl.x * 0.0f, m_vDefaultForceCharControl.y, 0.0f);
	        }
	        else if (Input.GetButton("Character Control Down"))
	        {
	            rigidbody.AddForce(m_vDefaultForceCharControl.x * 0.0f, -m_vDefaultForceCharControl.y, 0.0f);
	        }
	        else if (Input.GetButton("Character Control Right"))
	        {
	            rigidbody.AddForce(m_vDefaultForceCharControl.x, m_vDefaultForceCharControl.y * 0.0f, 0.0f);
	        }
	        else if (Input.GetButton("Character Control Left"))
	        {
	            rigidbody.AddForce(-m_vDefaultForceCharControl.x, m_vDefaultForceCharControl.y * 0.0f, 0.0f);
	        }
            m_fSamplePosTimer += Time.deltaTime;

            if (m_fSamplePosTimer > 0.01f)
            {
                m_vMovementForward = Vector3.Normalize(rigidbody.position - m_vPrevPos);
                m_fSamplePosTimer = 0.0f;
                m_vPrevPos = rigidbody.position;
            }
        } 
        else
        {
        }
    }

    public void OnReset()
    {
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }

    public void OnLaunchStarted()
    {
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.AddForce(AdditionalForceOnLaunch.x, AdditionalForceOnLaunch.y, 0);
    }

    public Vector3 MovementForwardScaledByVel()
    {
        return m_vMovementForward * rigidbody.velocity.x;
    }
}