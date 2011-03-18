using UnityEngine;
using System.Collections;

public class SmoothLookAtFollow : MonoBehaviour
{
    GameObject m_ZoomInOutBtn;
    Game m_Game;

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
    public float ZoomedInYLAOffset = 10.0f;

    public float ZoomedOutYPosOffset = 15.0f;
    //public float ZoomedOutYLAOffset = 10.0f;

    bool m_bZoomedIn = true;
    bool m_bResetting = false;

    void LateUpdate()
    {
        if (Game.g_bWeightDropped)
        {
            // Look at and dampen the rotation
            Quaternion rotation = Quaternion.LookRotation(LookAtTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtDampingPlay);
            
            transform.position = Vector3.Lerp(transform.position, new Vector3(LookAtTarget.position.x, transform.position.y, transform.position.z), Time.deltaTime * FollowDamping);
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
                if (m_ZoomInOutBtn.guiTexture.HitTest(Input.mousePosition))
                {
                    m_bZoomedIn = !m_bZoomedIn;
                    m_bResetting = false;
                }

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

    void Start()
    {
        m_ZoomInOutBtn = GameObject.Find("ZoomInOutButton");
        m_Game = GameObject.Find("Game").GetComponent<Game>();

        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        // setup the 2 modes' transforms:
        Vector3 wedgePos = GameObject.Find("Wedge").GetComponent<Wedge>().transform.position;
        m_vZoomedInPos = wedgePos;
        m_vZoomedInPos += Vector3.forward * -ZoomedInZPosOffset;
        m_vZoomedInPos += Vector3.up * ZoomedInYPosOffset;
        m_vZoomedInLA = wedgePos + Vector3.up * ZoomedInYLAOffset;

        // TODO:: do the calculation to have the whole level in view
        //        or if it's too large, just a certain section..
        m_vZoomedOutPos = m_Game.GetLevelCenterLookAt() + Vector3.forward * -50.0f;
        m_vZoomedOutPos = new Vector3(m_vZoomedOutPos.x, m_vZoomedInPos.y, m_vZoomedOutPos.z);

        // start zoomed in?
        // TODO:: probably set up a start for each level
        //        to be zoomed out at first, wait a bit, 
        //        then zoom in
        transform.position = m_vZoomedInPos;
        transform.LookAt(m_vZoomedInLA);
    }
    
    public void OnReset()
    {
        // TODO:: smooth interp to final position and look at on reset:
        m_bResetting = true;
        m_bZoomedIn = true;
//         Vector3 wedgePos = GameObject.Find("Wedge").GetComponent<Wedge>().transform.position;
//         transform.position = m_vZoomedInPos;
//        transform.LookAt(wedgePos + Vector3.up * ZoomedInYLAOffset);
    }
}