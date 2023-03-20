using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveControls : MonoBehaviour
{
    public float speed;
    public float jumpForce;

    private GatherInput gI;
    private Rigidbody2D rb;
    private Animator anim;

    private int direction = 1;

    public PlayerInteractableSystem _PlayerInteractableSystem;
    
    public float rayLength;
    public LayerMask groundLayer;
    public Transform leftPoint;
    public Transform rightPoint;
    public bool isGrounded = true;
    public bool hasControl = true;
    private bool knockBack = false;
    // Start is called before the first frame update
    private void Start()
    {
        gI = GetComponent<GatherInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        SetAnimatorValues();
        
        HandleInteractables();
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        if(knockBack || !hasControl)
        {
            return;
        }
        Move();
        JumpPlayer();
    }

    private void Move()
    {
        Flip();
        rb.velocity = new Vector2(speed * gI.valueX, rb.velocity.y);
    }

    private void HandleInteractables()
    {
        if (_PlayerInteractableSystem == null)
        {
            return;
        }
        
        if (!gI.ShouldInteract)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        _PlayerInteractableSystem.Interact(transform.position, mousePosition);
    }
    
    private void JumpPlayer()
    {
        return;
        /*
        if(gI.jumpInput)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(gI.valueX * speed, jumpForce);

            }

        }
        gI.jumpInput = false;
        */
    }

    private void CheckGroundStatus()
    {
        RaycastHit2D rightCheckHit = Physics2D.Raycast(rightPoint.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D leftCheckHit = Physics2D.Raycast(leftPoint.position, Vector2.down, rayLength, groundLayer);
        if (leftCheckHit || rightCheckHit)
        {
            isGrounded = true;

        }
        else
        {
            isGrounded = false;
        }
        SeeRays(leftCheckHit, rightCheckHit);
    }

    private void SeeRays(RaycastHit2D leftCheckHit, RaycastHit2D rightCheckHit)
    {
        Color color1 = leftCheckHit ? Color.red : Color.green;
        Color color2 = rightCheckHit ? Color.red : Color.green;

        Debug.DrawRay(leftPoint.position, Vector2.down * rayLength, color1);
        Debug.DrawRay(rightPoint.position, Vector2.down * rayLength, color2);
    }

    private void Flip()
    {
        if (gI.valueX * direction < 0)
        {
            // Multiply values by -1 to flip them
            transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
            direction *= -1;
        }
    }

    private void SetAnimatorValues()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vSpeed", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    public IEnumerator KnockBack(float forceX, float forceY, float duration, Transform otherObject)
    {
        int knockBackDirection;
        if (transform.position.x < otherObject.position.x)
        {
            knockBackDirection = -1;
        }
        else
        {
            knockBackDirection = 1;
        }

        knockBack = true;
        rb.velocity = Vector2.zero;
        Vector2 theForce = new Vector2(knockBackDirection * forceX, forceY);
        rb.AddForce(theForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        knockBack = false;
        rb.velocity = Vector2.zero;
    }    
}
