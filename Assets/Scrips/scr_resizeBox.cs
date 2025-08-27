using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scr_resizeBox : MonoBehaviour
{
    [SerializeField] private GameObject SquareBox;
    [SerializeField] private Scr_createArea createArea;
    [SerializeField] private scr_playerMove playerMove;
    //[SerializeField] private scr_ScriptableObject_prizeBox prizeBox; // Removed serialized field

    [SerializeField] public bool gotHit = false;

    [SerializeField] public int id = 0;

    [Tooltip("0:NOT Edge ; 1:EDGE"), SerializeField] public int EdgeRotate = 0;
    [Tooltip("0: straight, 1:BottomLeft, 2:TopLeft 3:TopRight 4:BottomRight"), SerializeField] public int start_bends = 0;

    [SerializeField] private float scaleMultiplier = 1f; // Use this to adjust the inverse scaling effect

    private void Awake()
    {
        // Check if createArea is assigned via the Inspector
        if (createArea == null)
        {
            // Get the Camera.main object
            Camera mainCamera = Camera.main;

            if (mainCamera != null)
            {
                // Get the component from the child GameObject within the main camera
                createArea = mainCamera.GetComponent<Scr_createArea>();

                if (createArea == null)
                {
                    Debug.LogWarning("Scr_createArea component not found in the children of the main camera.");
                }
            }
            else
            {
                Debug.LogWarning("Main camera not found.");
            }
        }
    }

    private void Start()
    {
        // Parse the name to set id and EdgeRotate
        if (SquareBox != null)
        {
            ParseName(SquareBox.name);
            DetermineStartBends();
            //GetPrizeCodeByStartBends(start_bends); // Commented out as it's unused
        }
        else
        {
            Debug.LogWarning("SquareBox is not assigned.");
        }
    }

    private void Update()
    {
        // Adjust scale in real-time if the camera zoom changes
        AdjustScaleToCamera();
    }

    private void ParseName(string name)
    {
        // Debug log the original name
        Debug.Log($"Original name: {name}");

        // Expecting name format "Box-<id>-<EdgeIndicator>"
        string[] parts = name.Split('-');

        // Debug log the parts array
        Debug.Log($"Parts after split: [{string.Join(", ", parts)}]");

        if (parts.Length == 3)
        {
            if (int.TryParse(parts[1], out int parsedId))
            {
                id = parsedId;
                Debug.Log($"Parsed id successfully: {id}");
            }
            else
            {
                Debug.LogWarning($"Failed to parse id from name: {name}");
            }

            switch (parts[2])
            {
                case "R":
                    EdgeRotate = 1; // Assuming R means it's an edge
                    Debug.Log($"EdgeRotate set to 1 (Edge indicator 'R')");
                    break;
                case "I":
                    EdgeRotate = 0; // Assuming I means it's not an edge
                    Debug.Log($"EdgeRotate set to 0 (Edge indicator 'I')");
                    break;
                default:
                    Debug.LogWarning($"Unknown edge indicator: {parts[2]} in name: {name}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Name format incorrect: {name}. Expected format: 'Box-<id>-<EdgeIndicator>'");
        }
    }

    private void AdjustScaleToCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null && SquareBox != null)
        {
            float inverseScaleFactor = 1f;

            if (mainCamera.orthographic)
            {
                // For orthographic camera
                inverseScaleFactor = 1 / mainCamera.orthographicSize;
            }
            else
            {
                // For perspective camera
                inverseScaleFactor = 1 / Mathf.Tan(Mathf.Deg2Rad * mainCamera.fieldOfView * 0.5f);
            }

            SquareBox.transform.localScale = Vector3.one * scaleMultiplier * inverseScaleFactor;


            // Set the Z position to -5
            Vector3 position = SquareBox.transform.position;
            position.z = -5;
            SquareBox.transform.position = position;
        }
        else
        {
            Debug.LogWarning("Main camera or SquareBox is not assigned.");
        }
    }

    private void DetermineStartBends()
    {
        // Assuming SquareBox is placed relative to the screen space
        Vector3 boxPosition = SquareBox.transform.position;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(boxPosition);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        if (EdgeRotate == 1)
        {
            if (screenPosition.x <= screenWidth / 2 && screenPosition.y <= screenHeight / 2)
            {
                start_bends = 1; // BottomLeft
            }
            else if (screenPosition.x <= screenWidth / 2 && screenPosition.y > screenHeight / 2)
            {
                start_bends = 2; // TopLeft
            }
            else if (screenPosition.x > screenWidth / 2 && screenPosition.y > screenHeight / 2)
            {
                start_bends = 3; // TopRight
            }
            else
            {
                start_bends = 4; // BottomRight
            }
        }
        else
        {
            start_bends = 0; // Default to straight
        }

        Debug.Log($"Start Bends: {start_bends}");
    }


}