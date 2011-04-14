using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    /// <summary>
    /// x = vt * cos(theta)
    /// y = vt * sin(theta) - (0.5 * g * t^2)
    /// </summary>

    float m_fLaunchSpeed;
    float m_fGravity;

    void Start()
    {
        m_fGravity = Physics.gravity.y;
    }

    void Update()
    {

    }
}