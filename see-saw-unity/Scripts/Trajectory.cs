using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    public int NumSamples;  // how many points we will calculate & draw the trajectory at

    public float TimeInterval;  // how often to take a sample

    public GameObject TrajObj;

    /// <summary>
    /// x = vt * cos(theta)
    /// y = vt * sin(theta) - (0.5 * g * t^2)
    /// </summary>

    float m_fGravity;
    float m_fAngle;

    Vector3 m_vVelocity;

    GameObject[] m_Trajectory;

    GameObject m_Player;

    void Start()
    {
        Debug.Log("Traj Start");

        m_fGravity = -Physics.gravity.y;
        Debug.Log("Grav:" + m_fGravity.ToString());
        m_fAngle = 45.0f;
        m_Player = GameObject.Find("Player");
        m_vVelocity = new Vector3(50.0f, 50.0f, 0.0f);
        m_Trajectory = new GameObject[NumSamples];
        for (int i = 0; i < NumSamples; ++i)
            m_Trajectory[i] = (GameObject)Instantiate(TrajObj);
    }

    void Update()
    {
        OnWedgeMoved();
        OnWeightMoved();

        float x, y, t;
        for (int i = 0; i < NumSamples; ++i)
        {
            t = (((float)i+1) * TimeInterval);
            x = m_vVelocity.x * t * Mathf.Cos(m_fAngle);
            y = m_vVelocity.y * t * Mathf.Sin(m_fAngle) - (0.5f * m_fGravity * t * t);
            m_Trajectory[i].transform.position = new Vector3(x, y, 0.0f);
            m_Trajectory[i].transform.position += m_Player.transform.position;
        }
    }

    public void OnWeightMoved()
    {
        // Calculate the velocity
        if (Input.GetButton("Move Weight Up"))
            m_vVelocity += new Vector3(10.0f, 10.0f, 0.0f) * Time.deltaTime;
        else if (Input.GetButton("Move Weight Down"))
            m_vVelocity -= new Vector3(10.0f, 10.0f, 0.0f) * Time.deltaTime;
    }

    public void OnWedgeMoved()
    {
        // Calculate the angle
        if (Input.GetButton("Move Wedge Right"))
            m_fAngle -= Time.deltaTime;
        else if (Input.GetButton("Move Wedge Left"))
            m_fAngle += Time.deltaTime;
    }
}