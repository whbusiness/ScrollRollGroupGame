using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UISettingsMenuFunctions : MonoBehaviour
{
    // Video Settings Variables

    // Audio Settings Variables
    [Header("Audio Settings Variables")]
    public AudioMixer m_audioMixer;

    // Sets whether the game is in fullscreen, used with a toggle
    public void FullScreenToggle(bool p_isFullScreen)
    {
        Screen.fullScreen = p_isFullScreen;
    }

    // Function for setting the volume of the MasterVolume audio mixer parameter, used with a slider
    public void SetMasterVolume(float p_volume)
    {
        m_audioMixer.SetFloat("MasterVolume", p_volume);
    }

    // Function for setting the volume of the SFXVolume audio mixer parameter, used with a slider
    public void SetSFXVolume(float p_volume)
    {
        m_audioMixer.SetFloat("SFXVolume", p_volume);
    }

    // Function for setting the volume of the MusicVolume audio mixer parameter, used with a slider
    public void SetMusicVolume(float p_volume)
    {
        m_audioMixer.SetFloat("MusicVolume", p_volume);
    }
}
