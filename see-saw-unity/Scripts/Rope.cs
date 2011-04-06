using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour
{
    enum eClimbingStatus { CS_UP, CS_DOWN, CS_NONE, };

    bool m_bPlayerAttached;
    eClimbingStatus m_eCurrClimbingStatus;

    int m_nConnectedLinkIndex;

    public float MaxGrabDistance;
    public float ClimbSpeed;
    public float NextLinkThreshold = 0.25f;

    Rigidbody[] m_lLinks;

    GameObject  m_Player;
    Vector3 m_vSimHandPos;
    Player m_PlayerScript;

    GameObject m_Hand;
    GameObject m_Target;
    GameObject m_Start;
    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
        m_bPlayerAttached = false;
        m_lLinks = GetComponentsInChildren<Rigidbody>();

        m_Hand = GameObject.Find("TestHand");
        m_Target = GameObject.Find("ClimbTarget");
        m_Start = GameObject.Find("ClimbStart");

//         for (int i = 0; i < m_lLinks.Length; ++i)
//             Debug.Log("Link [" + i.ToString() + "] " + "Name:" + m_lLinks[i].name + " pos:" + m_lLinks[i].transform.position.ToString());
        //Debug.Log("Num Links:" + m_lLinks.Length.ToString());

        m_Player = GameObject.Find("Player");
        m_PlayerScript = m_Player.GetComponent<Player>();
    }

    void Update()
    {
        // find the point of the hand:
        //m_vSimHandPos = m_Player.transform.position + m_Player.transform.up * m_PlayerScript.HandOffset;

        if (Input.GetButtonDown("Action Btn 1"))
        {
            if (!m_bPlayerAttached)
            {
                // TODO:: make this accurate obviously..

                // find the point of the hand:
                m_vSimHandPos = m_Player.transform.position + m_Player.transform.up * m_PlayerScript.HandOffset;

                m_nConnectedLinkIndex = -1;
                // Determine which link the player's arm is nearest and "grab" onto that one:
                for (int i = 0; i < m_lLinks.Length; ++i)
                    if ((m_lLinks[i].transform.position - m_vSimHandPos).magnitude < MaxGrabDistance)
                        m_nConnectedLinkIndex = i;

                if (m_nConnectedLinkIndex > -1)
                {
                    Debug.Log("Link found:" + m_nConnectedLinkIndex.ToString());

                    m_bPlayerAttached = true;
                    m_Player.AddComponent<HingeJoint>();
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    m_Player.hingeJoint.anchor += m_Player.transform.up * 0.25f;

                    m_vSimHandPos = m_Player.hingeJoint.connectedBody.transform.position;
                    m_Player.transform.position = m_vSimHandPos + m_Player.transform.up * -m_PlayerScript.HandOffset;
                }
            }
        }
        else if (m_bPlayerAttached)
        {
            // get grabbing input
//             if (Input.GetButtonUp("Action Btn 1"))
//             {
//                  Destroy(m_Player.hingeJoint);
//                  m_bPlayerAttached = false;
//             }

            // TODO:: need to determine if we want it to pause & require the player
            //        let go of the button or allow them to continually hold it
            //        in which case the movement would pause for a bit then continue to the next

            // get climbing input
            if (m_eCurrClimbingStatus == eClimbingStatus.CS_NONE)
            {   // make sure they can still go up/down first
                if (Input.GetButtonDown("Character Control Up") && m_nConnectedLinkIndex+1 < m_lLinks.Length)
                {
                    Debug.Log("Moving up");
                    m_eCurrClimbingStatus = eClimbingStatus.CS_UP;
                    m_Player.hingeJoint.connectedBody = null;
                }
                else if (Input.GetButtonDown("Character Control Down") && m_nConnectedLinkIndex-1 > -1)
                {
                    Debug.Log("Moving down");
                    m_eCurrClimbingStatus = eClimbingStatus.CS_DOWN;
                    m_Player.hingeJoint.connectedBody = null;
                }
            }
        }

        m_Hand.transform.position = m_vSimHandPos;
    }

    void FixedUpdate()
    {
        // perform climbing - up/down
        if (m_eCurrClimbingStatus == eClimbingStatus.CS_UP)
        {
            // we haven't reached the top yet...keep going
            if (m_nConnectedLinkIndex + 1 < m_lLinks.Length)
            {
                // Get the vector to the next link from the curr link //simulated hand position
                // this is the direction the character needs to move in
                Vector3 vToNextLink = m_lLinks[m_nConnectedLinkIndex + 1].position - m_lLinks[m_nConnectedLinkIndex].position;

                //Debug.Log("Dist to next link:" + vToNextLink.magnitude.ToString());

                // move the hand pos towards the next link
                m_vSimHandPos += (vToNextLink.normalized * ClimbSpeed);

                // set the position according to the hand and the offset
                m_Player.transform.position = m_vSimHandPos + m_Player.transform.up * -m_PlayerScript.HandOffset;

                // see if we've "arrived" at the next link yet, if so, stop moving
                if ((m_vSimHandPos - m_lLinks[m_nConnectedLinkIndex + 1].position).magnitude < NextLinkThreshold)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;

                    m_nConnectedLinkIndex++;
                    DestroyImmediate(m_Player.hingeJoint);
                    m_Player.rigidbody.velocity = Vector3.zero;
                    m_Player.AddComponent<HingeJoint>();
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    m_Player.hingeJoint.anchor += m_Player.transform.up * 0.25f;

                    Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                }
                // otherwise we keep moving towards that link
//                 else
//                 {
//                     m_vSimHandPos += (vToNextLink.normalized * ClimbSpeed);
//                     m_Player.transform.position = m_vSimHandPos + m_Player.transform.up * -m_PlayerScript.HandOffset;
//                 }
            }
            else  // reached the top...can't climb anymore
            {
                Debug.Log("DONE Moving up");
                m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
            }
        }
        else if (m_eCurrClimbingStatus == eClimbingStatus.CS_DOWN)
        {
            if (m_nConnectedLinkIndex - 1 > -1)
            {
                // by getting the vector from the player's position to the hand,
                // we know which direction to move in
                Vector3 vToNextLink = m_lLinks[m_nConnectedLinkIndex - 1].position - m_vSimHandPos;

                // see if we've "arrived" at the next link yet, if so, stop moving
                if (vToNextLink.magnitude < NextLinkThreshold)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;

                    m_nConnectedLinkIndex--;
                    DestroyImmediate(m_Player.hingeJoint);
                    m_Player.rigidbody.velocity = Vector3.zero;
                    m_Player.AddComponent<HingeJoint>();
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    m_Player.hingeJoint.anchor += m_Player.transform.up * 0.25f;

                    Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                }
                // otherwise we keep moving towards that link
                else
                {
                    m_vSimHandPos += (vToNextLink.normalized * ClimbSpeed);
                    m_Player.transform.position = m_vSimHandPos + m_Player.transform.up * -m_PlayerScript.HandOffset;
                }
            }
            else  // reached the bottom...can't climb anymore
            {
                Debug.Log("DONE Moving down");
                m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
            }
        }
    }
}