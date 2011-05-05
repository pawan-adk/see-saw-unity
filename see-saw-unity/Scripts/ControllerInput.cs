#if UNITY_IPHONE
using UnityEngine;
using System.Collections;

public class ControllerInput : MonoBehaviour
{		
	public const int BTN_LEFT = 0;
	public const int BTN_RIGHT= 1;
	public const int BTN_UP 	= 2;
	public const int BTN_DOWN	= 3;
	public const int BTN_A	= 4;
	public const int BTN_B	= 5;
	
	public const int NUM_BTNS  = 6;
	
	bool[] m_arrPrevTouches = new bool[NUM_BTNS];
	
	int m_nDownFlags;
	int m_nEnterFlags;
	int m_nReleasedFlags;
		
	GUITexture[] m_arrBtns = new GUITexture[NUM_BTNS];
	Color m_clrArrowOrig;
	public Color m_clrHitClr;
	
	
	void Start ()
	{		
		m_arrBtns[BTN_LEFT] = transform.Find("LeftArrow").guiTexture;
		m_arrBtns[BTN_RIGHT]= transform.Find("RightArrow").guiTexture;
		m_arrBtns[BTN_UP] 	= transform.Find("UpArrow").guiTexture;
		m_arrBtns[BTN_DOWN] = transform.Find("DownArrow").guiTexture;
		m_arrBtns[BTN_A]	= transform.Find("A_Btn").guiTexture;
		m_arrBtns[BTN_B]	= transform.Find("B_Btn").guiTexture;
				
		m_clrArrowOrig = m_arrBtns[BTN_LEFT].color;
		m_nDownFlags = m_nEnterFlags = m_nReleasedFlags = 0;
	}

	
	void Update ()
	{			
		// turn off any previously ended flags
		m_nReleasedFlags = 0;
		
		m_arrPrevTouches[0] = m_arrPrevTouches[1] = m_arrPrevTouches[2] = m_arrPrevTouches[3] = m_arrPrevTouches[4] = m_arrPrevTouches[5] = false;
		
		// go through all the buttons
		// if any touches hit a button on began, stationary, or moved, color it & set its flag
		// otherwise, if the previous touch did not hit the current button, the current button gets reset
		int i = 0;
		foreach (GUITexture btn in m_arrBtns)
		{
			foreach (Touch touch in Input.touches)
			{
				Vector3 pos = touch.position;
				if (btn.HitTest(pos))
				{
					m_arrPrevTouches[i] = true;
					btn.color = m_clrHitClr;
					
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{	
						//Debug.Log("Touch Down");
						
						Utilities.Instance.BitOn(ref m_nDownFlags, i);
						Utilities.Instance.BitOff(ref m_nReleasedFlags, i);
						Utilities.Instance.BitOff(ref m_nEnterFlags, i);
					}
					else if (touch.phase == TouchPhase.Began)
					{
						//Debug.Log("Touch Began");
						
						Utilities.Instance.BitOn(ref m_nEnterFlags, i);
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						//Debug.Log("Touch Ended");
						
						btn.color = m_clrArrowOrig;
						Utilities.Instance.BitOn(ref m_nReleasedFlags, i);
						Utilities.Instance.BitOff(ref m_nDownFlags, i);
						Utilities.Instance.BitOff(ref m_nEnterFlags, i);
					}
				}
				else if (!m_arrPrevTouches[i])
				{
					btn.color = m_clrArrowOrig;
					Utilities.Instance.BitOff(ref m_nDownFlags, i);
					Utilities.Instance.BitOff(ref m_nReleasedFlags, i);
					Utilities.Instance.BitOff(ref m_nEnterFlags, i);
				}
			}			
			++i;
		}
	}
	
	public bool BtnDown(int _btn)
	{
		return Utilities.Instance.BitTest(m_nDownFlags, _btn);	
	}
	public bool BtnPressed(int _btn)
	{
		return Utilities.Instance.BitTest(m_nEnterFlags, _btn);	
	}
	public bool BtnReleased(int _btn)
	{
		return Utilities.Instance.BitTest(m_nReleasedFlags, _btn);	
	}	
}

#else
using UnityEngine;
using System.Collections;

public class ControllerInput : MonoBehaviour
{	
	void Start ()
	{		
		DestroyImmediate(transform.Find("LeftArrow"));
		DestroyImmediate(transform.Find("RightArrow"));
		DestroyImmediate(transform.Find("UpArrow"));
		DestroyImmediate(transform.Find("DownArrow"));
		DestroyImmediate(transform.Find("A_Btn"));
		DestroyImmediate(transform.Find("B_Btn"));
	}
}

#endif

