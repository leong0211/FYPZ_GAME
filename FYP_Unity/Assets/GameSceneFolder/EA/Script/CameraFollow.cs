using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform followTarget; // The player this camera follows
    public Transform lookAtTarget; // The opposite player this camera looks at
    public Vector3 offset;         // Offset position relative to the followTarget

    public float rotationDamping = 5f; // Speed at which the camera rotates to sync with the followTarget
    public float rotationDelay = 0.5f; // Delay before the camera starts rotating (in seconds)

    private Quaternion currentTargetRotation; // The delayed target rotation
    private float rotationDelayTimer = 0f;    // Timer to track the rotation delay

    void LateUpdate()
    {
        // Update the camera's position relative to the followTarget
        FollowPlayer();

        // Make the camera smoothly rotate to look at the lookAtTarget with delay
        LookAtTargetWithDelay();
    }

    private void FollowPlayer()
    {
        // Calculate the desired position based on the followTarget's position and offset
        Vector3 desiredPosition = followTarget.position + followTarget.TransformDirection(offset);

        // Update the camera's position instantly or smoothly (you can interpolate if needed)
        transform.position = desiredPosition;
    }

    private void LookAtTargetWithDelay()
    {
        if (lookAtTarget != null)
        {
            // Calculate the direction to look at the lookAtTarget
            Vector3 direction = lookAtTarget.position - transform.position;

            // Calculate the desired rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Check if the lookAtTarget has moved
            if (Vector3.Distance(transform.position, lookAtTarget.position) > 0.01f)
            {
                // Increment delay timer when target moves
                rotationDelayTimer += Time.deltaTime;

                if (rotationDelayTimer >= rotationDelay)
                {
                    // Update the delayed target rotation after the delay
                    currentTargetRotation = targetRotation;
                    rotationDelayTimer = 0f; // Reset the timer
                }
            }

            // Smoothly rotate the camera toward the delayed target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetRotation, Time.deltaTime * rotationDamping);
        }
    }
}