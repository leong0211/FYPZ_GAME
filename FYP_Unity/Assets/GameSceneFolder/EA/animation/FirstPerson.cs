using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPerson : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f; // Crouch movement speed

    [Header("Animator Settings")]
    public Animator animator; // Reference to Animator

    [Header("Attack Settings")]
    public float attackForwardDistance = 3f; // Distance to move forward during attack
    public float attackDuration = 0.5f;      // Duration of attack animation

    private CharacterController characterController;
    private bool isAttacking = false;        // Whether currently attacking
    private bool isCrouching = false;        // Whether currently crouching
    private Vector3 originalPosition;        // Original position before attack

    // Height and center position
    private float originalHeight;            // Initial height
    private Vector3 originalCenter;          // Initial center

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Record initial height and center
        originalHeight = characterController.height;
        originalCenter = characterController.center;

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

    void Update()
    {
        // Skip other processing if attacking
        if (isAttacking)
        {
            return;
        }

        // Lock the character's rotation (only allow rotation along the Y-axis)
        Vector3 lockedRotation = transform.rotation.eulerAngles;
        lockedRotation.x = 0f;
        lockedRotation.z = 0f;
        transform.rotation = Quaternion.Euler(lockedRotation);

        HandleMovement();

        // Trigger attack types
        if (Input.GetKeyDown(KeyCode.K)) // Kick
        {
            HandleAttack("Kick");
        }
        else if (Input.GetKeyDown(KeyCode.J)) // Punch
        {
            HandleAttack("Punch");
        }
        else if (Input.GetKeyDown(KeyCode.L)) // Combo Punch
        {
            HandleAttack("ComboPunch");
        }
        else if (Input.GetKeyDown(KeyCode.M)) // Magic
        {
            HandleAttack("Magic");
        }

        // Crouch action
        if (Input.GetKeyDown(KeyCode.C)) // C key triggers crouch
        {
            ToggleCrouch();
        }
    }

    void HandleMovement()
    {
        float speed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);

        float horizontal = Input.GetAxis("Horizontal");
        Vector3 move = transform.right * horizontal;

        characterController.Move(move * speed * Time.deltaTime);

        // Update Animator parameters
        if (animator != null)
        {
            animator.SetBool("Right", horizontal > 0);
            animator.SetBool("Left", horizontal < 0);
        }
    }

    void HandleAttack(string attackType)
    {
        isAttacking = true;

        // Magic does not require movement
        if (attackType == "Magic")
        {
            // Play Magic attack animation
            if (animator != null)
            {
                animator.SetTrigger("Magic");
            }

            // Wait for animation to complete before ending attack state
            StartCoroutine(EndAttackAfterDelay());
            return;
        }

        // Other attacks require movement
        originalPosition = transform.position;
        Vector3 attackPosition = originalPosition + transform.forward * attackForwardDistance;

        // Move to attack position
        characterController.enabled = false; // Temporarily disable CharacterController
        transform.position = attackPosition; // Move to attack position
        characterController.enabled = true;  // Enable CharacterController

        // Play corresponding attack animation
        if (animator != null)
        {
            animator.SetTrigger(attackType); // Assuming Animator has "Kick", "Punch", "ComboPunch", "Magic" triggers
        }

        // Wait for animation to finish before returning to original position
        StartCoroutine(ReturnToOriginalPosition());
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        // Save current character rotation
        Quaternion currentRotation = transform.rotation;

        // Update Animator's crouch parameter
        if (animator != null)
        {
            animator.SetBool("Crouch", isCrouching);
        }

        // Adjust CharacterController's height and center
        if (isCrouching)
        {
            characterController.height = originalHeight / 2f; // Reduce height by half
            characterController.center = new Vector3(originalCenter.x, originalCenter.y / 2f, originalCenter.z); // Lower center point
        }
        else
        {
            characterController.height = originalHeight; // Restore original height
            characterController.center = originalCenter; // Restore original center
        }

        // Restore character rotation
        transform.rotation = currentRotation;
    }

    System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        // Wait for attack animation duration
        yield return new WaitForSeconds(attackDuration);

        // Move back to original position
        characterController.enabled = false; // Temporarily disable CharacterController
        transform.position = originalPosition; // Move back to original position
        characterController.enabled = true;  // Enable CharacterController

        isAttacking = false;
    }

    System.Collections.IEnumerator EndAttackAfterDelay()
    {
        // Wait for Magic action animation time before ending attack state
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }
}