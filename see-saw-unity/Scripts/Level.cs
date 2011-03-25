using UnityEngine;
using System.Collections;

/// <summary>
/// Contains information & functionality that all levels will require
/// </summary>
public class Level : MonoBehaviour
{
    public GameObject ZoomInOutBtn;
    
    //GameObject Ground;
    GameObject GoalTriggerObject;
    GameObject WeightObject;
    GameObject PlayerObject;
    GameObject WedgeObject;
    GameObject BoardObject;
    GameObject PlayerCameraObject;

    Game m_Game;
    SmoothLookAtFollow m_PlayerCam;

    public float LevelPreviewTime = 3.0f;
    public float ResetTime = 2.0f;
    float m_fResetTimer = -1.0f;

    bool m_bWeightDropped = false;    // player started the sequence, the weight has been dropped
    bool m_bLaunchStarted = false;    // has the LaunchTrigger been hit by the weight yet?

    void Start()
    {
        m_Game = Game.Instance;

        PlayerInfo playerInfo = m_Game.NewLevel(this);
        if (playerInfo == null)
        {
            m_Game.PlayerInfo = gameObject.AddComponent("PlayerInfo") as PlayerInfo;

            // TODO:: get this info from saved file:
            m_Game.PlayerInfo.Init(PlayerInfo.g_nDefaultLives, PlayerInfo.g_nDefaultHealth);
        }
        
        _InitCommonObjects();
        m_PlayerCam.LateStart();
        m_PlayerCam.ToggleZoom();
    }

    void Update()
    {
        if (LevelPreviewTime > 0.0f)
            LevelPreviewTime -= Time.deltaTime;

        if (LevelPreviewTime < 0.0f)
        {
            LevelPreviewTime = 0.0f;
            Game.Instance.PreviewDone = true;
            m_PlayerCam.ToggleZoom();
        }

        if (LevelPreviewTime == 0.0f)
            m_Game.Update();

        if (m_fResetTimer > 0.0f)
        {
            m_fResetTimer -= Time.deltaTime;
            if (m_fResetTimer < 0.0f)
                m_fResetTimer = 0.0f;
        }
        if (m_fResetTimer == 0.0f)
            _Reset();
    }

    void LateUpdate()
    {
        if (LevelPreviewTime == 0.0f)
        {
	        if (Input.GetButtonDown("Reset"))
	        {
	            _Reset();
	        }
	        else if (Input.GetButtonDown("Drop Weight"))
	        {
	            m_Game.WeightDropped = true;
                if (WeightObject)
	                WeightObject.GetComponent<Weight>().OnWeightDropped();
	        }
	        else if (Input.GetMouseButtonUp(0))
	            if (ZoomInOutBtn != null && ZoomInOutBtn.guiTexture.HitTest(Input.mousePosition))
	                m_PlayerCam.ToggleZoom();
	        
	        m_Game.LateUpdate();
        }
    }

//     void OnGUI()
//     {
//         GUI.Label(new Rect(50, 5, 100, 40), Game.Instance.PlayerInfo.Lives.ToString());
//     }


    public void OnGoalReached()
    {
        Game.Instance.OnGoalReached();
    }

    // called after dying, but player still has lives
    public void ResetLevel()
    {
        m_fResetTimer = ResetTime;
    }
    
    public Vector3 GetLevelCenterLookAt()
    {
        // take the see saw's board's center & the goal trigger's center
        // and look at the center point of that:
        return (BoardObject.transform.position + GoalTriggerObject.transform.position) * 0.5f;
    }

    public float GetDistBoardToTrigger()
    {
        return (BoardObject.transform.position - GoalTriggerObject.transform.position).magnitude;
    }

    // private:
    private void _InitCommonObjects()
    {
        Debug.Log("Init Common Objs");
        GoalTriggerObject = GameObject.Find("GoalTrigger");
        WeightObject = GameObject.Find("Weight");
        PlayerObject = GameObject.Find("Player");
        WedgeObject = GameObject.Find("Wedge");
        BoardObject = GameObject.Find("Board");
        PlayerCameraObject = GameObject.Find("PlayerCamera");
        m_PlayerCam = PlayerCameraObject.GetComponent<SmoothLookAtFollow>();
    }
    private void _Reset()
    {
        Debug.Log("Resetting Level");
        m_Game.WeightDropped = m_Game.LaunchStarted = false;
        m_fResetTimer = -1.0f; // means no longer resetting
        WeightObject.GetComponent<Weight>().OnReset();
        PlayerObject.GetComponent<Player>().OnReset();
        WedgeObject.GetComponent<Wedge>().OnReset();
        BoardObject.GetComponent<Board>().OnReset();
        m_PlayerCam.OnReset();
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    public bool WeightDropped
    {
        get { return m_bWeightDropped; }
        set { m_bWeightDropped = value; }
    }
    public bool LaunchStarted
    {
        get { return m_bLaunchStarted; }
        set { m_bLaunchStarted = value; }
    }
    public bool Resetting
    {
        get { return m_fResetTimer > 0.0f; }
    }
}