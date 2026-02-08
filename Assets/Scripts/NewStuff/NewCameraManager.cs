using UnityEngine;

public class NewCameraManager : MonoBehaviour
{
    public Camera ownCam;
    public Transform[] camItems;
    public float smoothSpeed;
    public Vector2 arenaBounds, farthestDistanceClamp, camDistanceClamp, cameraVertOffsetClamp;

    Vector3 lowestPoint, highestPoint, midPoint;
    float distToFarthest, dtfClamped, dtfPercentage, cameraDistance;
    void Update()
    {
        setupCamPoints();
        cameraMovement();
    }
    void setupCamPoints()
    {
        float lowestX = camItems[0].position.x;
        float lowestY = camItems[0].position.y;

        float highestX = camItems[0].position.x;
        float highestY = camItems[0].position.y;

        for (int i = 0; i < camItems.Length; i++)
        {
            lowestX = Mathf.Min(lowestX, camItems[i].position.x);
            highestX = Mathf.Max(highestX, camItems[i].position.x);
            lowestY = Mathf.Min(lowestY, camItems[i].position.y);
            highestY = Mathf.Max(highestY, camItems[i].position.y);

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

        //testLowPoint.position = lowestPoint;
        //testHighPoint.position = highestPoint;
        //testMidPoint.position = midPoint;
    }
    void cameraMovement()
    {
        Vector3 targetPos = midPoint + Vector3.up * Mathf.Lerp(cameraVertOffsetClamp.x, cameraVertOffsetClamp.y, dtfPercentage) + Vector3.back * cameraDistance;
        ownCam.transform.position = Vector3.Lerp(ownCam.transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
