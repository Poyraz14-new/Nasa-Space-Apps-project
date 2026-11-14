using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovemenet : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        rb.linearVelocity = movement * movementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        bool isMoving = movement != Vector2.zero;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("MoveX", movement.x);
            animator.SetFloat("MoveY", movement.y);
        }
    }
}