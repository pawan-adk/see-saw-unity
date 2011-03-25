using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float AdditionalForceTime = 1.0f;
    float m_fAddForceTimer;

    Vector2 m_vDefaultForceCharControl = new Vector2(200, 200);

    public Vector3 AdditionalForceOnLaunch = new Vector2(100, 0);

    Vector3 m_vMovementForward; // used to calculate camera offset
    Vector3 m_vOrigPos;

    Quaternion m_vOrigRot;

    Trampoline_OneWay m_HitTramp;

    void Start()
    {
        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = rigidbody.position;
        m_vMovementForward = rigidbody.transform.forward;
    }

    void Update()
    {
        if (Game.Instance.LaunchStarted)
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
            m_fAddForceTimer -= Time.deltaTime;
        } 
        else
        {
        }
    }

    void FixedUpdate()
    {
        if (m_HitTramp != null && m_HitTramp.ContinueToApplyForce())
        {
            //Debug.Log("Player Vel:" + rigidbody.velocity.ToString());
            // slow down movement
            rigidbody.AddForce(m_HitTramp.Force);
        }
        if (m_fAddForceTimer > 0.0f)
            rigidbody.AddForce(AdditionalForceOnLaunch);
    }

    public void OnReset()
    {
        m_fAddForceTimer = 0.0f;
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }

    public void OnLaunchStarted()
    {
        m_fAddForceTimer = AdditionalForceTime;
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.freezeRotation = true;
    }

    public Vector3 MovementForwardScaledByVel()
    {
        return m_vMovementForward * rigidbody.velocity.x;
    }

    public void OnTrampEnter(Trampoline_OneWay _tramp)
    {
        m_HitTramp = _tramp;
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    /// 
}