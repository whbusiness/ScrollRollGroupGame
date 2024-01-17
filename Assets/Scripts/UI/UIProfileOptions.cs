using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIProfileOptions : MonoBehaviour
{
    public static string PlayerNickname = "Nickname";

    /// <summary>
    /// Function for setting the Player's nickname, used with an input field component
    /// </summary>
    /// <param name="p_nickname"></param>
    public void SetPlayerNickname(string p_nickname)
    {
        PlayerNickname = p_nickname == string.Empty ? "Nickname" : p_nickname;

        Debug.Log("SetPlayerNickname Called - PlayerNickname: " + PlayerNickname);
    }
}
