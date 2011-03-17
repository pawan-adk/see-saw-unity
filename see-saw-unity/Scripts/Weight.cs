using UnityEngine;
using System.Collections;

public class Weight : MonoBehaviour 
{
    public float MoveSpeed = 3.0f;
    public float MaxMoveDist = 1.35f;

    float m_fCurrMoveDist;

    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    public GameObject BoardObject;
    public GameObject WedgeObject;
    public GameObject PlayerObject;

	void Start ()
    {
        rigidbody.isKinematic = true;

        m_fCurrMoveDist = 0.0f;

        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = rigidbody.position;
	}
	
	void Update ()
	{
		if (!Game.g_bWeightDropped)
		{
            if (Input.GetButton("Move Weight Up") && m_fCurrMoveDist < MaxMoveDist)
			{
                m_fCurrMoveDist += MoveSpeed * Time.deltaTime;
                transform.position += Vector3.up * MoveSpeed * Time.deltaTime;
			}		
			else if (Input.GetButton("Move Weight Down") && m_fCurrMoveDist > -MaxMoveDist)
			{
                m_fCurrMoveDist -= MoveSpeed * Time.deltaTime;
                transform.position -= Vector3.up * MoveSpeed * Time.deltaTime;
            }
		}
	}

    public void OnReset()
    {
        m_fCurrMoveDist = 0.0f;

        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }

    public void OnWeightDropped()
    {
        rigidbody.isKinematic = false;
        rigidbody.constraints = 0;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.useGravity = true;
    }

    void OnTriggerEnter(Collider body)
    {
        if (body.name == "LaunchTrigger")
        {
            BoardObject.GetComponent<Board>().OnLaunchStarted();
            WedgeObject.GetComponent<Wedge>().OnLaunchStarted();
            PlayerObject.GetComponent<Player>().OnLaunchStarted();
            Game.g_bLaunchStarted = true;
        }
    }
}
