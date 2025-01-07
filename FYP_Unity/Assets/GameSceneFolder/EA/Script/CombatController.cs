using UnityEngine;

public class CombatController : MonoBehaviour
{
    private Animator animator; // Reference to the Animator component

    // Customizable key bindings for each animation
    public KeyCode punch;       
    public KeyCode ComboPunch;       
    public KeyCode jump;      
    public KeyCode Crouch ;         
    public KeyCode Kick;         
    public KeyCode Magic;         
    public KeyCode hurt;
    public KeyCode Death;
    

    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();

        // Check if Animator is assigned
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject!");
        }
    }

    void Update()
    {
        // Check for key presses and play the corresponding animations
        if (Input.GetKeyDown(punch))
        {
            PlayAnimation("punch");
        }

        if (Input.GetKeyDown(ComboPunch))
        {
            PlayAnimation("Combo punch");
        }

        if (Input.GetKeyDown(jump))
        {
            PlayAnimation("jump");
        }

        if (Input.GetKeyDown(Crouch))
        {
            PlayAnimation("Crouch");
        }

        if (Input.GetKeyDown(Kick))
        {
            PlayAnimation("Kick");
        }

        if (Input.GetKeyDown(Magic))
        {
            PlayAnimation("Magic");
        }

        if (Input.GetKeyDown(hurt))
        {
            PlayAnimation("hurt");
        }
        
        if (Input.GetKeyDown(Death))
        {
            PlayAnimation("Death");
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            // Play the animation by triggering its name
            animator.Play(animationName);
        }
    }
}