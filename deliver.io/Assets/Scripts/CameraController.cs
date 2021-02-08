using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target = null;
    [SerializeField][Range(0,1)] float LerpSpeed = .2f;
    Vector3 offset = Vector3.zero;

    void Start()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, LerpSpeed);
    }

}
