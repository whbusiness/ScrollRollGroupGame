using UnityEngine;

public class PlayerReadyTrigger : MonoBehaviour
{
    public NewGameController m_controller;
    public int m_playerIndex;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            m_controller.SetPlayerReady(m_playerIndex);
        }
    }
}