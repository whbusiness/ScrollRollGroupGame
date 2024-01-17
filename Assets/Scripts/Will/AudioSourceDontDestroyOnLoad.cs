using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceDontDestroyOnLoad : MonoBehaviour
{
    public static AudioSourceDontDestroyOnLoad instance = null;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
