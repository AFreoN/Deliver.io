using UnityEngine;

public class SpawnsTables : MonoBehaviour
{
    public static SpawnsTables instance { get; private set; }

    public static PlateSpawner[] spawnPoints { get; private set; }
    public static Table[] tables { get; private set; }

    public static Table currentDeliverTable;
    static int totalTableCount, currentTableIndex;

    public Table testTable;

    void Awake()
    {
        instance = this;

        Component[] comp = GameObject.FindGameObjectsWithTag(TagsLayers.spawnPointTag).ToCustomArray(typeof(PlateSpawner));
        spawnPoints = System.Array.ConvertAll(comp, item => item as PlateSpawner);

        Component[] tableComp = GameObject.FindGameObjectsWithTag(TagsLayers.tableTag).ToCustomArray(typeof(Table));
        tables = System.Array.ConvertAll(tableComp, item => item as Table);
        foreach (Table t in tables)
            t.notThisTable();

        totalTableCount = tables.Length;
        currentTableIndex = 0;

        currentDeliverTable = tables[currentTableIndex].getTable();

        testTable = currentDeliverTable;
    }

    private void Update()
    {
        testTable = currentDeliverTable;
    }

    public static void changeDeliverTable()
    {
        resetAllTables();

        currentTableIndex++;
        currentTableIndex = currentTableIndex % totalTableCount;

        currentDeliverTable = tables[currentTableIndex].getTable();
        
    }

    static void resetAllTables()
    {
        foreach (Table t in tables)
            t.notThisTable();
    }
}
