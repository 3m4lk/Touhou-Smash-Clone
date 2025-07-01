using UnityEngine;
public enum ailv
{
    Easy,
    Normal,
    Hard
}
public class AIController : MonoBehaviour
{
    public MatchManager ownMatch;

    public PlayerStats ownStats;
    public PlayerMoveset ownMoveset;

    public ailv aiLevel;
    private float coolTillHard;

    public int PlayerDeathsToDropDiff = -1;

    // easy: 1.7f
    // norm: 0.7f
    // hard: 0.2f

    public float[] reflexes = new float[3] { 1.7f, 0.7f, 0.2f };

    public float reflexProg;

    private int diffInt;

    private string commandsBacklog;

    public Vector2 offsetToPlayer;

    public Vector2 inputAxis;

    private void Awake()
    {
        changeDiff(ailv.Normal);
        coolTillHard = 10f;
    }
    private void Update()
    {
        offsetToPlayer = ownMatch.realPlayer.position = transform.position;

        coolTillHard = Mathf.Max(coolTillHard - Time.deltaTime, 0f);

        reflexProg -= Time.deltaTime;
        for (; reflexProg < 0f; reflexProg += reflexes[diffInt])
        {
            clearBacklog();

            inputAxis = new Vector2(Mathf.Sign(offsetToPlayer.x), Mathf.Sign(offsetToPlayer.y));

            if (ownMatch.realPlayer.position.y > transform.position.y && ownMoveset.ownRb.linearVelocityY <= 0f)
            {
                //print("TRYJUMP");
                if (ownMoveset.jumpsRemaining > 0)
                {
                    //print("JUMP");
                    sendCommand("mJ");
                }// jump
                else
                {
                    sendCommand("mU");
                    sendCommand("shot");
                } // recovery

                //ownMoveset.processAxis("horizontal", Mathf.Sign(offsetToPlayer.x));

                if (Mathf.Sign(offsetToPlayer.x) == -1f)
                {
                    //sendCommand("mL");
                }
                else
                {
                    //sendCommand("mR");
                }
            }

            string[] axisHor = new string[] { "mL", "mR" };
            string[] axisVert = new string[] { "mD", "mU" };

            //sendCommand()

            sendBacklog();
        } // here be ai behaviors
    }

    public void loseStockBehavior(int stocks)
    {
        if (coolTillHard > 0f)
        {
            coolTillHard = 0f;
            changeDiff(ailv.Hard);
            PlayerDeathsToDropDiff = 2;
            return;
        } // sw2itch 2 hard on quick defeat
        if (stocks == 1)
        {
            if (aiLevel == ailv.Easy) aiLevel = ailv.Normal;
        }
    }
    public void dropDiff()
    {
        if (PlayerDeathsToDropDiff > 0)
        {
            PlayerDeathsToDropDiff--;
            if (PlayerDeathsToDropDiff == 0)
            {
                switch (aiLevel)
                {
                    case ailv.Normal:
                        changeDiff(ailv.Easy);
                        break;
                    case ailv.Hard:
                        changeDiff(ailv.Normal);
                        break;
                }
            }
        }
    }
    void changeDiff(ailv input)
    {
        aiLevel = input;
        switch (input)
        {
            case ailv.Easy:
                diffInt = 0;
                break;
            case ailv.Normal:
                diffInt = 1;
                break;
            case ailv.Hard:
                diffInt = 2;
                break;
            default:
                break;
        }
    }
    void sendCommand(string input)
    {
        commandsBacklog += input + "|";
    }
    void sendBacklog()
    {
        if (commandsBacklog == default) return;

        string[] commsToCancel = commandsBacklog.Split("|");

        for (int i = 0; i < commsToCancel.Length; i++)
        {
            ownMoveset.processInput(commsToCancel[i], true);
        }
    }
    void clearBacklog()
    {
        if (commandsBacklog == default) return;

        string[] commsToCancel = commandsBacklog.Split("|");

        for (int i = 0; i < commsToCancel.Length; i++)
        {
            ownMoveset.processInput(commsToCancel[i], false);
        }

        commsToCancel = default;
    }
}
