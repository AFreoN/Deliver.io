using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb = null;
    Joystick joyStick = null;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        joyStick = GetComponent<Joystick>();

        getValuesFromProps();

        playerState = PLAYERSTATE.Idle;
        platesHolder = transform.GetChild(0).Find("Plates Holder");
    }

    void getValuesFromProps()
    {
        movementSpeed = playerProps.playerSpeed;
        speedReductionPerPlate = playerProps.speedReductionPerPlate;
        minPlayerSpeed = playerProps.minPlayerSpeed;

        minInputDistance = playerProps.minInputDistance;
        plateStackDistance = playerProps.plateStackDistance;
    }

    private void Update()
    {
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
                rb.velocity = transform.forward * movementSpeed;
                break;

            case PLAYERSTATE.CarryIdle:
                rb.velocity = Vector3.zero;
                break;

            case PLAYERSTATE.CarryRun:
                float clampedReduction = Mathf.Clamp((plates.Count * speedReductionPerPlate), 0, minPlayerSpeed);
                float currentSpeed = movementSpeed - clampedReduction;
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
            if (plates.Count == 0 || collision.gameObject.GetComponent<Table>() != SpawnsTables.currentDeliverTable)
                return;

            foreach(Transform t in plates)
                Destroy(t.gameObject);

            plates.Clear();
            SpawnsTables.changeDeliverTable();
        }

        if(collision.gameObject.CompareTag(TagsLayers.enemyTag))
        {
            EnemyController ec = collision.gameObject.GetComponent<EnemyController>();
            if (ec == null || playerState == PLAYERSTATE.Dodged)
                return;

            if(plates.Count == 0 && ec.plates.Count == 0)       //If both player and enemy don't have plates
            {
                handleEnemyCollision(InteractionStatus.None, collision);
            }
            else if(plates.Count == 0 && ec.plates.Count != 0)      //If enemy have plates
            {
                handleEnemyCollision(InteractionStatus.You, collision);
            }
            else if(plates.Count != 0 && ec.plates.Count != 0)      //If both enemy and player have plates
            {
                handleEnemyCollision(InteractionStatus.Both, collision);
            }
        }
    }

    void handleEnemyCollision(InteractionStatus status, Collision collision)
    {
        Debug.Log("Interaction Status = " + status);

        EnemyController ec = collision.gameObject.GetComponent<EnemyController>();
        if (ec == null)
            return;

        switch(status)
        {
            case InteractionStatus.None:

                playerState = PLAYERSTATE.Dodged;
                rb.AddForce(-transform.forward * playerProps.getDodgeForce(), ForceMode.Impulse);

                if (currentDodgeRoutine != null)
                    StopCoroutine(currentDodgeRoutine);

                currentDodgeRoutine = StartCoroutine( returnDodgeBack(playerProps.getDodgeTime()) );

                break;

            case InteractionStatus.You:

                bool inView = collision.transform.IsInView(transform, GameManager.viewAngle);  //Know whether the player is in enemy view or not by using extension method

                if (inView)      //If player is in enemy view, then player will dodge back
                {
                    playerState = PLAYERSTATE.Dodged;

                    Vector3 direction = (transform.normalizeXZ() - collision.transform.normalizeXZ()).normalized;
                    transform.forward = new Vector3(-direction.x, transform.position.y, -direction.z);
                    rb.AddForce(direction * playerProps.getDodgeForce(), ForceMode.Impulse);

                    if (currentDodgeRoutine != null)
                        StopCoroutine(currentDodgeRoutine);

                    currentDodgeRoutine = StartCoroutine(returnDodgeBack(playerProps.getDodgeTime()));
                }
                else        //Else player is not in enemy view, so player can steal the plate
                {
                    //plates = ec.getPlates();
                    plates.Clear();
                    plates = new List<Transform>();
                    foreach (Transform t in ec.getPlates())
                        plates.Add(t);

                    plates.alignTransformPositions(plateStackDistance, Directions.Up, platesHolder);
                }

                break;

            case InteractionStatus.Both:

                inView = collision.transform.IsInView(transform, GameManager.viewAngle);  //Know whether the player is in enemy view or not by using extension method

                if (inView == false)      //If player is not in view of enemy
                {
                    List<Transform> enemyPlates = ec.getPlates();
                    foreach (Transform t in enemyPlates)
                        plates.Add(t);

                    plates.alignTransformPositions(plateStackDistance, Directions.Up, platesHolder);
                }

                break;
        }
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
