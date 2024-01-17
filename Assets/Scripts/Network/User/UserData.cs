using System;
using UnityEngine;

[Serializable]
public class UserData
{
    public string m_userName;
    public string m_password;
    public string m_userID = Guid.NewGuid().ToString();
    public UserGameData[] m_gameData = { new UserGameData(), new UserGameData(),
                                         new UserGameData(), new UserGameData()};
    public string GetJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static UserData FromJSON(string p_json)
    {
        return JsonUtility.FromJson<UserData>(p_json);
    }

    public UserData(UserData p_other)
    {
        m_userName = p_other.m_userName;
        m_password = p_other.m_password;
        m_userID = p_other.m_userID;
        m_gameData = p_other.m_gameData;
    }

    public UserData()
    {

    }
}