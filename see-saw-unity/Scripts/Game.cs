using UnityEngine;
using System.Collections;

/// <summary>
/// Main game logic class, a singleton
/// Acts as central access point for all classes
/// </summary>
public class Game
{
    private static Game instance;

    bool m_bPreviewDone = false;
    
    int m_nCurrLevel;
    int m_nCurrWorld;

    int m_nNumActualLevelsInWorld;
    int m_nNumWorlds;
    int m_nNumMenus;

    PlayerInfo m_PlayerInfo;
    Level m_CurrLevel;

    public Game()
    {
        Debug.Log("Game instance created (CTOR)");
        if (instance != null)
            return;
        instance = this;

        // TODO:: figure out a better dynamic way to do this:
        m_nNumMenus = 3;
        m_nNumActualLevelsInWorld = Application.levelCount - m_nNumMenus;
        m_nNumWorlds = 1;
    }

    public void Update()
    {
        // TODO:: display current world/level on screen:
    }

    public void LateUpdate()
    {
    }

    public void NextLevel()
    {

        // TODO:: increment g_nCurrLevel/g_nCurrWorld first:
        if (++m_nCurrLevel <= m_nNumActualLevelsInWorld)
        {
            Debug.Log("Loading next level:" + m_nCurrLevel.ToString());
            Application.LoadLevel(m_nCurrLevel);
        }
        else
        {
            // TODO:: goto next world / end
            OnGotoMainMenu();
        }
    }
    public void RedoCurrLevel()
    {
        Debug.Log("RedoCurrLevel:" + m_nCurrLevel.ToString());
        m_PlayerInfo.RevertToDefaults();
        Application.LoadLevel(m_nCurrLevel);
    }

    public void StartGame(int _level, int _world)
    {
        Debug.Log("StartGame called, Level:" + _level.ToString());
        m_nCurrLevel = _level;
        m_nCurrWorld = _world;
        Application.LoadLevel(m_nCurrLevel);
    }

    // called from level's start
    // returns m_PlayerInfo to the calling level
    public PlayerInfo NewLevel(Level lvl)
    {
        CurrLevel = lvl;
        CurrLevel.WeightDropped = CurrLevel.LaunchStarted = PreviewDone = false;

        return m_PlayerInfo;
    }

    public void OnGoalReached()
    {
        if (m_nCurrLevel == m_nNumActualLevelsInWorld && m_nCurrWorld == m_nNumWorlds)
        {
            // TODO:: celebrate this world complete or something, go to next world
            Application.LoadLevel("MainMenu");
        } 
        else
        {
            Application.LoadLevel("VictoryMenu");
        }
    }
    public void OnCharacterDied()
    {
        if (m_PlayerInfo.OnDeath())
        {
            // more lives left, reset level
            m_CurrLevel.ResetLevel(false, false);
        }
        else
        {
            Application.LoadLevel("DefeatMenu");
        }
    }
    public void OnGotoMainMenu()
    {
        m_PlayerInfo.RevertToDefaults();
        Application.LoadLevel("MainMenu");
    }

    // returns where the player needs to be in relation to the see-saw
    public Vector3 GetPlayerPlacing()
    {
        return CurrLevel.GetPlayerPlacement();
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    public Level CurrLevel
    {
        get { return m_CurrLevel; }
        set { m_CurrLevel = value; }
    }
    public int NumActualLevelsInWorld
    {
        get { return m_nNumActualLevelsInWorld; }
        set { m_nNumActualLevelsInWorld = value; }
    }
    public int CurrWorldNum
    {
        get { return m_nCurrWorld; }
        set { m_nCurrWorld = value; }
    }
    public int CurrLevelNum
    {
        get { return m_nCurrLevel; }
        set { m_nCurrLevel = value; }
    }
    public bool WeightDropped
    {
        get { return CurrLevel.WeightDropped; }
        set { CurrLevel.WeightDropped = value; }
    }
    public bool LaunchStarted
    {
        get { return CurrLevel.LaunchStarted; }
        set { CurrLevel.LaunchStarted = value; }
    }
    public bool PreviewDone
    {
        get { return m_bPreviewDone; }
        set { m_bPreviewDone = value; }
    }
    public PlayerInfo PlayerInfo
    {
        get { return m_PlayerInfo; }
        set { m_PlayerInfo = value; }
    }

    public static Game Instance
    {
        get 
        {
            if (instance == null)
                new Game();
            return instance; 
        }
    }
}