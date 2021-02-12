using UnityEngine;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;

    public static List<Transform> allPlayers = new List<Transform>();

    public List<string> Names = new List<string>();


    private void Awake()
    {
        instance = this;

        Transform player = GameObject.FindGameObjectWithTag(TagsLayers.playerTag).transform;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(TagsLayers.enemyTag);
        assignNames(enemies);

        allPlayers.Add(player);
        foreach(GameObject g in enemies)
        {
            allPlayers.Add(g.transform);
        }
    }


    void assignNames(GameObject[] enemies)
    {
        if (Names.Count < enemies.Length)
        {
            Debug.LogWarning("Not Enough Names");
            return;
        }

        for(int i = 0; i < enemies.Length; i++)
        {
            int r = Random.Range(0, Names.Count);
            enemies[i].name = Names[r];
            Names.RemoveAt(r);
        }
    }
}
