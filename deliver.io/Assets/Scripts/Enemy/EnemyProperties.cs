using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProp", menuName = "Enemy Property")]
public class EnemyProperties : ScriptableObject
{
    [Header("Movement Values")]
    [SerializeField][Range(1,10)] float MovementSpeed = 4;
    [SerializeField][Range(0.1f,1)] float SpeedReductionPerPlate = .4f;
    [SerializeField][Range(0.5f,10)] float MinMoveSpeed = 2;

    [Header("Dodge Values")]
    [Space(10)]
    [SerializeField] float DodgeForce = 150;
    [SerializeField] float DodgeForceTolerance = 15;
    [SerializeField][Range(0.1f,1)] float DodgeTime = 0.5f;
    [SerializeField][Range(0,0.5f)] float DodgeTimeTolerance = 0.15f;

    [Header("Other Player Interaction Values")]
    [SerializeField][Range(.1f,2)] float OtherPlayerCheckDelay = 1f;
    [SerializeField][Range(0,.5f)] float CheckDelayTolerance = .1f;
    [SerializeField][Range(1, 15)] float CheckRadius = 10f;

    [Header("Plates")]
    [Space(10)]
    [SerializeField][Range(0.02f,0.5f)] float PlateStackDistance = .15f;

    public float movementSpeed => MovementSpeed;
    public float speedReductionPerPlate => SpeedReductionPerPlate;
    public float minMoveSpeed => MinMoveSpeed;

    public float checkRadius => CheckRadius;

    public float plateStackDistance => PlateStackDistance;

    public float dodgeForce { get { return getDodgeForce(); } }
    public float dodgeTime { get { return getDodgeTime(); } }
    public float checkDelay { get { return OtherPlayerCheckDelay; } }

    float getDodgeForce()
    {
        float min = DodgeForce - DodgeForceTolerance;
        float max = DodgeForce + DodgeForceTolerance;

        return Random.Range(min, max);
    }

    float getDodgeTime()
    {
        float min = DodgeTime - DodgeTimeTolerance;
        float max = DodgeTime + DodgeTimeTolerance;

        return Random.Range(min, max);
    }

    float getCheckDelay()
    {
        float min = OtherPlayerCheckDelay - CheckDelayTolerance;
        float max = OtherPlayerCheckDelay + CheckDelayTolerance;

        return Random.Range(min, max);
    }
}
