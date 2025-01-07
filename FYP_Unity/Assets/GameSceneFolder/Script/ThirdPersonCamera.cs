using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player; // Reference to the player
    public Vector3 cameraOffset = new Vector3(0f, 2f, -4f); // Offset for the camera's position
    public float smoothSpeed = 10f; // Speed of the camera's smooth movement
    public float sensitivityX = 150f; // Horizontal look sensitivity
    public float sensitivityY = 100f; // Vertical look sensitivity
    public float minY = -30f; // Minimum vertical angle
    public float maxY = 60f; // Maximum vertical angle

    [Header("Collision Settings")]
    public LayerMask collisionLayers; // Layers to detect for camera collision
    public float collisionRadius = 0.2f; // Radius of the camera collision check
    public float collisionSmooth = 10f; // Smoothness for collision adjustment

    private float yaw = 0f; // Horizontal rotation (left/right)
    private float pitch = 0f; // Vertical rotation (up/down)



    void Start()
    {
        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        HandleCameraPosition();
    }

    void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        // Adjust yaw (horizontal) and pitch (vertical) based on input
        yaw += mouseX;
        pitch -= mouseY;

        // Clamp the vertical rotation to prevent flipping
        pitch = Mathf.Clamp(pitch, minY, maxY);
    }

    void HandleCameraPosition()
    {
        // Calculate the target rotation and position
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = player.position + targetRotation * cameraOffset;

        // Handle camera collision
        Vector3 adjustedPosition = HandleCameraCollision(player.position, desiredPosition);

        // Smoothly move the camera to the adjusted position
        transform.position = Vector3.Lerp(transform.position, adjustedPosition, smoothSpeed * Time.deltaTime);

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up * 1.5f); // Focus slightly above the player's center
    }

    Vector3 HandleCameraCollision(Vector3 playerPosition, Vector3 desiredPosition)
    {
        RaycastHit hit;
        Vector3 direction = desiredPosition - playerPosition;
        float distance = direction.magnitude;

        // Check for collisions in the direction of the desired position
        if (Physics.SphereCast(playerPosition, collisionRadius, direction, out hit, distance, collisionLayers))
        {
            // If there's a collision, position the camera at the collision point
            return hit.point + hit.normal * collisionRadius;
        }

        // If no collision, return the desired position
        return desiredPosition;
    }
}