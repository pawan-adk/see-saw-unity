using UnityEngine;
using System.Collections;

public class SmoothLookAtFollow : MonoBehaviour
{
    public Transform target;

    Vector3 m_vCurrLookAt;
    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    public float LookAtDamping = 8.0f;
    public float FollowDamping = 6.0f;
    public float MoveCamSpeed = 5.0f;
    public float MaxZDist = 2.0f;

    float m_fCurrZDist;

    public bool smooth = true;

    void LateUpdate()
    {
        if (target)
        {
            if (Game.g_bWeightDropped)
            {
	            if (smooth)
	            {
	                // Look at and dampen the rotation
	                Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
	                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtDamping);
	                
	                transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, transform.position.y, transform.position.z), Time.deltaTime * FollowDamping);
	            }
	            else
	            {
	                // Just lookat
	                transform.LookAt(target);
	            }
            }
            else
            {
                // "pan"
                if (Input.GetButton("Pan Left"))
                {
                    m_vCurrLookAt -= Vector3.right * MoveCamSpeed * Time.deltaTime;
                    transform.position -= Vector3.right * MoveCamSpeed * Time.deltaTime;
                }
                else if (Input.GetButton("Pan Right"))
                {
                    m_vCurrLookAt += Vector3.right * MoveCamSpeed * Time.deltaTime;
                    transform.position += Vector3.right * MoveCamSpeed * Time.deltaTime;
                }
                // "zoom"
                if (Input.GetButton("Zoom Out") && m_fCurrZDist > -MaxZDist)
                {
                    m_fCurrZDist -= MoveCamSpeed * Time.deltaTime;
                    transform.position -= Vector3.forward * MoveCamSpeed * Time.deltaTime;
                }
                else if (Input.GetButton("Zoom In") && m_fCurrZDist < MaxZDist)
                {
                    m_fCurrZDist += MoveCamSpeed * Time.deltaTime;
                    transform.position += Vector3.forward * MoveCamSpeed * Time.deltaTime;
                }

                transform.LookAt(m_vCurrLookAt);
            }
        }
    }

    void Start()
    {
        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        m_vOrigRot = new Quaternion(0,0,0,1);
        m_vOrigPos = transform.position;
        m_vCurrLookAt = target.position;
    }
    
    public void OnReset()
    {
        // TODO:: smooth interp to final position and look at on reset:

        m_fCurrZDist = 0.0f;

        transform.rotation = m_vOrigRot;
        transform.position = m_vOrigPos;
        m_vCurrLookAt = target.position;
    }
}