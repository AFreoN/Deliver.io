using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProp", menuName = "Player Properties")]
public class PlayerProperties : ScriptableObject
{
    [Header("Movement Values")]
    [SerializeField] float PlayerSpeed = 300;
    [SerializeField] float SpeedReductionPerPlate = 30;
    [SerializeField] float MinPlayerSpeed = 150;

    [Header("Dodge Values")]
    [SerializeField] float DodgeForce = 10;
    [SerializeField] float DodgeForceTolerance = 5;
    [SerializeField][Range(.1f,2)] float DodgeTime = 0.5f;
    [SerializeField][Range(.1f,.5f)] float DodgeTimeTolerance = .25f;

    [Header("Plate Stack")]
    [Space(10)]
    [SerializeField][Range(.02f,.2f)] float PlateStackDistance = .05f;

    [Header("Inputs")]
    [Space(10)]
    [SerializeField][Range(0,1)] float MinInputDistance = .2f;

    public float playerSpeed => PlayerSpeed;
    public float speedReductionPerPlate => SpeedReductionPerPlate;
    public float minPlayerSpeed => MinPlayerSpeed;
    public float plateStackDistance => PlateStackDistance;
    public float minInputDistance => MinInputDistance;

    public float getDodgeTime()
    {
        float min = DodgeTime - DodgeTimeTolerance;
        float max = DodgeTime + DodgeTimeTolerance;

        return Random.Range(min, max);
    }

    public float getDodgeForce()
    {
        float min = DodgeForce - DodgeForceTolerance;
        float max = DodgeForce + DodgeForceTolerance;

        return Random.Range(min, max);
    }
}
