using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour
{
    bool m_bPlayerAttached;

    public float MaxGrabDistance;

    Rigidbody[] m_lLinks;

    GameObject m_Player;

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_bPlayerAttached = false;
        m_lLinks = GetComponentsInChildren<Rigidbody>();
        //Debug.Log("Num Links:" + m_lLinks.Length.ToString());

        m_Player = GameObject.Find("Player");
    }

    void Update()
    {
        if (Input.GetButtonDown("Action Btn 1"))
        {
            if (!m_bPlayerAttached)
            {
                // TODO:: make this accurate obviously..

                // find the point of the hand:
                Vector3 vHand = m_Player.transform.position + m_Player.transform.up * 3.0f;

                int index = -1;
                // Determine which link the player's arm is nearest and "grab" onto that one:
                for (int i = 0; i < m_lLinks.Length; ++i)
                    if ((m_lLinks[i].transform.position - vHand).magnitude < MaxGrabDistance)
                        index = i;

                if (index > -1)
                {
                    //Debug.Log("Link found:" + index.ToString());

                    m_bPlayerAttached = true;
                    m_Player.AddComponent<HingeJoint>();
                    m_Player.hingeJoint.connectedBody = m_lLinks[index];
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
        }
    }
}