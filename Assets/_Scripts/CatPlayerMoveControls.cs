using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatPlayerMoveControls : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public int additionalJumps = 2;
    private int resetJumpsNumber;

    private GatherInput gI;
    private Rigidbody2D rb;
    private Animator anim;

    private int direction = 1;

    public float rayLength;
    public LayerMask groundLayer;
    public Transform leftPoint;
    public Transform rightPoint;
    private bool grounded = true;
    private bool doubleJump = true;
    public bool hasControl = true;

    private bool knockBack = false;
    // Start is called before the first frame update
    private void Start()
    {
        gI = GetComponent<GatherInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        resetJumpsNumber = additionalJumps;
    }

    private void Update()
    {
        SetAnimatorValues();
    }

    private void FixedUpdate()
    {
        CheckStatus();
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

    private void JumpPlayer()
    {
        if(gI.jumpInput)
        {
            if (grounded)
            {
                rb.velocity = new Vector2(gI.valueX * speed, jumpForce);
                doubleJump = true;
            }
            else if (additionalJumps > 0)
            {
                rb.velocity = new Vector2(gI.valueX * speed, jumpForce);
                doubleJump = false;
                additionalJumps--;
            }
        }
        gI.jumpInput = false;
    }

    private void CheckStatus()
    {
        RaycastHit2D rightCheckHit = Physics2D.Raycast(rightPoint.position, Vector2.down, rayLength, groundLayer);
        RaycastHit2D leftCheckHit = Physics2D.Raycast(leftPoint.position, Vector2.down, rayLength, groundLayer);
        if (leftCheckHit || rightCheckHit)
        {
            grounded = true;
            doubleJump = false;
            additionalJumps = resetJumpsNumber;
        }
        else
        {
            grounded = false;
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
        anim.SetBool("Grounded", grounded);
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
