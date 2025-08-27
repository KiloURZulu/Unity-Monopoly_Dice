using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_playerMove : MonoBehaviour
{
    [SerializeField] public Scr_createArea CreateArea;
    [SerializeField] public scr_rollDice rollDice;
    [SerializeField] private scr_ScriptableObject_Data SO_Data;

    [SerializeField] private float moveSpeed = 5.0f; // Speed at which the player moves

    [SerializeField] public int totalBoxCreate = 0;

    [SerializeField] public int Laps = 0;
    [SerializeField] public int diceResult = 0;
    [SerializeField] public int count_position = 0;
    [SerializeField] public int lastPlace = 0;
    [SerializeField] public int totalTravels = 0;
    [SerializeField] public bool changeStartIcon = false;

    [SerializeField] public bool normalizePlayer = false;  // flag normalize the player angle
    [SerializeField] public bool isMoving = false;          // Flag to control movement
    
    public bool player_firstRun = false;    //  first run (set the last run location)
    //public bool player_firstRun_fix = false;    //  first run (set the last run location)
    public bool player_dieRun = false;
    public bool passingStart = false;
    public bool saveToRoll = false;

    public int collisionEdgeRotate = 0;
    public int collisionStart_Bends = 0;

    [SerializeField] public int moveByDie = 0;

    //private Coroutine moveCoroutine;

    private Coroutine FisrtStartCoroutine;

    void ReState()
    {
        lastPlace = 0;
        diceResult = 0;
        count_position = 0;
        Laps = 0;
        totalBoxCreate = 0;
        collisionEdgeRotate = 0;
        collisionStart_Bends = 0;
        moveByDie = 0;

        player_firstRun = false;
        //player_firstRun_fix = false;
        player_dieRun = false;
        isMoving = false;
        passingStart = false;
        normalizePlayer = false;
        saveToRoll = true;
        changeStartIcon = false;

        moveSpeed = SO_Data.Player_move_Spd;

    }


    // Awake is called before Start
    private void Awake()
    {
        
        if (CreateArea == null)
        {
            // Try to find an object of type Scr_createArea in the scene
            CreateArea = FindObjectOfType<Scr_createArea>();

            if (CreateArea != null)
            {
                Debug.Log("CreateArea found and assigned in Awake.");
            }
            else
            {
                Debug.LogError("CreateArea is not assigned and could not be found in the scene!");
            }
        }

        if (rollDice == null)
        {
            // Try to find an object of type Scr_createArea in the scene
            rollDice = FindObjectOfType<scr_rollDice>();

            if (rollDice != null)
            {
                Debug.Log("scr_rollDice found and assigned in Awake.");
            }
            else
            {
                Debug.LogError("rollDice is not assigned and could not be found in the scene!");
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        ReState();
        //normalizePlayer = false;
        InitializeData();

    }

    void InitializeData()
    {
        if (CreateArea != null)
        {
            lastPlace = CreateArea.PlayerlastPlace;
            if (lastPlace == 0)
            {
                lastPlace = 0;
                count_position = 0;
                changeStartIcon = false;

            }
            else
            {
                lastPlace -= 2;
                count_position = 2;
                changeStartIcon = true;
            }

            Laps = CreateArea.PlayerLaps;
            totalBoxCreate = CreateArea.CountTotalAmountBoxs;

            Debug.Log($"InitializeData: lastPlace = {lastPlace}, Laps = {Laps}, Boxs = {totalBoxCreate}");

            // Start the firstStart coroutine
            FisrtStartCoroutine = StartCoroutine(firstStart());

            // Find the icon image pivot transform

        }
        else
        {
            Debug.LogError("CreateArea is not assigned!");
        }
    }
    /*
    IEnumerator firstStart()
    {
        yield return new WaitForSecondsRealtime(0.40f);

        Transform iconImageTransform = transform.Find("Icon_Image_Pivot");

        if (iconImageTransform != null)
        {
            // Get the current rotation of the parent transform
            Vector3 parentRotation = transform.rotation.eulerAngles;
            // Set the X and Y rotations to zero, keeping the Z rotation
            transform.rotation = Quaternion.Euler(0, 0, parentRotation.z);

            // Get the current rotation of the icon image transform
            Vector3 iconImageRotation = iconImageTransform.rotation.eulerAngles;
            // Set the X and Y rotations to zero, keeping the Z rotation
            iconImageTransform.rotation = Quaternion.Euler(0, 0, iconImageRotation.z);
        }
        else
        {
            Debug.LogError("Icon_Image_Pivot transform not found!");
        }

        yield return new WaitForSecondsRealtime(0.10f);

        normalizePlayer = true;
        yield break;
    }
    */
    
    IEnumerator firstStart()
    {

        yield return new WaitForSecondsRealtime(0.40f);

        Transform iconImageTransform = transform.Find("Icon_Image_Pivot");

        if (iconImageTransform != null)
        {
            // Check if the parent transform's rotation is already (0, 0, 0)
            if (transform.rotation != Quaternion.Euler(Vector3.zero))
            {
                // If not, set the parent's rotation to (0, 0, 0)
                transform.rotation = Quaternion.Euler(Vector3.zero);
            }

            // Set the icon image transform's rotation to (0, 0, 0)
            iconImageTransform.rotation = Quaternion.Euler(Vector3.zero);
            
        }
        else
        {
            Debug.LogError("Icon_Image_Pivot transform not found!");
        }

        yield return new WaitForSecondsRealtime(0.10f);

        normalizePlayer = true;
        yield break;

    }
    
    //---------------------------------------------------------------- AFTER INITIALIZE -----------------

    IEnumerator passingWait(float delayTime)
    {
        isMoving = false;

        lastPlace -= count_position;
        count_position -= count_position;
        Laps += 1;

        passingStart = true;

        yield return new WaitForSecondsRealtime(delayTime);

        passingStart = false;

        yield return new WaitForSecondsRealtime(delayTime);

        isMoving = true;

    }

    // Update is called once per frame
    void Update()
    {
        totalTravels = Laps * totalBoxCreate + lastPlace;

        if (rollDice.dieRollsEnd1 && rollDice.dieRollsEnd2)
        {
            diceResult = rollDice.dieRolls1 + rollDice.dieRolls2;

        }

        if (isMoving)                   // moving the player command
        {
            if (lastPlace > count_position)     // move when not reach position
            {
                saveToRoll = false;
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            }
            else if (lastPlace == count_position)   //stoping it self when reach position
            {
                isMoving = false;
                saveToRoll = true;

            }

        }

        if (normalizePlayer && CreateArea != null)      //initial the positioning from the last play
        {
            if (!player_firstRun )
            {
                if (lastPlace != CreateArea.PlayerlastPlace)
                {
                    lastPlace = CreateArea.PlayerlastPlace;
                }

                Laps = CreateArea.PlayerLaps;
                player_firstRun = true;

                isMoving = true;
                
            }

        }

        if (player_firstRun)    
        {
            if (player_dieRun)  //the die roll runs...
            {
                lastPlace += diceResult;
                isMoving = true;

                player_dieRun = false;

                if (passingStart)   // if passing start the next roll remove the passing start
                {
                    passingStart = false;
                }

            }
        }

       
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (normalizePlayer)
        {
            // Log the collision information for debugging purposes
            Debug.Log("AA-Collision detected with: " + collision.name);

            Transform iconImageTransform = transform.Find("Icon_Image_Pivot");
            // Check if the collider has the tag "center_box"
            if (collision.CompareTag("center_Box"))
            {
                Debug.Log("center is definitley hit");

                count_position += 1;    // Increment the counter

                transform.position = new Vector3(collision.transform.position.x, collision.transform.position.y,transform.position.z);    // Move the object to the position of the collided object

                if (collisionEdgeRotate == 1)
                {
                    transform.Rotate(0, 0, -90);
                    iconImageTransform.Rotate(0, 0, 90);    // Rotate the child named "iconImage" by 90 degrees

                }

            }

            // Check if the object name starts with "box-" or has the tag "main_Box"
            if (collision.CompareTag("main_Box"))
            {
                // Attempt to get the scr_resizeBox script from the collided object
                scr_resizeBox resizeBoxScript = collision.GetComponent<scr_resizeBox>();
                if (resizeBoxScript != null)
                {
                    // If the scr_resizeBox script is found, retrieve its rotation value
                    collisionEdgeRotate = resizeBoxScript.EdgeRotate;
                    collisionStart_Bends = resizeBoxScript.start_bends;

                    Debug.Log("Transferring collisionEdgeRotate: " + collisionEdgeRotate);

                    if (resizeBoxScript.id == 3)
                    {
                        changeStartIcon = true;
                    }

                    if (resizeBoxScript.id == 0)    // id = 0 id "start" position
                    {
                        StartCoroutine(passingWait(0.01f));

                        //passingStart = false;

                        //lastPlace -= count_position;
                        //count_position = 0;

                    }

                }
                else
                {
                    Debug.LogWarning("scr_resizeBox script not found on collided object: " + collision.gameObject.name);
                }
            }
            else
            {
                Debug.Log("Object does not meet criteria for collision: " + collision.gameObject.name);
            }
        }
    }

}

