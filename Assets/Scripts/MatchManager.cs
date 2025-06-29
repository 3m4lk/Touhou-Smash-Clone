using UnityEngine;

public class MatchManager : MonoBehaviour
{
    //[Header("replace with PlayerStats l8r")]
    public PlayerStats[] Players;

    public Vector2 arenaSize;

    public Transform[] respawnLocations;
    private bool[] platformsOccupied;

    private void Awake()
    {
        platformsOccupied = new bool[respawnLocations.Length];
    }
    private void FixedUpdate()
    {
        blastZoneThingy();
    }
    void blastZoneThingy()
    {
        for (int i = 0; i < Players.Length; i++)
        {
            if (Mathf.Abs(Players[i].transform.position.x) > arenaSize.x || Mathf.Abs(Players[i].transform.position.y) > arenaSize.y)
            {
                print("Player oob: " + Players[i].name);
                Players[i].kill();
            }
        }
    }
    public int pickRespawnPlatform()
    {
        int randLoc = 0;
        for (int i = 0; i < 10; i++)
        {
            randLoc = Random.Range(0, respawnLocations.Length);

            if (!platformsOccupied[randLoc]) return randLoc;
        }
        return 0; // if 10 checks fail (SOMEHOW?!?!?!?!?!?!?!??), just respawn on platform 0 (can cause overlap but that's the drawback aye)
    }
    public void freeRespPlatform(int input)
    {
        platformsOccupied[input] = false;
    }
}
