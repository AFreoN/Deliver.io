using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    Rigidbody rb = null;
    NavMeshAgent navAgent = null;
    [HideInInspector] public PowerHandler powerHandler = null;

    [SerializeField] EnemyProperties prop = null;
    public ENEMYSTATE enemyState = ENEMYSTATE.Idle;
    TargetType targetType = TargetType.None;

    #region Property Values
    float moveSpeed = 0, minMoveSpeed = 0, speedReductionPerPlate = 0;
    float checkRadius = 0;
    float plateStackDistance = 0;
    #endregion

    float currentSpeed = 0;

    PlateSpawner[] spawnPoints = null;
    Transform[] allOtherPlayer;

    [HideInInspector] public List<Transform> plates = new List<Transform>();
    [HideInInspector] public bool havePlate = false;
    Transform platesHolder = null;

    [SerializeField] Transform currentTarget = null;
    PlateSpawner currentPlate = null;
    PlayerController currentPC = null;
    EnemyController currentEC = null;
    Coroutine lastDodgeRoutine = null;

    float coolDown = 0;

    void Start()
    {
        setPropValues();

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;
        powerHandler = new PowerHandler(transform, 1);

        enemyState = ENEMYSTATE.Idle;

        spawnPoints = SpawnsTables.spawnPoints;
        platesHolder = transform.GetChild(0).Find("Plates Holder");

        List<Transform> allPlayers = new List<Transform>();
        allPlayers = PlayersManager.allPlayers;
        allPlayers.Remove(transform);
        allOtherPlayer = allPlayers.ToArray();
    }

    void setPropValues()
    {
        moveSpeed = prop.movementSpeed;
        speedReductionPerPlate = ControlValues.speedReductionPerPlate;
        minMoveSpeed = prop.minMoveSpeed;
        checkRadius = prop.checkRadius;
        plateStackDistance = prop.plateStackDistance;

        currentSpeed = moveSpeed;
        coolDown = prop.checkDelay;
    }

    void Update()
    {
        if (GameManager.gameState != GameState.InGame || enemyState == ENEMYSTATE.Dodged)          //If game is not on, there is no need to call any enemy ai functions
        {
            if (navAgent.hasPath)
                navAgent.ResetPath();
            return;
        }

        havePlate = plates.Count > 0 ? true : false;

        #region setting up the velocity
        if (havePlate)
        {
            float normalVelocity = moveSpeed + powerHandler.speed;
            float reduction = 1 - plates.Count * speedReductionPerPlate;
            currentSpeed = Mathf.Clamp(normalVelocity * reduction, minMoveSpeed, Mathf.Infinity);
        }
        else
        {
            currentSpeed = moveSpeed + powerHandler.speed;
        }
        navAgent.speed = currentSpeed;
        #endregion

        currentTarget = getTarget();

        UpdateEnemyState();
    }

    Transform getTarget()
    {
        Transform tar = currentTarget;   //main function return
        Transform t = null; //used for condition return

        if (havePlate)
        {
            tar = SpawnsTables.currentDeliverTable.transform;
            targetType = TargetType.Tables;
        }
        else
        {
            switch(targetType)
            {
                case TargetType.Spawns:
                    if(currentPlate != null && currentPlate.havePlate == false)
                    {
                        t = getRandomSpawnTarget(spawnPoints);
                        if (t != null) return t;    //return one
                    }

                    coolDown -= Time.deltaTime;
                    if(coolDown <= 0)
                    {
                        coolDown = prop.checkDelay;
                        t = getClosestPlayer(transform, allOtherPlayer, checkRadius);
                        tar = t != null ? t : currentTarget;
                        return tar;     //return two
                    }
                    break;

                case TargetType.Players:
                    coolDown = prop.checkDelay;

                    if(currentPC != null && currentPC.havePlates == false)
                    {
                        t = getRandomSpawnTarget(spawnPoints);
                        if (t != null) return t;        //return three
                    }
                    else if(currentEC != null && currentEC.havePlate == false)
                    {
                        t = getRandomSpawnTarget(spawnPoints);  
                        if (t != null) return t;    //return five
                    }
                    break;

                case TargetType.None:

                    t = getRandomSpawnTarget(spawnPoints);
                    if (t != null) return t;    //return four

                    break;

                case TargetType.Tables:
                    if(havePlate == false)
                    {
                        t = getRandomSpawnTarget(spawnPoints);
                        if (t != null) return t;
                    }
                    break;

                default:
                    coolDown = prop.checkDelay;
                    return currentTarget;
            }
        }

        return tar;     //last return
    }

    Transform switchTarget()
    {
        Transform tar = null;

        if(havePlate)
        {
            tar = SpawnsTables.currentDeliverTable.transform;
        }
        else
        {
            tar = getRandomSpawnTarget(spawnPoints);
            
        }

        return tar;
    }

    Transform nearSpawnTarget(PlateSpawner[] allTargets)
    {
        Transform tar = null;

        float dis = float.MaxValue;
        int index = 0;
        bool atOneHavePlate = false;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PlateSpawner ps = spawnPoints[i];
            float itsDistance = Vector3.Distance(transform.position, ps.transform.position);
            if (ps.havePlate && itsDistance < dis)
            {
                dis = itsDistance;
                index = i;
                atOneHavePlate = true;
            }
        }

        if (atOneHavePlate == true)
        {
            tar = spawnPoints[index].transform;
            currentPlate = spawnPoints[index];
            targetType = TargetType.Spawns;
        }
        return tar;
    }

    Transform getRandomSpawnTarget(PlateSpawner[] allTargets)
    {
        Transform tar = null;
        List<int> setIndex = new List<int>();
        bool atOneHavePlate = false;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            PlateSpawner ps = spawnPoints[i];
            if(ps.havePlate)
            {
                setIndex.Add(i);
                atOneHavePlate = true;
            }
        }

        if (atOneHavePlate == true)
        {
            int random = Random.Range(0, setIndex.Count);
            tar = spawnPoints[setIndex[random]].transform;
            currentPlate = spawnPoints[setIndex[random]];
            targetType = TargetType.Spawns;
        }
        return tar;
    }

    Transform getClosestPlayer(Transform thisPlayer, Transform[] otherPlayers, float distance)
    {
        Transform tar = null;

        List<Transform> playersWithPlate = new List<Transform>();
        foreach(Transform t in otherPlayers)
        {
            if (t.CompareTag(TagsLayers.enemyTag) && t.GetComponent<EnemyController>().havePlate && Vector3.Distance(thisPlayer.position, t.position) < distance)
                playersWithPlate.Add(t);
            else if (t.CompareTag(TagsLayers.playerTag) && t.GetComponent<PlayerController>().havePlates && Vector3.Distance(thisPlayer.position, t.position) < distance)
                playersWithPlate.Add(t);
        }

        if (playersWithPlate.Count == 0)
            return null;

        float dis = float.MaxValue;
        for(int i = 0; i < playersWithPlate.Count; i++)
        {
            float curDis = Vector3.Distance(thisPlayer.position, playersWithPlate[i].position);
            if (curDis < dis)
            {
                dis = curDis;
                tar = playersWithPlate[i];
            }
        }

        if(tar != null)
        {
            if (tar.CompareTag(TagsLayers.enemyTag))
            {
                currentPC = null;
                currentEC = tar.GetComponent<EnemyController>();
            }
            else
            {
                currentEC = null;
                currentPC = tar.GetComponent<PlayerController>();
            }
            targetType = TargetType.Players;
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
                if(navAgent.hasPath == false)
                {
                    enemyState = ENEMYSTATE.Idle;
                    Vector3 point = Vector3.zero;
                    if(RandomPoint(transform.position, 10, out point))
                    {
                        navAgent.SetDestination(point);
                        enemyState = ENEMYSTATE.Run;
                    }
                }
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

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
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
            targetType = TargetType.None;
            currentPlate = null;
            currentPC = null;
            currentEC = null;

            if(enemyState != ENEMYSTATE.Dodged && plates.Count != 0 && collision.gameObject.GetComponent<Table>() == SpawnsTables.currentDeliverTable)
            {
                powerHandler.updateScale(plates.Count);

                foreach (Transform t in plates)
                    Destroy(t.gameObject);

                plates.Clear();
                SpawnsTables.changeDeliverTable();
            }
        }

        if(collision.gameObject.CompareTag(TagsLayers.playerTag))
        {
            Interactor.handleInteraction(this, collision.gameObject.GetComponent<PlayerController>());
        }

        if(collision.gameObject.CompareTag(TagsLayers.enemyTag))
        {
            Interactor.handleInteraction(this, collision.gameObject.GetComponent<EnemyController>());
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

    void stealPlate(Transform target)
    {
        PlayerController pc = target.GetComponent<PlayerController>();
        List<Transform> stealablePlate = new List<Transform>();
        stealablePlate = pc.getPlates(this);

        carryPlatesFromOthers(stealablePlate);
    }

    void stealPlate(PlayerController pc)
    {
        List<Transform> stealedPlates = new List<Transform>();
        stealedPlates = pc.getPlates(this);
        carryPlatesFromOthers(stealedPlates);
    }

    void stealPlate(EnemyController ec)
    {
        List<Transform> stealedPlates = new List<Transform>();
        stealedPlates = ec.getPlates(this);
        carryPlatesFromOthers(stealedPlates);
    }

    void carryPlatesFromOthers(List<Transform> stealedPlates)
    {
        foreach (Transform t in stealedPlates)
            plates.Add(t);

        havePlate = true;
        plates.alignTransformPositions(plateStackDistance, Directions.Up, platesHolder);
    }

    void StartDodging(PlayerController pc)
    {
        StartDodging(pc.transform, pc.powerHandler.power);
    }

    void StartDodging(EnemyController ec)
    {
        StartDodging(ec.transform, ec.powerHandler.power);
    }

    void StartDodging(Transform other, float power)
    {
        navAgent.ResetPath();

        enemyState = ENEMYSTATE.Dodged;
        transform.forward = transform.getFaceDirection(other);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<Animator>().Play("Idle");

        if (lastDodgeRoutine != null)
            StopCoroutine(lastDodgeRoutine);

        lastDodgeRoutine = StartCoroutine(disableDodgeState(prop.dodgeTime));

        rb.AddForce(-transform.forward * (prop.dodgeForce + power) , ForceMode.Impulse);
    }

    IEnumerator disableDodgeState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        rb.constraints = RigidbodyConstraints.FreezeAll;
        enemyState = ENEMYSTATE.Idle;
    }

    //This functions is called when other player steal plates from this player
    public List<Transform> getPlates(PlayerController stealer)
    {
        StartDodging(stealer);
        return allPlates();
    }

    public List<Transform> getPlates(EnemyController stealer)
    {
        StartDodging(stealer);
        return allPlates();
    }

    List<Transform> allPlates()
    {
        List<Transform> result = new List<Transform>();
        foreach (Transform t in plates)
            result.Add(t);

        plates.Clear();
        return result;
    }

    public void getInteractionStatus(Interaction inter, PlayerController pc)
    {
        switch (inter)
        {
            case Interaction.Dodge:
                StartDodging(pc);
                break;

            case Interaction.Steal:
                stealPlate(pc);
                break;

            case Interaction.None:
                break;

            default:
                StartDodging(pc);
                break;
        }
    }

    public void getInteractionStatus(Interaction inter, EnemyController ec)
    {
        switch (inter)
        {
            case Interaction.Dodge:
                StartDodging(ec);
                break;

            case Interaction.Steal:
                stealPlate(ec);
                break;

            case Interaction.None:
                break;

            default:
                StartDodging(ec);
                break;
        }
    }

    public enum TargetType
    {
        Spawns,
        Players,
        Tables,
        None
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
