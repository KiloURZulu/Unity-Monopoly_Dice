using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Scr_createArea : MonoBehaviour
{
    [SerializeField] private scr_ScriptableObject_Data SO_Data;

    [SerializeField] private GameObject boxPrefab;      // Reference to the prefab of the box
    [SerializeField] private GameObject centerGameArea; // Parent object for the boxes
    [SerializeField] private GameObject playerIconPrefab; // Changed the name for clarity

    [SerializeField] private const int startPlace = 0;
    [SerializeField] private int X_area_length = 5;  // Number of points along X-axis
    [SerializeField] private int Y_area_length = 5;  // Number of points along Y-axis

    [SerializeField] public int CountTotalAmountBoxs = 0;

    [SerializeField] private float spacingX = 1.5f;     // spacing between box in X direction
    [SerializeField] private float spacingY = 1.5f;     // spacing between box in Y direction

    //[SerializeField] private bool boxNamingDone = false;
    [SerializeField] private bool trigger1x_001 = false;
    [SerializeField] private bool trigger1x_002 = false;

    public bool playerCreated = false;
    
    [Tooltip("for create grid Box")] public bool grid_Create_bool = false;
    [Tooltip("for clean Up grid Box")] public bool grid_Destroy_bool = false;

    public Vector2 startPos;
    public float boxResize_Delay = 0.1f;
    public int PlayerlastPlace = 0;
    public int PlayerLaps = 0;

    private GameObject instantiatedPlayerIcon; // Store the instantiated player icon


    void ReSetState()
    {
        //boxNamingDone = false;
        CountTotalAmountBoxs = 0;

        startPos = Vector2.zero;

        grid_Create_bool = false;
        trigger1x_001 = false;

        grid_Destroy_bool = false;
        trigger1x_002 = false;
    }

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        X_area_length = SO_Data.X_AreaLength;
        Y_area_length = SO_Data.Y_AreaLength;

        spacingX = SO_Data.spacingX_Dir;
        spacingY = SO_Data.spacingY_Dir;
    }

    private void Start()
    {
        ReSetState();
        StartCoroutine(BuiltGrid(1.0f));
    }

    IEnumerator BuiltGrid(float Create_TimeDelay)
    {
        yield return new WaitForSecondsRealtime(Create_TimeDelay);

        grid_Create_bool = true;

        yield return null;

    }

    private void Update()
    {

        if (grid_Create_bool && !trigger1x_001)
        {
            grid_Destroy_bool = false;
            trigger1x_002 = false;

            //grid_Create_bool = true;
            trigger1x_001 = true;

            Grid_Create();

            //RenameBoxes();

            placePlayer(0.5f); // Call placePlayer() after creating the grid

        }

        if (grid_Destroy_bool && !trigger1x_002)
        {
            grid_Create_bool = false;
            trigger1x_001 = false;

            //grid_Destroy_bool = true;
            trigger1x_002 = true;

            DestroyPlayerIcon();
            Grid_Destroy();
            

        }

    }

    private void Grid_Create()
    {
        // Calculate the size of each boxPrefab
        Vector2 boxSize = boxPrefab.GetComponent<SpriteRenderer>().bounds.size;

        // Calculate the starting position for the grid
        startPos = new Vector2(-(X_area_length - 1) * spacingX / 2f, -(Y_area_length - 1) * spacingY / 2f);

        int boxCount = 0; // Counter for naming the boxes

        // Loop to instantiate boxes in a grid pattern for the outer boundary
        for (int y = 0; y < Y_area_length; y++)
        {
            for (int x = 0; x < X_area_length; x++)
            {
                // Check if the current position (x, y) is within the outer boundary
                if (x == 0 || x == X_area_length - 1 || y == 0 || y == Y_area_length - 1)
                {
                    // Calculate the position for each box
                    Vector3 position = new Vector3(
                        startPos.x + x * spacingX,
                        startPos.y + y * spacingY,
                        centerGameArea.transform.position.z
                    );

                    // Instantiate boxPrefab at the calculated position with no rotation
                    GameObject box = Instantiate(boxPrefab, position, Quaternion.identity);

                    // Generate a unique name for each box
                    if ((x == 0 || x == X_area_length - 1) && (y == 0 || y == Y_area_length - 1))
                    {
                        box.name = "box-" + boxCount + "-R"; // Corner-most boxes get "_R" suffix
                        CountTotalAmountBoxs = boxCount + 1;
                    }
                    else
                    {
                        box.name = "box-" + boxCount + "-I";
                        CountTotalAmountBoxs = boxCount + 1;
                    }

                    // Parent the instantiated box under centerGameArea
                    box.transform.parent = centerGameArea.transform;

                    // Increment the boxCount for the next box name
                    boxCount++;
                }
            }
        }
    }


    void Grid_Destroy() {
        // Clear existing boxes
        foreach (Transform child in centerGameArea.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void placePlayer(float playerScale)
    {
        if (PlayerlastPlace == 0)
        {
            // Ensure startPlace is within the valid range of instantiated boxes
            if (startPlace >= 0 && startPlace < CountTotalAmountBoxs)
            {
                // Retrieve the box GameObject based on startPlace
                Transform boxTransform = centerGameArea.transform.GetChild(startPlace);

                // Check if the boxTransform is valid (not null)
                if (boxTransform != null)
                {
                    // Get the position of the box
                    Vector3 boxPosition = boxTransform.position;

                    // Set the z-coordinate to -10
                    Vector3 playerPosition = new Vector3(boxPosition.x, boxPosition.y, -10);

                    // Instantiate playerIconPrefab at the adjusted position and store the instance
                    instantiatedPlayerIcon = Instantiate(playerIconPrefab, playerPosition, Quaternion.identity);

                    // Adjust the scale of the instantiated player icon
                    instantiatedPlayerIcon.transform.localScale = new Vector3(playerScale, playerScale, playerScale);

                    playerCreated = true;
                }
                else
                {
                    Debug.LogError("Box transform is null. Ensure startPlace is valid.");
                }
            }
            else
            {
                Debug.LogError("Invalid startPlace value. Check the range.");
            }
        }
        else
        {
            // Ensure startPlace is within the valid range of instantiated boxes
            if (startPlace >= 0 && startPlace < CountTotalAmountBoxs)
            {
                // Retrieve the box GameObject based on startPlace
                Transform boxTransform = centerGameArea.transform.GetChild(startPlace);

                // Check if the boxTransform is valid (not null)
                if (boxTransform != null)
                {
                    // Get the position of the box
                    Vector3 boxPosition = boxTransform.position;

                    // Set the z-coordinate to -10
                    Vector3 playerPosition = new Vector3(boxPosition.x, boxPosition.y, -10);

                    // Instantiate playerIconPrefab at the adjusted position and store the instance
                    instantiatedPlayerIcon = Instantiate(playerIconPrefab, playerPosition, Quaternion.identity);

                    // Adjust the scale of the instantiated player icon
                    instantiatedPlayerIcon.transform.localScale = new Vector3(playerScale, playerScale, playerScale);

                    playerCreated = true;
                }
                else
                {
                    Debug.LogError("Box transform is null. Ensure startPlace is valid.");
                }
            }
            else
            {
                Debug.LogError("Invalid startPlace value. Check the range.");
            }
        }
    }

    void DestroyPlayerIcon()
    {
        if (instantiatedPlayerIcon != null)
        {
            Debug.Log("Destroying player icon: " + instantiatedPlayerIcon.name);
            Destroy(instantiatedPlayerIcon);
            instantiatedPlayerIcon = null; // Clear the reference after destruction
        }
        else
        {
            Debug.LogWarning("No instantiated player icon to destroy.");
        }
    }


}