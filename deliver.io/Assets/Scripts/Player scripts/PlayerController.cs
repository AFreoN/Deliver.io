using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb = null;
    Joystick joyStick = null;
    [HideInInspector] public PowerHandler powerHandler = null;

    [SerializeField] PlayerProperties playerProps = null;

    float movementSpeed = 0;
    float speedReductionPerPlate = 0;
    float minPlayerSpeed = 0;
    float minInputDistance = 0;

    public PLAYERSTATE playerState { get; private set; }
    Coroutine currentDodgeRoutine = null;

    //For Plates
    float plateStackDistance = .1f;
    Transform platesHolder = null;
    [SerializeField] List<Transform> plates = new List<Transform>();
    [HideInInspector] public bool havePlates = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joyStick = GetComponent<Joystick>();
        powerHandler = new PowerHandler(transform, 1);
    }

    private void Start()
    {
        getValuesFromProps();

        playerState = PLAYERSTATE.Idle;
        platesHolder = transform.GetChild(0).Find("Plates Holder");
    }

    void getValuesFromProps()
    {
        movementSpeed = playerProps.playerSpeed;
        speedReductionPerPlate = ControlValues.speedReductionPerPlate;
        minPlayerSpeed = playerProps.minPlayerSpeed;

        minInputDistance = playerProps.minInputDistance;
        plateStackDistance = playerProps.plateStackDistance;
    }

    private void Update()
    {
        havePlates = plates.Count > 0 ? true : false;

        if(GameManager.gameState == GameState.InGame && playerState != PLAYERSTATE.Dodged && joyStick.minDisMoved(minInputDistance))
        {
            if (plates.Count != 0)
                playerState = PLAYERSTATE.CarryRun;
            else
                playerState = PLAYERSTATE.Run;

            transform.rotation = Quaternion.Euler(Vector3.up * Joystick.angle);
        }
        else if(playerState != PLAYERSTATE.Dodged)
        {
            if(plates.Count != 0)
            {
                playerState = PLAYERSTATE.CarryIdle;
            }
            else
                playerState = PLAYERSTATE.Idle;
        }
    }

    private void FixedUpdate()
    {

        switch(playerState)
        {
            case PLAYERSTATE.Idle:
                rb.velocity = Vector3.zero;
                break;

            case PLAYERSTATE.Run:
                rb.velocity = transform.forward * (movementSpeed + powerHandler.speed);
                break;

            case PLAYERSTATE.CarryIdle:
                rb.velocity = Vector3.zero;
                break;

            case PLAYERSTATE.CarryRun:
                float normalVelocity = movementSpeed + powerHandler.speed;
                float reduction = 1 - plates.Count * speedReductionPerPlate;        //Speed reduction by carrying plates
                float currentSpeed = Mathf.Clamp(normalVelocity * reduction, minPlayerSpeed, Mathf.Infinity);
                rb.velocity = transform.forward * currentSpeed;
                break;

            case PLAYERSTATE.Dodged:
                rb.AddForce(transform.forward * -10 * 50 * Time.fixedDeltaTime);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag(TagsLayers.spawnPointTag))
        {
            PlateSpawner ps = collision.gameObject.GetComponent<PlateSpawner>();
            if(ps != null && ps.havePlate)
            {
                CarryPlate(ps);
            }
        }

        if(collision.gameObject.CompareTag(TagsLayers.tableTag))
        {
            if (plates.Count != 0 && collision.gameObject.GetComponent<Table>() == SpawnsTables.currentDeliverTable)
            {
                powerHandler.updateScale(plates.Count);

                foreach (Transform t in plates)
                    Destroy(t.gameObject);

                plates.Clear();
                SpawnsTables.changeDeliverTable();
            }
        }

        if(collision.gameObject.CompareTag(TagsLayers.enemyTag))
        {
            Interactor.handleInteraction(this, collision.gameObject.GetComponent<EnemyController>());
        }
    }

    void startDodge(EnemyController other)
    {
        playerState = PLAYERSTATE.Dodged;

        float enemyPower = other.powerHandler.power;

        transform.forward = transform.getFaceDirection(other.transform);

        rb.AddForce(-transform.forward * (playerProps.dodgeForce + enemyPower), ForceMode.Impulse);

        if (currentDodgeRoutine != null)
            StopCoroutine(currentDodgeRoutine);

        currentDodgeRoutine = StartCoroutine(returnDodgeBack(playerProps.dodgeTime));
    }

    void stealPlate(EnemyController other)
    {
        if (other == null || other.havePlate == false)
            return;

        List<Transform> stealablePlates = new List<Transform>();
        stealablePlates = other.getPlates(this);

        foreach (Transform t in stealablePlates)
            plates.Add(t);

        plates.alignTransformPositions(plateStackDistance, Directions.Up, platesHolder);
    }

    IEnumerator returnDodgeBack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        playerState = PLAYERSTATE.Idle;
    }

    void CarryPlate(PlateSpawner ps)
    {
        if (playerState == PLAYERSTATE.Dodged)
            return;

        if (joyStick.minDisMoved(minInputDistance))
            playerState = PLAYERSTATE.CarryRun;
        else
            playerState = PLAYERSTATE.CarryIdle;

        Transform t = ps.getPlate();
        plates.Add(t);
        t.SetParent(platesHolder);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localPosition = t.up * plateStackDistance * (plates.Count - 1);
    }

    public List<Transform> getPlates(EnemyController stealer)
    {
        startDodge(stealer);

        List<Transform> result = new List<Transform>();
        foreach (Transform t in plates)
            result.Add(t);

        plates.Clear();
        havePlates = false;

        return result;
    }

    public void GetInteractiosStatus(Interaction inter, EnemyController other)
    {
        switch(inter)
        {
            case Interaction.Dodge:
                startDodge(other);
                break;

            case Interaction.Steal:
                stealPlate(other);
                break;

            default:
                break;
        }
    }
}

public enum PLAYERSTATE
{
    Idle,
    Run,
    CarryIdle,
    CarryRun,
    Dodged
}

public enum InteractionStatus
{
    I,
    You,
    None,
    Both
}
