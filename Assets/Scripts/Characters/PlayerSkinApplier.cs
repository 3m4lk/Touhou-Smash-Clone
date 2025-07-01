using TMPro;
using UnityEngine;

public class PlayerSkinApplier : MonoBehaviour
{
    public Material[] possibleSkins;
    public Material[] secondaryMats;

    public MeshRenderer[] mRs;
    public SkinnedMeshRenderer[] sMRs;

    public int testSkin;

    public TMP_Text skinName;
    private void Awake()
    {
        if (possibleSkins.Length == 0) return;
        applySkin(testSkin);
    }
    private void Update()
    {
        if (Input.GetKeyDown("u"))
        {
            if (skinName)
            {
                testSkin = (int)Mathf.Repeat(testSkin + 1, possibleSkins.Length);
                applySkin(testSkin);
                skinName.text = GetComponent<PlayerMoveset>().moveset.fullName + "'s skin: " + possibleSkins[testSkin].name;
            }
        }
    }
    public void applySkin(int input)
    {
        for (int i = 0; i < mRs.Length; i++)
        {
            //print("mrs " + i + ": " + mRs[i].materials.Length);

            Material[] matsToApply = new Material[1];

            if (mRs[i].sharedMaterials.Length > 1 && secondaryMats.Length > 0)
            {
                matsToApply = new Material[2];
                matsToApply[1] = secondaryMats[input];
            }
            matsToApply[0] = possibleSkins[input];

            if (Application.isEditor)
            {
                mRs[i].sharedMaterials = matsToApply;
            }
            else
            {
                mRs[i].materials = matsToApply;
            }
        }

        for (int i = 0; i < sMRs.Length; i++)
        {
            //print("smrs " + i + ": " + sMRs[i].materials.Length);

            Material[] matsToApply = new Material[1];

            if (sMRs[i].sharedMaterials.Length > 1 && secondaryMats.Length > 0)
            {
                matsToApply = new Material[2];
                matsToApply[1] = secondaryMats[input];
            }
            matsToApply[0] = possibleSkins[input];

            if (Application.isEditor)
            {
                sMRs[i].sharedMaterials = matsToApply;
            }
            else
            {
                sMRs[i].materials = matsToApply;
            }
        }

        print("<color=yellow> Skin Applied: " + possibleSkins[input].name);
    }
}
