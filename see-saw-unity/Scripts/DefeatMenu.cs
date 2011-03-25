using UnityEngine;
using System.Collections;

public class DefeatMenu : MonoBehaviour
{
    GUIText m_Level;
    GUIText m_MainMenu;

    void Start()
    {
        m_Level = GameObject.Find("ResetLevel").guiText;
        m_MainMenu = GameObject.Find("MainMenu").guiText;
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            // TODO:: color text on hover:
            if (m_Level.HitTest(Input.mousePosition))
            {
                Game.Instance.RedoCurrLevel();
            }
            else if (m_MainMenu.HitTest(Input.mousePosition))
            {
                // TODO:: perform any resetting necessary here:
                Game.Instance.OnGotoMainMenu();
            }
        }
    }
}