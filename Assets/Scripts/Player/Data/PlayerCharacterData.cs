using System;
using System.IO;
using UnityEngine;

/// <summary>
/// A basic store of customisable data for player characters.
/// </summary>
[Serializable]
public struct PlayerCharacterData
{
    public const string PLAYER_CHAR_DATA_FILE_NAME = "skin.json";
    public string m_playerID;
    public Color32 m_hatColour;
    public Color32 m_bodyColour;
    public Color32 m_wandColour;

    public void SaveCharacterData()
    {
        if (!Directory.Exists(Application.persistentDataPath))
        {
            Directory.CreateDirectory(Application.persistentDataPath);
        }

        var l_fileName = Application.persistentDataPath + Path.DirectorySeparatorChar + PLAYER_CHAR_DATA_FILE_NAME;

        if (File.Exists(l_fileName))
        {
            File.Delete(l_fileName);
            File.WriteAllText(l_fileName, JsonUtility.ToJson(this));
        } else
        {
            var l_createdFile = File.CreateText(l_fileName);
            l_createdFile.Write(JsonUtility.ToJson(this));
            l_createdFile.Flush();
            l_createdFile.Close();
        }
    }

    public void SetPlayerID(string p_playerID)
    {
        m_playerID = new string(p_playerID);
    }

    public static PlayerCharacterData LoadCharacterData()
    {
        return JsonUtility.FromJson<PlayerCharacterData>(File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + PLAYER_CHAR_DATA_FILE_NAME));
    }

    public static PlayerCharacterData RandomCharacterData()
    {
        return new PlayerCharacterData() {
            m_hatColour = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255),
            m_bodyColour = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255),
            m_wandColour = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 255)
        };
    }
}

public enum PlayerCharCustomisable
{
    Hat,
    Body, 
    Wand
}