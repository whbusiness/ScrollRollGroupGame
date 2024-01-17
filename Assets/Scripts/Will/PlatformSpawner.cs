using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] platformTypes;
    [SerializeField]
    private Vector3 firePLocation, icePLocation, grassPLocation, poisonPLocation;
    [SerializeField]
    private ScrollController scrollController;
    [SerializeField]
    private List<GameObject> platformsInScene = new();

    private void Start()
    {
        scrollController = FindObjectOfType<ScrollController>();
    }

    public void SpawnPlatforms()
    {
        var currentBiome = scrollController.m_currentElement;
        switch (currentBiome)
        {
            case ElementType.Grass:
                print("Platform Grass");
                RemoveAllPlatforms();
                GameObject platform1 = Instantiate(platformTypes[0], grassPLocation, Quaternion.identity);
                platformsInScene.Add(platform1);
                break;
            case ElementType.Poison:
                print("Platform Poison");
                RemoveAllPlatforms();
                GameObject platform2 = Instantiate(platformTypes[1], poisonPLocation, Quaternion.Euler(0,180,0));
                platformsInScene.Add(platform2);
                break;
            case ElementType.Ice:
                print("Platform Ice");
                RemoveAllPlatforms();
                GameObject platform3 = Instantiate(platformTypes[2], icePLocation, Quaternion.identity);
                platformsInScene.Add(platform3);
                break;
            case ElementType.Fire:
                print("Platform Fire");
                RemoveAllPlatforms();
                GameObject platform4 = Instantiate(platformTypes[3], firePLocation, Quaternion.identity);
                platformsInScene.Add(platform4);
                break;
        }
    }

    void RemoveAllPlatforms()
    {
        foreach (var platform in platformsInScene)
        {
            Destroy(platform);
        }
        platformsInScene = new();
    }
}
