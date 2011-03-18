using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
    public static bool g_bWeightDropped = false;    // player started the sequence, the weight has been dropped
    public static bool g_bLaunchStarted = false;    // has the LaunchTrigger been hit by the weight yet?

    public GameObject Ground;

    public GameObject WeightObject;
    public GameObject PlayerObject;
    public GameObject WedgeObject;
    public GameObject BoardObject;
    public GameObject PlayerCameraObject;

    void Start()
    {
        renderer.enabled = false;
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Reset"))
        {
            Debug.Log("Reset");
            g_bWeightDropped = g_bLaunchStarted = false;

            WeightObject.GetComponent<Weight>().OnReset();
            PlayerObject.GetComponent<Player>().OnReset();
            WedgeObject.GetComponent<Wedge>().OnReset();
            BoardObject.GetComponent<Board>().OnReset();
            PlayerCameraObject.GetComponent<SmoothLookAtFollow>().OnReset();
        }
        else if (Input.GetButtonDown("Drop Weight"))
        {
            Debug.Log("Weight Dropped");
            g_bWeightDropped = true;
            WeightObject.GetComponent<Weight>().OnWeightDropped();
        }
    }

    public Vector3 GetLevelCenterLookAt()
    {
        return (Ground.collider.bounds.min + Ground.collider.bounds.max) * 0.5f;
    }
}