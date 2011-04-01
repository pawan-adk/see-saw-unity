using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour
{
    bool m_bPlayerAttached;

    int m_nConnectedLinkIndex;

    public float ArmDist;
    public float MaxGrabDistance;

    Rigidbody[] m_lLinks;

    GameObject  m_Player;
    Player m_PlayerScript;

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
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
            if (Input.GetButtonUp("Action Btn 1"))
            {
                 Destroy(m_Player.hingeJoint);
                 m_bPlayerAttached = false;
            }
            else if (Input.GetButtonDown("Character Control Up"))
            {
                if (m_nConnectedLinkIndex + 1 < m_lLinks.Length)
                {
                    m_nConnectedLinkIndex++;
                    Vector3 vDistToHand = m_Player.transform.position - m_PlayerScript.Hand.transform.position;
                    m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex].position + vDistToHand;
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                }
            }
            else if (Input.GetButtonDown("Character Control Down"))
            {
                if (m_nConnectedLinkIndex - 1 > -1)
                {
                    m_nConnectedLinkIndex--;
                    Vector3 vDistToHand = m_Player.transform.position - m_PlayerScript.Hand.transform.position;                    
                    m_Player.transform.position = m_lLinks[m_nConnectedLinkIndex].position + vDistToHand;
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
                    Debug.Log("Current Link:" + m_nConnectedLinkIndex.ToString());
                }
            }
        }
    }

    void FixedUpdate()
    {
    }
}