using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider _info)
    {
        if (_info.gameObject.name == "Player")
        {
            Game.Instance.OnCharacterDied();
        }
    }
}