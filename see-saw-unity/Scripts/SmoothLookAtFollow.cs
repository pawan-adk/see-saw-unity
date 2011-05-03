using UnityEngine;
using System.Collections;

public class SmoothLookAtFollow : MonoBehaviour
{
    public Transform LookAtTarget;	// e.g. the player
	
	Vector3 m_vCurrLA;
	Vector3 m_vTargetLA;
	
//    Vector3 m_vZoomedInPos;
    Vector3 m_vZoomedOutPos;
	
	public Vector3 ZoomedInPosOS = new Vector3(10.0f, 10.0f, 25.0f);
	public Vector3 ZoomedInLookAtOS = new Vector3(10.0f, 10.0f, 0.0f);
	
    public float ZoomedOutZDistMultiplier = -0.7759663f;
	
    public float LookAtDampingReset = 1.0f;
    public float LookAtDampingPlay = 8.0f;
    public float FollowDamping = 6.0f;
    public float MoveCamSpeed = 5.0f;

    bool m_bZoomedIn = true;

    void LateUpdate()
    {
//        if (Game.Instance.WeightDropped)
//        {
//            // Look at and dampen the rotation
//            Quaternion rotation = Quaternion.LookRotation((LookAtTarget.position + ZoomedInLookAtOS) - transform.position);
//            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtDampingPlay);
//
//            transform.position = Vector3.Lerp(transform.position, 
//                                            new Vector3(LookAtTarget.position.x + ZoomedInPosOS.x,
//                                                        LookAtTarget.position.y + ZoomedInPosOS.y, 
//                                                        transform.position.z), 
//                                                Time.deltaTime * FollowDamping);
//        }
//        else
        {
            if (m_bZoomedIn)    // zoomed in or currently zooming in
            {
                transform.position = Vector3.Lerp(transform.position, LookAtTarget.position + ZoomedInPosOS, Time.deltaTime * MoveCamSpeed);
				m_vCurrLA = Vector3.Lerp(m_vCurrLA, LookAtTarget.position + ZoomedInLookAtOS, Time.deltaTime * MoveCamSpeed);
            } 
            else   // zoomed out or currently zooming out
            {
                transform.position = Vector3.Lerp(transform.position, m_vZoomedOutPos, Time.deltaTime * MoveCamSpeed);	
				m_vCurrLA = Vector3.Lerp(m_vCurrLA, m_vTargetLA, Time.deltaTime * MoveCamSpeed);		
            }
			transform.LookAt(m_vCurrLA);
        }
    }

    // need to do this later to make sure all objects are initialized first
    public void LateStart()
    {
        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        // setup the 2 modes' transforms:
        //SetNewZoomedInVars();
        SetNewZoomedOutVars();

        // start zoomed in?
        transform.position = LookAtTarget.position + ZoomedInLookAtOS;
    }

//    public void SetNewZoomedInVars()
//    {
//        Vector3 wedgePos = GameObject.Find("Wedge").GetComponent<Wedge>().transform.position;
//        m_vZoomedInPos = wedgePos + ZoomedInPosOS;
//    }
	
    public void SetNewZoomedOutVars()
    {
        // calculate how far back we need to be to have the see-saw
        // and the goal in view
        Level lvl = GameObject.Find("Level").GetComponent<Level>();
        float zOffset = lvl.GetDistBoardToTrigger() * ZoomedOutZDistMultiplier;

        m_vZoomedOutPos = lvl.GetLevelCenterPt() + Vector3.forward * zOffset;
        m_vZoomedOutPos = new Vector3(m_vZoomedOutPos.x, LookAtTarget.position.y, m_vZoomedOutPos.z);
		m_vTargetLA = m_vCurrLA = lvl.GetLevelCenterPt();
    }

    public void ToggleZoom()
    {
        m_bZoomedIn = !m_bZoomedIn;
		
		if (!m_bZoomedIn)
			m_vTargetLA = GameObject.Find("Level").GetComponent<Level>().GetLevelCenterPt();
//		else
//			m_vTargetLA = LookAtTarget.position + ZoomedInLookAtOS;
    }
    
    public void OnReset()
    {
		if (!m_bZoomedIn)
			m_vTargetLA = GameObject.Find("Level").GetComponent<Level>().GetLevelCenterPt();
//		else
//			m_vTargetLA = LookAtTarget.position + ZoomedInLookAtOS;
    }
}