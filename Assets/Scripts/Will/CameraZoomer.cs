using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomer : MonoBehaviour
{
    public List<PlayerController> player = new List<PlayerController>();
    private float zoomInFOV = 30;
    public float smoothTime;
    private float velocity = 0;
    public List<float> locationsOfPlayers = new List<float>();
    public List<int> Distances;
    public float biggestDist, minDist, zoomTime, maxZoomDist;
    public Camera cm;
    public static bool ZoomIn = false;
    [SerializeField]
    private NewGameController m_gameController;
    public List<GameObject> playersAndAI = new();
    // Start is called before the first frame update
    void Start()
    {
        FetchCameraPositioners();
    }

    public void FetchCameraPositioners()
    {
        m_gameController = FindFirstObjectByType<NewGameController>();
        if(!m_gameController.aiIsPlaying)
        {
            foreach (var l_player in FindObjectsOfType<PlayerController>())
            {
                print(l_player.name);
                locationsOfPlayers.Add(l_player.transform.position.x);
                // Distances.Add(Mathf.RoundToInt(Vector2.Distance(go.transform.position, player[player.Length + 1].transform.position)));
                l_player.gameObject.GetComponent<CameraPositioning>().m_cam = cm.transform;
                player.Add(l_player);
            }
        }
        if(m_gameController.aiIsPlaying)
        {
            playersAndAI = new List<GameObject>(GameObject.FindGameObjectsWithTag("AI"));
            foreach (var l_player in FindObjectsOfType<PlayerController>())
            {
                print(l_player.name);
                // Distances.Add(Mathf.RoundToInt(Vector2.Distance(go.transform.position, player[player.Length + 1].transform.position)));
                l_player.gameObject.GetComponent<CameraPositioning>().m_cam = cm.transform;
                player.Add(l_player);
                playersAndAI.Add(l_player.gameObject);
            }
            foreach (var playerOrAi in playersAndAI)
            {
                print("Adding Location");
                locationsOfPlayers.Add(playerOrAi.transform.position.x);
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player.Count <= 0) return;
        
        if (!m_gameController.aiIsPlaying)
        {
            for (int i = 0; i < locationsOfPlayers.Count; i++)
            {
                if (locationsOfPlayers[i] != player[i].transform.position.x)
                {
                    locationsOfPlayers[i] = player[i].transform.position.x;
                }
            }
        }
        else
        {
            for (int i = 0; i < locationsOfPlayers.Count; i++)
            {
                if (locationsOfPlayers[i] != playersAndAI[i].transform.position.x)
                {
                    locationsOfPlayers[i] = playersAndAI[i].transform.position.x;
                }
            }
        }
        biggestDist = getBiggestDist(locationsOfPlayers);
        if (Mathf.RoundToInt(biggestDist) <= minDist)
        {
            //Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, defaultFOV, ref velocity, smoothTime);
            cm.fieldOfView = Mathf.MoveTowards(cm.fieldOfView, zoomInFOV, smoothTime * Time.deltaTime);
        }
        else
        {
            //Camera.main.orthographicSize = Mathf.Clamp(Mathf.SmoothDamp(Camera.main.orthographicSize, dist, ref velocity, smoothTime), 5, 15);
            cm.fieldOfView = Mathf.Clamp(Mathf.MoveTowards(cm.fieldOfView, biggestDist * zoomTime, smoothTime * Time.deltaTime), zoomInFOV, maxZoomDist);
        }
        if(cm.fieldOfView < maxZoomDist) { ZoomIn = true; }
        else { ZoomIn=false; }

    }
    float getBiggestDist(List<float> list)
    {
        list.Sort();
        float highestValue = list[list.Count - 1];
        float lowestValue = list[0];
        return highestValue - lowestValue;
    }
}

