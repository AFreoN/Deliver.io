using UnityEngine;

public class PlateSpawner : MonoBehaviour
{
    Transform spawnPoint;

    [SerializeField] Transform platePrefab = null;
    [SerializeField] float spawnDuration = 1f;

    public Transform currentPlate { get; private set; }
    public bool havePlate { get; private set; }
    float temp = 0;

    void Start()
    {
        spawnPoint = transform.Find("SpawnPoint");

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

            if(temp >= spawnDuration)
            {
                currentPlate = Instantiate(platePrefab, spawnPoint.position, platePrefab.rotation);
                havePlate = true;
                temp = 0;
            }
        }
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
