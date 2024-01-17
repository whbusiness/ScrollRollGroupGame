using UnityEngine;
using UnityEngine.SceneManagement;

public class UIChangeScene : MonoBehaviour
{
    public int m_nextSceneID;

    public void ChangeScene()
    {
        SceneManager.LoadScene(m_nextSceneID);
    }

    public void SetNextScene(int p_next)
    {
        m_nextSceneID = p_next;
    }
}