using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    public Transform otherPlayer; // Reference to the other player
    public KeyCode moveLeftKey;   // Key for left movement
    public KeyCode moveRightKey;  // Key for right movement
    public float speed = 5f;      // Movement speed
    public float fixedDistance = 10f; // Fixed distance between players

    private bool isLeftPressed;   // Whether left key is pressed
    private bool isRightPressed;  // Whether right key is pressed

    private Quaternion currentTargetRotation; // Store the target rotation
    private float rotationDelayTimer = 0f;    // Timer for rotation delay
    public float rotationDelay = 0.5f;        // Delay before starting to rotate (in seconds)

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found!");
            }
        }

        // Disable Root Motion
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        // Check for key inputs
        isLeftPressed = Input.GetKey(moveLeftKey);
        isRightPressed = Input.GetKey(moveRightKey);

        if (animator != null)
        {
            animator.SetBool("Right", isRightPressed);
            animator.SetBool("Left", isLeftPressed);
        }

        // Handle the movement logic
        Move();
        MaintainDistance();
        FaceOtherPlayer();
    }

    private void Move()
    {
        // Get the other player's movement state
        PlayerMovement otherPlayerMovement = otherPlayer.GetComponent<PlayerMovement>();
        bool otherLeft = otherPlayerMovement.isLeftPressed;
        bool otherRight = otherPlayerMovement.isRightPressed;

        // Rule 1: Both players not moving
        if (!isLeftPressed && !isRightPressed && !otherLeft && !otherRight)
        {
            return;
        }

        // Rule 2 & 3: Both players move left or right
        if ((isLeftPressed && otherLeft) || (isRightPressed && otherRight))
        {
            // Circular motion around the center of the line
            float angle = (isLeftPressed ? 1 : -1) * speed * 6 * Time.deltaTime;
            Vector3 center = (transform.position + otherPlayer.position) / 2;
            Vector3 direction = transform.position - center;
            direction = Quaternion.Euler(0, angle, 0) * direction;
            transform.position = center + direction;
        }
        // Rule 4 & 5: Opposite directions -> Perpendicular motion
        else if ((isLeftPressed && otherRight) || (isRightPressed && otherLeft))
        {
            Vector3 perpendicular = Vector3.Cross((otherPlayer.position - transform.position).normalized, Vector3.up);
            float direction = isLeftPressed ? 1 : -1;
            transform.position += perpendicular * direction * speed / 2 * Time.deltaTime;
        }
        // Rule 6, 7, 8, 9: One player moves, the other stays stationary
        else if (isLeftPressed || isRightPressed)
        {
            float angle = (isLeftPressed ? 1 : -1) * speed * 4 * Time.deltaTime;
            Vector3 direction = transform.position - otherPlayer.position;
            direction = Quaternion.Euler(0, angle, 0) * direction;
            transform.position = otherPlayer.position + direction;
        }
    }

    private void MaintainDistance()
    {
        // Ensure the players stay 10 units apart
        Vector3 direction = (otherPlayer.position - transform.position).normalized;
        transform.position = otherPlayer.position - direction * fixedDistance;
    }

    private void FaceOtherPlayer()
    {
        // Calculate the direction to the other player
        Vector3 directionToOther = otherPlayer.position - transform.position;
        directionToOther.y = 0; // Ensure rotation stays on the horizontal plane

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToOther);

        // Check if the other player has moved
        if (Vector3.Distance(transform.position, otherPlayer.position) > 0.01f)
        {
            // Start or reset the timer if the opposite player moves
            rotationDelayTimer += Time.deltaTime;
            if (rotationDelayTimer >= rotationDelay)
            {
                // Update the target rotation only after the delay
                currentTargetRotation = targetRotation;
                rotationDelayTimer = 0f; // Reset the timer
            }
        }

        // Smoothly rotate towards the delayed target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, currentTargetRotation, Time.deltaTime * 2f);
    }
}