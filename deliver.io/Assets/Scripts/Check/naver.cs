using UnityEngine;
using UnityEngine.AI;

public class naver : MonoBehaviour
{
    NavMeshAgent agent;
    public float distance = 10;
    public float velocity = 10;

    Vector3 targetPosition;

    float temp = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;

        targetPosition = transform.position + Vector3.forward * distance;
        agent.SetDestination(targetPosition + Vector3.forward);
    }

    private void Update()
    {
        temp += Time.deltaTime;

        if (transform.position.z >= targetPosition.z)
        {
            agent.isStopped = true ;
            Debug.Log("Naver Time = " + temp);
            Destroy(this);
        }
    }

}
