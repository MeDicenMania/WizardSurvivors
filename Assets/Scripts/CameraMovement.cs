using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    public Vector3 offset;


    void Update()
    {
        transform.position = target.position + offset;
    }

}
 



