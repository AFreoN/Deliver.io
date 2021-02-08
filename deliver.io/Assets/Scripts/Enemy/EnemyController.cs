using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    NavMeshAgent navAgent = null;
    [SerializeField] EnemyProperties prop = null;
    public ENEMYSTATE enemyState = ENEMYSTATE.Idle;

    #region Property Values
    float moveSpeed = 0, minMoveSpeed = 0, speedReductionPerPlate = 0;
    float checkRadius = 0;
    float plateStackDistance = 0;
    #endregion

    float currentSpeed = 0;

    PlateSpawner[] spawnPoints = null;
    List<Transform> allOtherPlayer = new List<Transform>();

    [HideInInspector] public List<Transform> plates = new List<Transform>();
    bool havePlate = false;
    Transform platesHolder = null;

    Transform currentTarget = null;

    void Start()
    {
        setPropValues();

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;

        enemyState = ENEMYSTATE.Idle;

        spawnPoints = SpawnsTables.spawnPoints;
        platesHolder = transform.GetChild(0).Find("Plates Holder");

        allOtherPlayer = PlayersManager.allPlayers;
        allOtherPlayer.Remove(transform);
    }

    void setPropValues()
    {
        moveSpeed = prop.movementSpeed;
        speedReductionPerPlate = prop.speedReductionPerPlate;
        minMoveSpeed = prop.minMoveSpeed;
        checkRadius = prop.checkRadius;
        plateStackDistance = prop.plateStackDistance;

        currentSpeed = moveSpeed;
    }

    void Update()
    {
        if (GameManager.gameState != GameState.InGame)          //If game is not on, there is no need to call any enemy ai functions
            return;

        havePlate = plates.Count > 0 ? true : false;

        if (havePlate)
            currentSpeed = moveSpeed - (speedReductionPerPlate * plates.Count);
        else
            currentSpeed = moveSpeed;

        navAgent.speed = currentSpeed;

        currentTarget = getTarget();

        UpdateEnemyState();
    }

    Transform getTarget()
    {
        Transform tar = null;

        if(havePlate)
        {
            tar = SpawnsTables.currentDeliverTable.transform;
            navAgent.SetDestination(currentTarget.position);
        }
        else
        {
            float dis = float.MaxValue;
            int index = 0;
            bool atOneHavePlate = false;
            for(int i = 0; i < spawnPoints.Length; i++)
            {
                PlateSpawner ps = spawnPoints[i];
                float itsDistance = Vector3.Distance(transform.position, ps.transform.position);
                if(ps.havePlate && itsDistance < dis)
                {
                    dis = itsDistance;
                    index = i;
                    atOneHavePlate = true;
                }
            }

            if(atOneHavePlate == true)
            {
                tar = spawnPoints[index].transform;
            }
        }

        return tar;
    }

    void UpdateEnemyState()
    {
        if (enemyState == ENEMYSTATE.Dodged)
            return;

        if (havePlate == false)         //If there is no plate
        {
            if (currentTarget != null)
            {
                enemyState = ENEMYSTATE.Run;
                navAgent.SetDestination(currentTarget.position);
            }
            else
            {
                enemyState = ENEMYSTATE.Idle;
            }
        }
        else
        {
            if (currentTarget != null)
            {
                enemyState = ENEMYSTATE.CarryRun;
                navAgent.SetDestination(currentTarget.position);
            }
            else
            {
                enemyState = ENEMYSTATE.CarryIdle;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(TagsLayers.spawnPointTag))
        {
            navAgent.ResetPath();

            if (enemyState != ENEMYSTATE.Dodged)
                carryPlate(collision.gameObject.GetComponent<PlateSpawner>());
        }

        if(collision.gameObject.CompareTag(TagsLayers.tableTag))
        {
            navAgent.ResetPath();

            if(enemyState != ENEMYSTATE.Dodged && plates.Count != 0 && collision.gameObject.GetComponent<Table>() == SpawnsTables.currentDeliverTable)
            {
                foreach (Transform t in plates)
                    Destroy(t.gameObject);

                plates.Clear();
                SpawnsTables.changeDeliverTable();
            }
        }
    }

    void carryPlate(PlateSpawner ps)
    {
        if (ps.havePlate == false)
            return;

        Transform t = ps.getPlate();
        plates.Add(t);
        t.SetParent(platesHolder);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localPosition = t.up * 0.1f * (plates.Count - 1);
    }

    void switchCon()
    {
        switch (enemyState)
        {
            case ENEMYSTATE.Idle:

                break;

            case ENEMYSTATE.Run:

                break;

            case ENEMYSTATE.CarryIdle:

                break;

            case ENEMYSTATE.CarryRun:

                break;

            case ENEMYSTATE.Dodged:

                break;
        }
    }

    //This functions is called when other player steal plates from this player
    public List<Transform> getPlates()
    {
        havePlate = false;
        navAgent.ResetPath();

        List<Transform> result = new List<Transform>();
        foreach (Transform t in plates)
            result.Add(t);

        plates.Clear();

        return result;
    }
}

public enum ENEMYSTATE
{
    Idle,
    Run,
    CarryIdle,
    CarryRun,
    Dodged
}
