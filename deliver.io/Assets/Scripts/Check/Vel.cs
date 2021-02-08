using UnityEngine;

public class Vel : MonoBehaviour
{
    public float distance = 10;
    public float velocity = 10;

    Vector3 targetPosition;

    Rigidbody rb;

    float temp = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position + Vector3.forward * distance;

        //rb.velocity = Vector3.forward * velocity;
    }

    private void Update()
    {
        temp += Time.deltaTime;
        if(transform.position.z >= targetPosition.z)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            Debug.Log("Vel Temp = " + temp);
            Destroy(this);
        }
        rb.velocity = Vector3.forward * velocity;
    }

}
