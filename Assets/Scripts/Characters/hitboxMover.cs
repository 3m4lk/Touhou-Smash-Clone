using UnityEngine;

public class hitboxMover : MonoBehaviour
{
    public bool dontMove;
    public Transform refPoint;
    public bool keepRotation;
    private Quaternion ogRotation;
    private void Awake()
    {
        if (keepRotation)
        {
            ogRotation = transform.rotation;
        }
    }
    private void Update()
    {
        if (!dontMove) transform.position = refPoint.position;
        if (keepRotation) transform.rotation = ogRotation;
    }
}
