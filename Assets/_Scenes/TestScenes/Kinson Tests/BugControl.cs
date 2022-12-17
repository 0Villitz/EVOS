using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BugControl : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float movementSpeed = 200.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = new Vector2(0.0f, movementSpeed);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            velocity = new Vector2(0.0f, movementSpeed);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);

        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            velocity = new Vector2(0.0f, -movementSpeed);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            velocity = new Vector2(-movementSpeed, 0.0f);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            velocity = new Vector2(movementSpeed, 0.0f);
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }

    }

    private void Move(Vector3 direction)
    {
        Vector3 destination = transform.position + direction;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
        {
            Death();
        }
    }

    public void Death()
    {

    }

    public void MoveBug(InputAction.CallbackContext mousePosition)
    {
        transform.position = mousePosition.ReadValue<Vector2>();
    }
}
