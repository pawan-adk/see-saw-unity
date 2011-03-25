using UnityEngine;
using System.Collections;

public class VictoryMenu : MonoBehaviour
{
    GUIText m_NextLvl;

    void Start()
    {
        m_NextLvl = GameObject.Find("NextLevel").guiText;
    }

    void Update()
    {
        if (m_NextLvl.HitTest(Input.mousePosition))
        {
            // TODO:: color text on hover:

            if (Input.GetMouseButtonUp(0))
                Game.Instance.NextLevel();
        }
    }
}