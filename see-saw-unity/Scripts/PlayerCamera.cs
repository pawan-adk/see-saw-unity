using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public float MoveCamSpeed = 5.0f;
    public float PosYOffset = 1.0f;
    public float MaxZDist = 2.0f;

    float m_fCurrZDist;

    Vector3 m_vCurrLookAt;

    public GameObject m_PlayerObject;

    void Start()
    {
        Vector3 playerPos = m_PlayerObject.transform.position;
        Vector3 newPos = new Vector3(playerPos.x, playerPos.y + PosYOffset, transform.position.z);
        transform.position = newPos;

        m_vCurrLookAt = playerPos;
        transform.LookAt(m_vCurrLookAt);

        m_fCurrZDist = 0.0f;
    }

    void Update()
    {
        if (!Game.Instance.WeightDropped)
        {
            // "pan"
	        if (Input.GetButton("Pan Left"))
	        {
                m_vCurrLookAt -= Vector3.right * MoveCamSpeed * Time.deltaTime;
	        }
	        else if (Input.GetButton("Pan Right"))
	        {
                m_vCurrLookAt += Vector3.right * MoveCamSpeed * Time.deltaTime;
            }
            // "zoom"
            if (Input.GetButton("Zoom Out") && m_fCurrZDist > -MaxZDist)
            {
                m_fCurrZDist -= MoveCamSpeed * Time.deltaTime;
                transform.position -= Vector3.forward * MoveCamSpeed * Time.deltaTime;
            }
            else if (Input.GetButton("Zoom In") && m_fCurrZDist < MaxZDist)
            {
                m_fCurrZDist += MoveCamSpeed * Time.deltaTime;
                transform.position += Vector3.forward * MoveCamSpeed * Time.deltaTime;
            }
        }
        else
        {
            m_vCurrLookAt = m_PlayerObject.transform.position;
        }

        m_vCurrLookAt += m_PlayerObject.GetComponent<Player>().MovementForwardScaledByVel();
        Vector3 newPos = new Vector3(m_vCurrLookAt.x, m_vCurrLookAt.y + PosYOffset, transform.position.z);
        transform.position = newPos;

        transform.LookAt(m_vCurrLookAt);
    }

    public void OnReset()
    {
        m_fCurrZDist = 0.0f;

        Vector3 playerPos = m_PlayerObject.transform.position;
        Vector3 newPos = new Vector3(playerPos.x, playerPos.y + PosYOffset, transform.position.z);
        transform.position = newPos;
        m_vCurrLookAt = playerPos;
    }
}