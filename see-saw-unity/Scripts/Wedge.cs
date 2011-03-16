using UnityEngine;
using System.Collections;

public class Wedge : MonoBehaviour 
{
    public float MoveSpeed = 1.5f;
    public float MaxMoveDist = 0.25f;

    float m_fCurrMoveDist;

    Vector3 m_vParentOffset;

    GameObject BoardObj;    // the see-saw board (to move the hinge)

	void Start () 
	{
        BoardObj = GameObject.Find("Board");
        
        rigidbody.freezeRotation = true;

        rigidbody.isKinematic = true;

        m_fCurrMoveDist = 0.0f;

        m_vParentOffset = transform.parent.transform.position;
    }
	
	void Update () 
	{
        if (!Game.g_bWeightDropped)
        {
            if (Input.GetButton("Move Wedge Right") && m_fCurrMoveDist < MaxMoveDist)
            {
                m_fCurrMoveDist += MoveSpeed * Time.deltaTime;
                if (m_fCurrMoveDist > MaxMoveDist)
                    m_fCurrMoveDist = MaxMoveDist;

                transform.position = new Vector3(m_vParentOffset.x + m_fCurrMoveDist, transform.position.y, transform.position.z);
                BoardObj.hingeJoint.anchor = new Vector3(m_fCurrMoveDist / BoardObj.GetComponent<Board>().HalfBoardLength, BoardObj.hingeJoint.anchor.y, BoardObj.hingeJoint.anchor.z);
            }
            else if (Input.GetButton("Move Wedge Left") && m_fCurrMoveDist > -MaxMoveDist)
            {
                m_fCurrMoveDist -= MoveSpeed * Time.deltaTime;
                if (m_fCurrMoveDist < -MaxMoveDist)
                    m_fCurrMoveDist = -MaxMoveDist;

                transform.position = new Vector3(m_vParentOffset.x + m_fCurrMoveDist, transform.position.y, transform.position.z);
                BoardObj.hingeJoint.anchor = new Vector3(m_fCurrMoveDist / BoardObj.GetComponent<Board>().HalfBoardLength, BoardObj.hingeJoint.anchor.y, BoardObj.hingeJoint.anchor.z);
            }
        }
	}

    public void OnReset()
    {
        m_fCurrMoveDist = 0.0f;
        rigidbody.isKinematic = true;
    }

    public void OnLaunchStarted()
    {
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.useGravity = true;
    }
}
