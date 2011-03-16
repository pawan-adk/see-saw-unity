using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
    float m_fHalfBoardLength;

    void Start() 
    {
		rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
		rigidbody.freezeRotation = true;

        HalfBoardLength = this.collider.bounds.size.x * 0.5f;
    }
	
	void Update () 
	{

	}

    public void OnReset()
    {
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    public void OnLaunchStarted()
    {
        rigidbody.constraints = 0;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
        rigidbody.useGravity = true;
    }



    /// <summary>
    /// PROPERTIES
    /// </summary>
    public float HalfBoardLength
    {
        get { return m_fHalfBoardLength; }
        set { m_fHalfBoardLength = value; }
    }
}
