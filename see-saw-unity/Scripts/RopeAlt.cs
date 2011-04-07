using UnityEngine;
using System.Collections;

public class RopeAlt : MonoBehaviour
{
    enum eClimbingStatus { CS_UP, CS_DOWN, CS_NONE, };

    bool m_bPlayerAttached;
    bool m_bMoveCompleted;
    eClimbingStatus m_eCurrClimbingStatus;

    int m_nConnectedLinkIndex;

    public float MaxGrabDistance;
    public float NextLinkThreshold = 0.25f;
    public float MoveDelay = 0.25f;
    float m_fMoveTimer;

    Rigidbody[] m_lLinks;

    GameObject m_Player;
    GameObject m_Dummy;
    Player m_PlayerScript;

    GameObject m_Hand;  // represents the current hand position
    GameObject m_Target;// represents the target position when moving up/down rope
    GameObject m_Start; // represents where the current up/down move started from

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
        m_bPlayerAttached = false;
        m_lLinks = GetComponentsInChildren<Rigidbody>();
        m_fMoveTimer = MoveDelay;

        m_Hand = GameObject.Find("TestHand");
        m_Target = GameObject.Find("ClimbTarget");
        m_Start = GameObject.Find("ClimbStart");
        m_Player = GameObject.Find("Player");
        m_PlayerScript = m_Player.GetComponent<Player>();

        //         for (int i = 0; i < m_lLinks.Length; ++i)
        //             Debug.Log("Link [" + i.ToString() + "] " + "Name:" + m_lLinks[i].name + " pos:" + m_lLinks[i].transform.position.ToString());
        //Debug.Log("Num Links:" + m_lLinks.Length.ToString());
    }

    void Update()
    {
        if (Input.GetButtonDown("Action Btn 1"))
        {
            if (!m_bPlayerAttached)
            {
                m_nConnectedLinkIndex = -1;
                // Determine which link the player's arm is nearest and "grab" onto that one:
                for (int i = 0; i < m_lLinks.Length; ++i)
                    if ((m_lLinks[i].transform.position - m_Hand.transform.position).magnitude < MaxGrabDistance)
                        m_nConnectedLinkIndex = i;

                if (m_nConnectedLinkIndex > -1)
                {
                    Debug.Log("Link found:" + m_nConnectedLinkIndex.ToString());

                    m_bPlayerAttached = true;

                    m_Dummy = (GameObject)Instantiate(m_Player);
                    GameObject hand = m_Dummy.transform.FindChild("TestHand").gameObject;
                    hand.AddComponent<HingeJoint>();
                    hand.rigidbody.constraints = RigidbodyConstraints.None;
                    hand.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    m_Dummy.AddComponent<FixedJoint>();
                    m_Dummy.GetComponent<FixedJoint>().connectedBody = hand.rigidbody;
                    m_Dummy.rigidbody.constraints = RigidbodyConstraints.None;
                    m_Dummy.rigidbody.velocity = m_Player.rigidbody.velocity;

                    DestroyImmediate(m_Dummy.collider);
                    DestroyImmediate(m_Dummy.renderer);
                    DestroyImmediate(hand.renderer);
                }
            }
        }
        else if (m_bPlayerAttached)
        {
            if (m_bMoveCompleted)
                m_fMoveTimer += Time.deltaTime;

            m_Player.rigidbody.isKinematic = true;
            m_Start.transform.position = m_lLinks[m_nConnectedLinkIndex].position;

            // get grabbing input
            if (Input.GetButtonUp("Action Btn 1"))
            {
                Destroy(m_Dummy);
                m_bPlayerAttached = false;
                m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
                m_Player.rigidbody.isKinematic = false;
                m_Player.rigidbody.velocity = m_lLinks[m_nConnectedLinkIndex].rigidbody.velocity;
            }

            // TODO:: need to determine if we want it to pause & require the player
            //        let go of the button or allow them to continually hold it
            //        in which case the movement would pause for a bit then continue to the next

            // get climbing input
            if (m_eCurrClimbingStatus == eClimbingStatus.CS_NONE)
            {
                _SetPlayerHandPos(m_Start.transform.position);
                _SetPlayerPosFromHand();

                // make sure they can still go up/down first
                if (m_fMoveTimer >= MoveDelay && 
                    Input.GetButton("Character Control Up") && 
                    m_nConnectedLinkIndex + 1 < m_lLinks.Length)
                {
                    Debug.Log("Moving up");
                    m_fMoveTimer = 0.0f;
                    m_bMoveCompleted = false;
                    m_eCurrClimbingStatus = eClimbingStatus.CS_UP;
                    m_Target.transform.position = m_lLinks[m_nConnectedLinkIndex + 1].position;
                }
                else if (m_fMoveTimer >= MoveDelay && 
                        Input.GetButton("Character Control Down") && 
                        m_nConnectedLinkIndex - 1 > -1)
                {
                    Debug.Log("Moving down");
                    m_fMoveTimer = 0.0f;
                    m_bMoveCompleted = false;
                    m_eCurrClimbingStatus = eClimbingStatus.CS_DOWN;
                    m_Target.transform.position = m_lLinks[m_nConnectedLinkIndex - 1].position;
                }
            }
            else if (m_eCurrClimbingStatus == eClimbingStatus.CS_UP)
            {
                m_Target.transform.position = m_lLinks[m_nConnectedLinkIndex + 1].position;

                Vector3 vDir = (m_lLinks[m_nConnectedLinkIndex + 1].position - m_Hand.transform.position);
                float dist = vDir.magnitude;
                Debug.Log("Dist:" + dist.ToString());

                if (dist < NextLinkThreshold)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
                    ++m_nConnectedLinkIndex;
                    Debug.Log("Climbing Up Done, index:" + m_nConnectedLinkIndex.ToString());
                    m_bMoveCompleted = true;
                }
                else
                {
                    _SetPlayerHandPos(m_Hand.transform.position + vDir.normalized * m_PlayerScript.ClimbForce * Time.deltaTime);
                    _SetPlayerPosFromHand();
                }
            }
            else if (m_eCurrClimbingStatus == eClimbingStatus.CS_DOWN)
            {
                m_Target.transform.position = m_lLinks[m_nConnectedLinkIndex - 1].position;

                Vector3 vDir = (m_lLinks[m_nConnectedLinkIndex - 1].position - m_Hand.transform.position);
                float dist = vDir.magnitude;
                Debug.Log("Dist:" + dist.ToString());

                if (dist < NextLinkThreshold)
                {
                    m_eCurrClimbingStatus = eClimbingStatus.CS_NONE;
                    --m_nConnectedLinkIndex;
                    Debug.Log("Climbing Down Done, index:" + m_nConnectedLinkIndex.ToString());
                    m_bMoveCompleted = true;
                }
                else
                {
                    _SetPlayerHandPos(m_Hand.transform.position + vDir.normalized * m_PlayerScript.ClimbForce * Time.deltaTime);
                    _SetPlayerPosFromHand();
                }
            }
        }
    }

    void LateUpdate()
    {

    }

    void FixedUpdate()
    {

    }

    /// <summary>
    /// PRIVATE
    /// </summary>
    /// <param name="_pos"></param>
    void _SetPlayerHandPos(Vector3 _pos)
    {
        m_Hand.transform.position = _pos;
    }

    void _SetPlayerPosFromHand()
    {
        m_Player.transform.position = m_Hand.transform.position + m_Player.transform.up * -m_PlayerScript.HandOffset;
    }
}