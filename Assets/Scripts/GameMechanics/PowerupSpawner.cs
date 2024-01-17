using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public List<GameObject> m_powerupsToSpawn;
    public List<GameObject> m_powerupsSpawned;
    public float m_minimumSpawnX;
    public float m_maximumSpawnX;
    public float m_spawnIntervalSeconds;

    private float m_lastSpawnTime = 0;

    private void FixedUpdate()
    {
        if(Time.time > m_lastSpawnTime + m_spawnIntervalSeconds)
        {
            m_lastSpawnTime = Time.time;

            m_powerupsSpawned.Add(Instantiate(m_powerupsToSpawn[Random.Range(0, m_powerupsToSpawn.Count)]));
            m_powerupsSpawned[^1].transform.position = new Vector3(Random.Range(m_minimumSpawnX, m_maximumSpawnX), transform.position.y, transform.position.z);
        }
    }
}