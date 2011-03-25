using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
    int m_nLives;
    int m_nHealth;

    // defaults occur when all lives are exhausted
    // and no more than defaults have been obtained
    public const int g_nDefaultHealth = 3;
    public const int g_nDefaultLives = 3;

    int m_nMaxHealth;
    int m_nMaxLives;

    void Start()
    {
        DontDestroyOnLoad(this);

        m_nMaxHealth = 3;
        m_nMaxLives = 99;
    }

    void Update()
    {

    }

    public void Init(int _lives, int _health)
    {
        m_nLives = _lives;
        m_nHealth = _health;
    }

    // return false if game over
    public bool OnDeath()
    {
        --m_nLives;
        return m_nLives > 0;
    }

    public void RevertToDefaults()
    {
        m_nHealth = g_nDefaultHealth;
        m_nLives = g_nDefaultLives;
    }


    public int Lives
    {
        get { return m_nLives; }
        set { m_nLives = value; }
    }
    public int Health
    {
        get { return m_nHealth; }
        set { m_nHealth = value; }
    }
}