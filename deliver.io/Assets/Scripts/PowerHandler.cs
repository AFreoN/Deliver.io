using UnityEngine;
using UnityEngine.UI;

public class PowerHandler
{
    public Transform thisPlayer;
    Rigidbody rb;
    Text levelText;

    public float currentScale;
    public float power;
    public float speed;

    public int platesDelivered;

    public PowerHandler(Transform t, float scale)
    {
        thisPlayer = t;
        rb = t.GetComponent<Rigidbody>();
        levelText = t.GetComponent<PlayerCanvas>().levelText;
        levelText.text = "0";

        currentScale = scale;
        power = 0;
        speed = 0;

        platesDelivered = 0;

        thisPlayer.localScale = Vector3.one * currentScale;
    }

    public void updateScale(int increasedPlate)
    {
        currentScale += (increasedPlate * ControlValues.scaleIncPerPlate);
        power += (increasedPlate * ControlValues.powerIncPerPlate);
        speed += (increasedPlate * ControlValues.speedIncPerPlate);

        thisPlayer.localScale = Vector3.one * currentScale;
        rb.mass  += (increasedPlate * ControlValues.massIncPerPlate);

        platesDelivered += increasedPlate;
        levelText.text = platesDelivered.ToString();
    }
}
