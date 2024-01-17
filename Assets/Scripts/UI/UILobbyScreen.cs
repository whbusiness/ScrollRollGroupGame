using UnityEngine;
using TMPro;

public class UILobbyScreen : MonoBehaviour
{
    // Player UI Components
    [Header("Player UI Components")]
    public GameObject[] m_playerJoinPrompts = new GameObject[4];
    public TextMeshProUGUI[] m_playerNicknames = new TextMeshProUGUI[4];

    private void Start()
    {
        ShowAllPlayerJoinPrompts();

        // Set all player name TMPro components to empty by default
        ClearAllPlayerNicknames();
    }

    /// <summary>
    /// Sets all join prompts in m_playerJoinPrompts to active
    /// </summary>
    private void ShowAllPlayerJoinPrompts()
    {
        foreach (GameObject p_gameObject in m_playerJoinPrompts)
        {
            p_gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides a player join prompt
    /// </summary>
    /// <param name="p_playerIndex"></param>
    public void HidePlayerJoinPrompt(int p_playerIndex)
    {
        m_playerJoinPrompts[p_playerIndex].SetActive(false);
    }

    /// <summary>
    /// Sets all TMPro components in m_playerNames to empty strings
    /// </summary>
    private void ClearAllPlayerNicknames()
    {
        foreach (TextMeshProUGUI p_text in m_playerNicknames)
        {
            p_text.text = string.Empty;
        }
    }

    /// <summary>
    /// Applies the players nickname to the UI
    /// </summary>
    /// <param name="p_nickname"></param>
    /// <param name="p_playerIndex"></param>
    public void ApplyPlayerNicknameToUI(string p_nickname, int p_playerIndex)
    {
        m_playerNicknames[p_playerIndex].SetText(p_nickname);
    }
}
