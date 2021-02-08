using UnityEngine;

public class GoTween : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] float rotDuration = .5f;

    [Header("Scale")][Space(10)]
    [SerializeField] float scaleDuration = .5f;
    [SerializeField] float minScale = .8f;
    [SerializeField] float maxScale = 1.2f;

    float tempScale = 0, tempRot = 0;
    bool scaleUp = true;

    Vector3 initScale,minScaledVector, maxScaledVector;

    void Start()
    {
        tempScale = 1;
        tempRot = 0;
        scaleUp = true;

        initScale = transform.localScale;
        minScaledVector = transform.localScale * minScale;
        maxScaledVector = transform.localScale * maxScale;
    }

    private void Update()
    {
        ScaleTween();
        RotTween();
    }

    void ScaleTween()
    {
        if(scaleUp)
        {
            tempScale += Time.deltaTime / scaleDuration / .5f;

            if(tempScale >= maxScale)
            {
                scaleUp = false;
                return;
            }

            transform.localScale = initScale * tempScale;
        }
        else
        {
            tempScale -= Time.deltaTime / scaleDuration / .5f;

            if(tempScale <= minScale)
            {
                scaleUp = true;
                return;
            }

            transform.localScale = initScale * tempScale;
        }
    }

    void RotTween()
    {
        tempRot += Time.deltaTime * 360 / rotDuration;
        tempRot = Mathf.Repeat(tempRot, 360);

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(Vector3.up * tempRot),.2f);
    }

}
