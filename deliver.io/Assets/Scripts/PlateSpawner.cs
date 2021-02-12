using UnityEngine;

public class PlateSpawner : MonoBehaviour
{
    Transform spawnPoint;

    [SerializeField] Transform platePrefab = null;
    [SerializeField] float spawnDuration = 1f;
    float tolerance = 1.5f;
    float currentSpawnDuration = 0;

    public Transform currentPlate { get; private set; }
    public bool havePlate { get; private set; }
    float temp = 0;

    void Start()
    {
        spawnPoint = transform.Find("SpawnPoint");

        currentSpawnDuration = getCurrentSpawnDuration(tolerance);
        temp = 0;
        havePlate = false;
    }

    private void Update()
    {
        if (GameManager.gameState != GameState.InGame)
            return;

        if (currentPlate == null)
            havePlate = false;

        if(havePlate == false)
        {
            temp += Time.deltaTime;

            if(temp >= currentSpawnDuration)
            {
                currentPlate = Instantiate(platePrefab, spawnPoint.position, platePrefab.rotation);
                havePlate = true;
                temp = 0;
                currentSpawnDuration = getCurrentSpawnDuration(tolerance);
            }
        }
    }

    float getCurrentSpawnDuration(float variance)
    {
        float min = spawnDuration;
        float max = spawnDuration + variance;

        return Random.Range(min, max);
    }

    public Transform getPlate()
    {
        if (!havePlate)
            return null;

        havePlate = false;
        Transform p = currentPlate;
        currentPlate = null;

        return p;
    }
}
