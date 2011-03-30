using UnityEngine;
using System.Collections;

/// <summary>
/// Contains information & functionality that all levels will require
/// </summary>
public class Level : MonoBehaviour
{
    public GameObject ZoomInOutBtn;

    GUIStyle m_GUIStyle = new GUIStyle();
    
    //GameObject Ground;
    GameObject m_GoalTriggerObject;
    GameObject m_PlayerObject;
    GameObject m_PlayerCameraObject;
    GameObject m_SeeSawObject;

    Game m_Game;
    SmoothLookAtFollow m_PlayerCam;

    public float LevelPreviewTime = 3.0f;
    public float ResetTime = 2.0f;
    float m_fResetTimer = -1.0f;

    bool m_bWeightDropped = false;    // player started the sequence, the weight has been dropped
    bool m_bLaunchStarted = false;    // has the LaunchTrigger been hit by the weight yet?
    bool m_bPlayerCameToRest = false;

    void Start()
    {
        m_GUIStyle.fontSize = 34;

        m_Game = Game.Instance;

        PlayerInfo playerInfo = m_Game.NewLevel(this);
        if (playerInfo == null)
        {
            m_Game.PlayerInfo = gameObject.AddComponent("PlayerInfo") as PlayerInfo;

            // TODO:: get this info from saved file:
            m_Game.PlayerInfo.Init(PlayerInfo.g_nDefaultLives);
        }
        
        _InitCommonObjects();
        m_PlayerCam.LateStart();
        m_PlayerCam.ToggleZoom();
    }

    void Update()
    {
        //////////////////////////////////////////////////////////////////////////
        // Level Preview
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
        //////////////////////////////////////////////////////////////////////////
        // Level Reset
        if (m_fResetTimer > 0.0f)
        {
            m_fResetTimer -= Time.deltaTime;
            if (m_fResetTimer < 0.0f)
                m_fResetTimer = 0.0f;
        }
        if (m_fResetTimer == 0.0f)
        {
            if (!m_bPlayerCameToRest)
                _Reset();
            else
                _ResetToNewPosition();
        }
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
                if (m_SeeSawObject)
	                m_SeeSawObject.GetComponent<SeeSaw>().OnWeightDropped();
	        }
	        else if (Input.GetMouseButtonUp(0))
	            if (ZoomInOutBtn != null && ZoomInOutBtn.guiTexture.HitTest(Input.mousePosition))
	                m_PlayerCam.ToggleZoom();
	        
	        m_Game.LateUpdate();
        }
    }

    void OnGUI()
    {
        if (Application.loadedLevel > 0 && Application.loadedLevel <= Game.Instance.NumActualLevelsInWorld)
        { 
            GUI.Label(new Rect(150, 5, 100, 40), Game.Instance.PlayerInfo.Lives.ToString() + " Lives"/*, m_GUIStyle*/);
        }
    }

    public void OnGoalReached()
    {
        Game.Instance.OnGoalReached();
    }

    // called after dying, but player still has lives
    public void ResetLevel(bool _instant, bool _moveSeeSaw)
    {
        m_bPlayerCameToRest = _moveSeeSaw;
        if (!_instant)
            m_fResetTimer = ResetTime;
        else
            m_fResetTimer = 0.0f;
    }
    
    public Vector3 GetLevelCenterPt()
    {
        // take the see saw's board's center & the goal trigger's center
        // and look at the center point of that:
        return (m_SeeSawObject.GetComponent<SeeSaw>().BoardObject.transform.position + m_GoalTriggerObject.transform.position) * 0.5f;
    }

    public float GetDistBoardToTrigger()
    {
        return (m_SeeSawObject.GetComponent<SeeSaw>().BoardObject.transform.position - m_GoalTriggerObject.transform.position).magnitude;
    }

    public Vector3 GetPlayerPlacement()
    {
        return m_SeeSawObject.GetComponent<SeeSaw>().GetPlayerPosition();
    }

    // private:
    private void _InitCommonObjects()
    {
        Debug.Log("Init Common Objs");

        m_GoalTriggerObject = GameObject.Find("GoalTrigger");

        m_SeeSawObject = GameObject.Find("SeeSaw");
        m_SeeSawObject.GetComponent<SeeSaw>().Init();

        m_PlayerObject = GameObject.Find("Player");
        m_PlayerObject.GetComponent<Player>().Init();

        m_PlayerCameraObject = GameObject.Find("PlayerCamera");
        m_PlayerCam = m_PlayerCameraObject.GetComponent<SmoothLookAtFollow>();
    }
    private void _Reset()
    {
        Debug.Log("Resetting Level");
        m_fResetTimer = -1.0f; // means no longer resetting
        m_Game.WeightDropped = m_Game.LaunchStarted = false;
        m_bPlayerCameToRest = false;

        if (m_SeeSawObject != null)
        {
            m_SeeSawObject.GetComponent<SeeSaw>().OnReset();
            m_PlayerObject.GetComponent<Player>().OnReset();
            m_PlayerCam.OnReset();
        }
    }
    private void _ResetToNewPosition()
    {
        Debug.Log("Resetting Level, new pos:" + m_PlayerObject.transform.position.ToString());
        m_fResetTimer = -1.0f; // means no longer resetting
        m_Game.WeightDropped = m_Game.LaunchStarted = false;
        m_bPlayerCameToRest = false;

        if (m_SeeSawObject != null)
        {
            m_SeeSawObject.GetComponent<SeeSaw>().OnResetToNewPosition(m_PlayerObject.transform.position);
            m_PlayerObject.GetComponent<Player>().OnReset();
            m_PlayerCam.SetNewZoomedInVars();
            m_PlayerCam.SetNewZoomedOutVars();
            m_PlayerCam.OnReset();
        }
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