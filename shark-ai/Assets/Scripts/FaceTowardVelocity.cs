using UnityEngine;

public class FaceTowardVelocity : MonoBehaviour
{
    Quaternion rotation;

    void Awake()
    {
        rotation = transform.rotation;
    }

    void LateUpdate()
    {
        //transform.rotation = Quaternion.LookRotation(dir);
        transform.position = transform.parent.transform.position;
        transform.LookAt(transform.position + GetComponentInParent<Rigidbody>().velocity);
    }
}
