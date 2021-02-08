using UnityEngine;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;

    public static List<Transform> allPlayers = new List<Transform>();


    private void Awake()
    {
        instance = this;

        Transform player = GameObject.FindGameObjectWithTag(TagsLayers.playerTag).transform;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(TagsLayers.enemyTag);

        allPlayers.Add(player);
        foreach(GameObject g in enemies)
        {
            allPlayers.Add(g.transform);
        }
    }

}
