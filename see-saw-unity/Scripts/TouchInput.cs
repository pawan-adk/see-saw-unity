using UnityEngine;using System.Collections;#if UNITY_IPHONEpublic class TouchInput : MonoBehaviour{	    // the max amount of time that can pass for a second tap to register as a double tap    public float DoubleTapDelayMax;

    int m_nNumTaps;    void Start()    {    }    void Update()    {
        // check for other touch input besides buttons
        if (Input.touchCount > 0 && !Game.Instance.ControllerInput.ButtonTouched)        {        }    }}#endif