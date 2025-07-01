using UnityEngine;

public class FaceManager : MonoBehaviour
{
    public Material[] faces;
    //public int hurtInt; // always 1 // he's the one... // COMING !!!~!!
    public float hurtTime;
    public MeshRenderer face;
    private int desiredInt;
    private void FixedUpdate()
    {
        hurtTime = Mathf.Max(hurtTime - Time.fixedDeltaTime, 0f);

        if (hurtTime == 0f) face.sharedMaterial = faces[desiredInt];
        else face.sharedMaterial = faces[1];
    }
    public void changeFace(int index)
    {
        desiredInt = index;

        if (hurtTime == 0f) face.sharedMaterial = faces[desiredInt];
    }
    public void hurt()
    {
        hurtTime = 0.4f;
        face.sharedMaterial = faces[1];
    }
}