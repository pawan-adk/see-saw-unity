using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour
{
    enum eClimbingStatus { CS_UP, CS_DOWN, CS_NONE, };

    bool m_bPlayerAttached;
    eClimbingStatus m_eCurrClimbingStatus;

    int m_nConnectedLinkIndex;

    public float ArmDist;
    public float MaxGrabDistance;
    public float ClimbSpeed;
    public float NextLinkThreshold = 0.25f;

    Rigidbody[] m_lLinks;

    GameObject  m_Player;
    Player m_PlayerScript;

    Rigidbody m_Dummy;

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
        m_bPlayerAttached = false;
        m_lLinks = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < m_lLinks.Length; ++i)
            Debug.Log("Link [" + i.ToString() + "] " + "Name:" + m_lLinks[i].name + " pos:" + m_lLinks[i].transform.position.ToString());
        //Debug.Log("Num Links:" + m_lLinks.Length.ToString());

        m_Player = GameObject.Find("Player");
        m_PlayerScript = m_Player.GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Action Btn 1"))
        {
            if (!m_bPlayerAttached)
            {
                // TODO:: make this accurate obviously..

                // find the point of the hand:
                Vector3 vHand = m_Player.transform.position + m_Player.transform.up * ArmDist;

                m_nConnectedLinkIndex = -1;
                // Determine which link the player's arm is nearest and "grab" onto that one:
                for (int i = 0; i < m_lLinks.Length; ++i)
                    if ((m_lLinks[i].transform.position - vHand).magnitude < MaxGrabDistance)
                        m_nConnectedLinkIndex = i;

                if (m_nConnectedLinkIndex > -1)
                {
                    Debug.Log("Link found:" + m_nConnectedLinkIndex.ToString());

                    m_bPlayerAttached = true;
                    m_Player.AddComponent<HingeJoint>();
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    m_Player.hingeJoint.anchor += m_Player.transform.up * 0.25f;
                }
            }
        }
        else if (m_bPlayerAttached)
        {
            // get grabbing input
            if (Input.GetButtonUp("Action Btn 1"))
            {
                 Destroy(m_Player.hingeJoint);
                 m_bPlayerAttached = false;
            }

            // TODO:: need to determine if we want it to pause & require the player
            //        let go of the button or allow them to continually hold it
            //        in which case the movement would pause for a bit then continue to the next

            // get climbing input
            if (m_eCurrClimbingStatus == eClimbingStatus.CS_NONE)
            {
                if (Input.GetButtonDown("Character Control Up") && m_nConnectedLinkIndex + 1 < m_lLinks.Length)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_UP;
                    // if this is the first movement made after grabbing the rope,
                    // replace the player being jointed to the rope
                    // with a dummy object, so that we can move the player manually
                    if (m_Player.hingeJoint != null)
                    {
                        DestroyImmediate(m_Player.hingeJoint);
                        Instantiate(m_Dummy, m_lLinks[m_nConnectedLinkIndex].transform.position);
                        m_lLinks[m_nConnectedLinkIndex].AddComponent();
                    }
                }
                else if (Input.GetButtonDown("Character Control Down") && m_nConnectedLinkIndex - 1 > -1)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_DOWN;
                    // if this is the first movement made after grabbing the rope,
                    // replace the player being jointed to the rope
                    // with a dummy object, so that we can move the player manually
                    if (m_Player.hingeJoint != null)
                    {
                        DestroyImmediate(m_Player.hingeJoint);
                    }
                }
            }

            // perform climbing - up/down
            if (m_eCurrClimbingStatus == eClimbingStatus.CS_UP)
            {
                // we haven't reached the top yet...keep going
                if (m_nConnectedLinkIndex + 1 < m_lLinks.Length)
                {
                    // by getting the vector from the player's position to the hand,
                    // we know which direction to move in
                    Vector3 vDistToHand = m_Player.transform.position - m_PlayerScript.Hand.transform.position;

                    // see if we've "arrived" at the next link yet, if so, stop moving
                    if (vDistToHand.magnitude < NextLinkThreshold)
                    {
                        m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;

                        m_nConnectedLinkIndex++;
                        m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex].position + vDistToHand;
                        m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                        Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                    }
                    // otherwise we keep moving towards that link
                    else
                    {
                        m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex+1].position + (vDistToHand * ClimbSpeed * Time.deltaTime);
                    }
                }
                else  // reached the top...can't climb anymore
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
            }
            else if (m_eCurrClimbingStatus == eClimbingStatus.CS_DOWN)
            {
                if (m_nConnectedLinkIndex - 1 > -1)
                {
                    // by getting the vector from the player's position to the hand,
                    // we know which direction to move in
                    Vector3 vDistToHand = m_Player.transform.position - m_PlayerScript.Hand.transform.position;

                    // see if we've "arrived" at the next link yet, if so, stop moving
                    if (vDistToHand.magnitude < NextLinkThreshold)
                    {
                        m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;

	                    m_nConnectedLinkIndex--;
                        m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex].position + vDistToHand;
                        m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                        Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                    }
                    // otherwise we keep moving towards that link
                    else
                    {
                        m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex-1].position + (vDistToHand * ClimbSpeed * Time.deltaTime);
                    }
                }
                else  // reached the bottom...can't climb anymore
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
            }
        }
    }

    void FixedUpdate()
    {
    }
}