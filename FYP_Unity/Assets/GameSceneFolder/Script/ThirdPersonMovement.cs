using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{   [Header("Feature")]
    public bool Run = true;
    public bool Jump;
    


    [Header("Movement Settings")]
    public float walkSpeed = 5f; // Speed for walking
    public float runSpeed = 10f; // Speed for running
    public float turnSmoothTime = 0.1f; // Smooth rotation time

    [Header("References")]
    public Transform cameraTransform; // Reference to the camera's transform

    private CharacterController characterController;
    private float turnSmoothVelocity;

    [Header("Jump Settings")]
    public float jumpHeight = 10f; // Jump force
    public int maxJumps = 2; // Maximum number of jumps allowed (including the initial ground jump)
    public float gravity = -9.8f; // Gravity value
    public float moveSpeed = 5f; // Optional movement speed

    
    private Vector3 velocity; // Velocity for gravity and jumping
    private int currentJumpCount; // Tracks how many jumps have been performed
    private bool isGrounded; // Tracks whether the player is on the ground

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {   
        
        HandleMovement();
        if(Jump)
        {
            
            HandleJump();
        }
    }

    void HandleMovement()
    {
        // Get input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) // Only move if there's input
        {
            // Calculate the target angle (relative to the camera's forward direction)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smoothly rotate the player toward the target angle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Calculate the movement direction
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

           
          
            // Determine movement speed (walk or run)
            float speed = (Run && Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;
            


            // Apply movement
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
    }
        

        void HandleJump()
    {
        // Check if the player is grounded
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            // Reset velocity and jump count when grounded
            velocity.y = -2f; // Small value to keep the player grounded
            currentJumpCount = 0;
        }

        // Jump input
        if (Input.GetButtonDown("Jump") && currentJumpCount < maxJumps)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate jump velocity
            currentJumpCount++; // Increment jump count
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply velocity to the CharacterController
        characterController.Move(velocity * Time.deltaTime);
    }
    
    
    
   



}