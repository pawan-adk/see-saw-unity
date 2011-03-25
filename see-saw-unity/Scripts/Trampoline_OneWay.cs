using UnityEngine;
using System.Collections;

public class Trampoline_OneWay : MonoBehaviour
{
    Animation m_BounceAnim;
    Player m_Player;
    Vector3 m_vForce;

    // the maximum amount of time that the tramp
    // applies force to the player
    public float RestingThresholdForce = 400.0f;
    public float Friction = 0.85f;
    public float TensionConstant = 90.0f;
    public float MaximumForce = 1350.0f;
    public float LowAnimMultiplier = 0.75f, HighAnimMultiplier = 1.75f;

    bool m_bPlayerInTrigger = false;

    ////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_BounceAnim = transform.parent.gameObject.animation;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider info)
    {
//         Debug.Log("Collided With Trampoline trigger");

        Vector3 vNorm = transform.up.normalized;
        Vector3 vVel = m_Player.rigidbody.velocity;
        float velX = Mathf.Abs(vVel.x) * Friction * vNorm.x * TensionConstant;
        float velY = Mathf.Abs(vVel.y) * Friction * vNorm.y * TensionConstant;
        if (Mathf.Abs(velX) < RestingThresholdForce * vNorm.x)
            velX = 0.0f;
        if (Mathf.Abs(velY) < RestingThresholdForce * vNorm.y)
            velY = 0.0f;

        // only play anim if bounce was high enough
        if (Mathf.Abs(velX) > 0.0f || Mathf.Abs(velY) > 0.0f)
        {
	        // TODO:: maybe play at a speed corresponding to the player's velocity??
	        m_BounceAnim["Take 001"].speed = Mathf.Lerp(LowAnimMultiplier, HighAnimMultiplier, (velX + velY) / MaximumForce);
//             Debug.Log("Anim Speed:" + m_BounceAnim["Take 001"].speed.ToString());
	        m_BounceAnim.Play();
        }

        // max force will mostly come into play when jumping is combined with bouncing
//         if (velX > MaximumForce)
//             velX = MaximumForce;
//         if (velY > MaximumForce)
//             velY = MaximumForce;

        // TODO:: make sure the player is coming from "in front" of the bounce pad
        // dot determines this

//         Debug.Log("Trans Up (normal): " + vNorm.ToString());
//         Debug.Log("Player Vel: " + m_Player.rigidbody.velocity.ToString());

        Force = new Vector3(velX, velY, 0.0f);

//         Debug.Log("Force To Be Applied: " + Force.ToString());
        m_bPlayerInTrigger = true;
        m_Player.OnTrampEnter(this);
    }

    void OnTriggerStay(Collider info)
    {
        if (info.gameObject.name == "Player")
        {
            m_bPlayerInTrigger = true;
        }
    }
    void OnTriggerExit(Collider info)
    {
        if (info.gameObject.name == "Player")
        {
            m_bPlayerInTrigger = false;
            Debug.Log("No Longer colliding with tramp");
        }
    }

    public bool ContinueToApplyForce()
    {
        return m_bPlayerInTrigger;
    }

    public bool PlayerInTrigger
    {
        get { return m_bPlayerInTrigger; }
    }
    public Vector3 Force
    {
        get { return m_vForce; }
        set { m_vForce = value; }
    }
}