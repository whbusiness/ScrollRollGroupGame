using UnityEngine;

public class Powerup : MonoBehaviour
{
    public PowerupType m_powerupType;
    public float m_rotationSpeed = 10.0f;
    public float m_timeAlive = 5.0f;
    private float m_timeSpawned;
    private Rigidbody m_rb;
    public void Start()
    {
        if(m_rb == null) m_rb = GetComponent<Rigidbody>();
        m_timeSpawned = Time.time;
    }

    public void FixedUpdate()
    {
        transform.Rotate(m_rotationSpeed * Time.deltaTime * Vector3.forward);

        if(Time.time > m_timeSpawned + m_timeAlive)
        {
            transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);

            if(transform.localScale.y <= 0.05f)
            {
                Destroy(gameObject);
            }
        }
    }   

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerController>(out var l_player))
        {
            l_player.m_hudController.SetPowerup(m_powerupType);
            l_player.PlayPickupAudio();
            Destroy(gameObject);
        }else if (other.tag.Equals("Scroll"))
        {
            m_rb.isKinematic = true;
        }
    }
}