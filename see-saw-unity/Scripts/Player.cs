using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public Vector3 AdditionalForceOnLaunch = new Vector2(10000, 1000);

    Vector2 m_vDefaultForceCharControl = new Vector2(200, 200);

    void Start()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
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
        } 
        else
        {
        }
    }

    public void OnReset()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.useGravity = false;
    }

    public void OnLaunchStarted()
    {
        //rigidbody.freezeRotation = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.useGravity = true;
        rigidbody.AddForce(AdditionalForceOnLaunch.x, AdditionalForceOnLaunch.y, 0);
    }
}