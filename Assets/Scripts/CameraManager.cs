using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public ControllerManager cManager;

    public Camera ownCam;
    private Vector3 lowestPoint;
    private Vector3 highestPoint;

    public Vector3 midPoint;

    [Header("distance from middle to corner of the screen")]
    public float distToFarthest;
    public Vector2 farthestDistanceClamp;
    public float dtfClamped;

    [Header("distance from camera to arena")]
    public float cameraDistance;
    public Vector2 camDistanceClamp;

    public Vector2 cameraVertOffsetClamp;

    public Transform Player2;

    public float dtfPercentage;

    public Vector2 arenaBounds;

    [Space]
    public Transform testLowPoint;
    public Transform testHighPoint;
    public Transform testMidPoint;

    public float smoothSpeed;
    void Update()
    {
        setupCamPoints();
        cameraMovement();
    }
    void setupCamPoints()
    {
        float lowestX = Player2.position.x;
        float lowestY = Player2.position.y;

        float highestX = Player2.position.x;
        float highestY = Player2.position.y;

        for (int i = 0; i < cManager.Players.Length; i++)
        {
            if (cManager.Players[i].Character.transform.position.x < lowestX)
            {
                lowestX = cManager.Players[i].Character.transform.position.x;
            }
            if (cManager.Players[i].Character.transform.position.y < lowestY)
            {
                lowestY = cManager.Players[i].Character.transform.position.y;
            }

            if (cManager.Players[i].Character.transform.position.x > highestX)
            {
                highestX = cManager.Players[i].Character.transform.position.x;
            }
            if (cManager.Players[i].Character.transform.position.y > highestY)
            {
                highestY = cManager.Players[i].Character.transform.position.y;
            }

            lowestX = Mathf.Clamp(lowestX, -arenaBounds.x, arenaBounds.x);
            lowestY = Mathf.Clamp(lowestY, -arenaBounds.y, arenaBounds.y);

            highestX = Mathf.Clamp(highestX, -arenaBounds.x, arenaBounds.x);
            highestY = Mathf.Clamp(highestY, -arenaBounds.y, arenaBounds.y);

            lowestPoint = new Vector3(lowestX, lowestY, 0);
            highestPoint = new Vector3(highestX, highestY, 0);
        }

        midPoint = Vector3.Lerp(lowestPoint, highestPoint, 0.5f);

        distToFarthest = Mathf.Max((highestX - lowestX), (highestY - lowestY));

        //distToFarthest = Vector3.Distance(midPoint, highestPoint);

        dtfClamped = Mathf.Clamp(distToFarthest, farthestDistanceClamp.x, farthestDistanceClamp.y);

        dtfPercentage = (dtfClamped - farthestDistanceClamp.x) / (farthestDistanceClamp.y - farthestDistanceClamp.x);
        //print(dtfPercentage);

        cameraDistance = Mathf.Lerp(camDistanceClamp.x, camDistanceClamp.y, dtfPercentage);

        testLowPoint.position = lowestPoint;
        testHighPoint.position = highestPoint;
        testMidPoint.position = midPoint;
    }
    void cameraMovement()
    {
        Vector3 targetPos = midPoint + Vector3.up * Mathf.Lerp(cameraVertOffsetClamp.x, cameraVertOffsetClamp.y, dtfPercentage) + Vector3.back * cameraDistance;
        ownCam.transform.position = Vector3.Lerp(ownCam.transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
