using UnityEngine;
using System.Collections;

public class SmoothLookAtFollow : MonoBehaviour
{
    public Transform LookAtTarget;

    Vector3 m_vZoomedInPos;
    Vector3 m_vZoomedOutPos;
    Vector3 m_vZoomedInLA;

    public float LookAtDampingReset = 1.0f;
    public float LookAtDampingPlay = 8.0f;
    public float FollowDamping = 6.0f;
    public float MoveCamSpeed = 5.0f;

    public float ZoomedInZPosOffset = 25.0f;
    public float ZoomedInYPosOffset = 10.0f;
	public float ZoomedInXPosOffset = 10.0f;
    public float ZoomedInYLAOffset = 10.0f;
	public float ZoomedInXLAOffset = 10.0f;

    public float ZoomedOutYPosOffset = 15.0f;
    public float ZoomedOutZDistMultiplier = -0.7759663f;
    //public float ZoomedOutYLAOffset = 10.0f;

    bool m_bZoomedIn = true;
    bool m_bResetting = false;

    void LateUpdate()
    {
        if (Game.Instance.WeightDropped)
        {
            // Look at and dampen the rotation
            Quaternion rotation = Quaternion.LookRotation(LookAtTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtDampingPlay);

            transform.position = Vector3.Lerp(transform.position, 
                                            new Vector3(LookAtTarget.position.x,
                                                        LookAtTarget.position.y, 
                                                        transform.position.z), 
                                                Time.deltaTime * FollowDamping);
        }
        else
        {
            if (m_bZoomedIn)    // zoomed in or currently zooming in
            {
                // Look at and dampen the rotation
                if (m_bResetting)
                {
                    Quaternion rotation = Quaternion.LookRotation(m_vZoomedInLA - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtDampingReset);
                }
                transform.position = Vector3.Lerp(transform.position, m_vZoomedInPos, Time.deltaTime * MoveCamSpeed);
            } 
            else   // zoomed out or currently zooming out
            {
                transform.position = Vector3.Lerp(transform.position, m_vZoomedOutPos, Time.deltaTime * MoveCamSpeed);
            }
        }
    }

    // need to do this later to make sure all objects are initialized first
    public void LateStart()
    {
        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        // setup the 2 modes' transforms:
        SetNewZoomedInVars();
        SetNewZoomedOutVars();

        // start zoomed in?
        transform.position = m_vZoomedInPos;
        transform.LookAt(m_vZoomedInLA);
    }

    public void SetNewZoomedInVars()
    {
        Vector3 wedgePos = GameObject.Find("Wedge").GetComponent<Wedge>().transform.position;
        m_vZoomedInPos = wedgePos + Vector3.right * ZoomedInXPosOffset;
        m_vZoomedInPos += Vector3.forward * -ZoomedInZPosOffset;
        m_vZoomedInPos += Vector3.up * ZoomedInYPosOffset;
        m_vZoomedInLA = wedgePos + Vector3.up * ZoomedInYLAOffset;
		m_vZoomedInLA += Vector3.right * ZoomedInXLAOffset;
    }
    public void SetNewZoomedOutVars()
    {
        // calculate how far back we need to be to have the see-saw
        // and the goal in view
        Level lvl = GameObject.Find("Level").GetComponent<Level>();
        float zOffset = lvl.GetDistBoardToTrigger() * ZoomedOutZDistMultiplier;

        m_vZoomedOutPos = lvl.GetLevelCenterPt() + Vector3.forward * zOffset;
        m_vZoomedOutPos = new Vector3(m_vZoomedOutPos.x, m_vZoomedInPos.y, m_vZoomedOutPos.z);
    }

    public void ToggleZoom()
    {
        m_bZoomedIn = !m_bZoomedIn;
        m_bResetting = false;
    }
    
    public void OnReset()
    {
        m_bResetting = true;
        m_bZoomedIn = true;
    }
}