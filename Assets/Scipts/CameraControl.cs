using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{

    [Header("Game Objects")]
    [SerializeField]
    private Transform target; // The target the camera will follow

    [SerializeField]
    private Transform verticalPivot; // The vertical camera pivot ('Pivot')

    [SerializeField]
    private Transform horizontalPivot; // The horizontal camera pivot ('HPivot')


    [Header("Camera Controls")]
    [SerializeField]
    private Vector3 offset; // Offset between the player and the camera

    [SerializeField]
    private float rotateSpeed; // How fast the camera rotates around the player

    [SerializeField]
    private float maxView; // The maximum a player can look upwards

    [SerializeField]
    private float minView; // The maximum a player can look downwards

    [SerializeField]
    private bool invertY; // Control setting


    [Header("Camera Switching")]
    [SerializeField]
    private Camera MainCamera; // Main 3d Camera Field

    [SerializeField]
    private Camera Camera2D; // Field for the 2d camera

    private Vector3 MainStored; // Variable to store the position of the 3d camera


    [Header("Layer Masks")]
    private int playerLayerMask;
    private int checkPointLayerMask;
    private int ignoreLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        // Sets the offset position
        offset = target.position - transform.position;

        // Sets the pivot locations
        verticalPivot.transform.position = target.transform.position;
        horizontalPivot.transform.position = target.transform.position;

        // Sets the horizontal pivot as the parent of the vertical pivot
        verticalPivot.transform.parent = horizontalPivot.transform;
        horizontalPivot.transform.parent = null;


        // Makes it so the cursor cannot be seen during gameplay
        Cursor.lockState = CursorLockMode.Locked;

        // Create layer masks
        playerLayerMask = 1 << target.gameObject.layer;
        checkPointLayerMask = 1 << LayerMask.NameToLayer("Checkpoints"); // Assuming "CheckpointLayer" is the name of your checkpoint layer
        int playerAndCheckpointLayerMask = playerLayerMask | checkPointLayerMask;
        ignoreLayerMask = ~playerAndCheckpointLayerMask;

    }


    // Called once per frame after Update()
    void LateUpdate()
    {
        // Button to switch camera view
        if (Input.GetKeyDown(KeyCode.G))
        {
            Show2DView();
        }

        // 3D camera control
        if (MainCamera.enabled == true)
        {
            // Sets the pivot to the same position as the player
            verticalPivot.transform.position = target.transform.position;

            // Controls the pivots rotations depending on how the player moves their mouse
            float horizontalCamera = Input.GetAxis("Mouse X") * rotateSpeed;
            horizontalPivot.Rotate(0, horizontalCamera, 0);

            float verticalCamera = Input.GetAxis("Mouse Y") * rotateSpeed;
            // Controls the Y invert (e.g if mouse goes up, camera goes down)
            if (invertY)
            {
                verticalPivot.Rotate(-verticalCamera, 0, 0);
            }
            else
            {
                verticalPivot.Rotate(verticalCamera, 0, 0);
            }


            // Controls how far up the player can look
            if (verticalPivot.rotation.eulerAngles.x > maxView && verticalPivot.rotation.eulerAngles.x < 180.0f)
            {
                verticalPivot.rotation = Quaternion.Euler(maxView, verticalPivot.eulerAngles.y, 0.0f);
            }


            // Controls how low down the player can look
            if (verticalPivot.rotation.eulerAngles.x > 180.0f && verticalPivot.rotation.eulerAngles.x < 360f + minView)
            {
                verticalPivot.rotation = Quaternion.Euler(360.0f + minView, verticalPivot.eulerAngles.y, 0.0f);
            }

            // Takes in the rotation of the pivots (vPivot inherits hPivots x)
            float yAngle = verticalPivot.eulerAngles.y;
            float xAngle = verticalPivot.eulerAngles.x;


            // Applies the pivot values
            Quaternion rotationValue = Quaternion.Euler(xAngle, yAngle, 0);
            transform.position = target.position - (rotationValue * offset);


            // Calculate desired camera position
            Vector3 desiredPosition = target.position - (rotationValue * offset);


            // Perform raycast to check for collision
            RaycastHit hit;

            if (Physics.Linecast(target.position, desiredPosition, out hit, ignoreLayerMask))
            {
                // If a collision occurs with anything except the player, set camera position to hit point
                transform.position = hit.point;
            }

            else
            {
                // If no collision, set camera position as desired position
                transform.position = desiredPosition;
            }


            // Prevents the camera from flipping
            if (transform.position.y < target.position.y)
            {
                transform.position = new Vector3(transform.position.x, target.position.y - 0.9f, transform.position.z);
            }


            // Makes the camera look at the player
            transform.LookAt(target);
        }
    }

    // Function to switch the camera to a 2d view
    public void Show2DView()
    {
        // If already in 2d then switch back to 3d and vice versa
        if (MainCamera.enabled == true)
        {
            MainStored = MainCamera.transform.position; // Stores the main cameras position
            MainCamera.transform.position = Camera2D.transform.position; // Stores the main camera at the 2d cameras location
            transform.LookAt(target); // Makes the camera look at the player
            MainCamera.enabled = false; 
            Camera2D.enabled = true; 
        }
        else
        {
            MainCamera.enabled = true; 
            Camera2D.enabled = false;
            MainCamera.transform.position = MainStored; // Repositions the camera to its previous position
        }
    }
}
