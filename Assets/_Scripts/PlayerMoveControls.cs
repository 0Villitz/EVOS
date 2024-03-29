
using System;
using System.Collections;
using Game2D;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerMoveControls : MonoBehaviour, Game2D.IPlayerCharacter
{
    public float speed;
    public float jumpForce;

    private GatherInput gI;
    private Rigidbody2D rb;
    private Animator anim;

    private int direction = 1;

    public ScriptableEventDispatcher _GameEventDispatcher;
    public PlayerInteractableSystem _PlayerInteractableSystem;
    
    [SerializeField] private int _health = 100;
    private int _currentHealth;
    
    public float rayLength;
    public LayerMask groundLayer;
    public Transform leftPoint;
    public Transform rightPoint;
    public bool isGrounded = true;
    public bool hasControl = true;
    private bool knockBack = false;

    private BlackoutController _blackoutController;
    private GatherInput _inputController;

    [SerializeField]
    private AnimationEventHelper _animationEventHelper;
    
    // Start is called before the first frame update
    private void Start()
    {
        gI = GetComponent<GatherInput>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        _blackoutController = FindObjectOfType<BlackoutController>();
        _inputController = FindObjectOfType<GatherInput>();
        
        _currentHealth = _health;

        if (_animationEventHelper != null)
        {
            _animationEventHelper.AddEvent(OnPlayerDeathEvent);
            _animationEventHelper.AddEvent(OnPlayerRespawnEvent);
        }
    }

    private void OnDestroy()
    {
        if (_animationEventHelper != null)
        {
            _animationEventHelper.RemoveEvent(OnPlayerDeathEvent);
            _animationEventHelper.RemoveEvent(OnPlayerRespawnEvent);
        }
    }

    private void OnPlayerDeathEvent(AnimationEvent eventData)
    {
        string eventKey = eventData.stringParameter;
        switch (eventKey)
        {
            case AnimationEventKey.PlayerDeathStart:
                _inputController.CurrentControlType = GatherInput.ControlType.None;
                break;
            case AnimationEventKey.PlayerDeathEnd:
                StartCoroutine(PlayerRespawn());
                break;
        }
    }

    private void OnPlayerRespawnEvent(AnimationEvent eventData)
    {
        if (eventData.stringParameter == AnimationEventKey.PlayerSpawnEnd)
        {
            _inputController.CurrentControlType = GatherInput.ControlType.Player;
        }
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
        
        if (!gI.ShouldInteract && !gI.ShouldInteractLocker)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (_PlayerInteractableSystem.Interact(transform.position, mousePosition, out IPlayerRespawn spawnPoint)
            && spawnPoint != null
           )
        {
            _spawnPoint = spawnPoint;
        }
    }

    private IPlayerRespawn _spawnPoint = null;
    
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

    #region Game2D.IInteractableObject

    void Game2D.IInteractableObject.Interact(Game2D.ICharacterController characterController)
    {
        
    }
    
    #endregion
    
    #region Game2D.IPlayerCharacter

    Transform Game2D.IPlayerCharacter.GetTransform()
    {
        return this.transform;
    }

    public bool CanBeDetected()
    {
        return !_isHiding
               && _currentHealth > 0;
    }

    [SerializeField] private bool _animateRespawn = false;
    void Game2D.IPlayerCharacter.TakeDamage(Game2D.IAttackerObject attackingObject)
    {
        _currentHealth -= attackingObject.Damage;
        // attackingObject.ProcessAttack();

        if (_currentHealth <= 0)
        {
            if (_spawnPoint != null)
            {
                _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
                
                if (_animateRespawn)
                {
                    anim.SetTrigger("Death");
                    // code to trigger death animation
                }
                else
                {
                    StartCoroutine(PlayerRespawn());
                }
            }
            else
            {
                Debug.LogError("Cannot respawn player, missing respawn object");
            }
        }
        else
        {
            // TODO: Play hit animation?
        }
    }

    [SerializeField] private float _respawnDelay = 2f;
    [SerializeField] private float _respawnFadeSpeed = 0.2f;

    private IEnumerator PlayerRespawn()
    {
        yield return PlayerRespawnStart();
        yield return PlayerRespawnWait();
        yield return PlayerRespawnEnd();
    }

    private IEnumerator PlayerRespawnStart()
    {
        _inputController.CurrentControlType = GatherInput.ControlType.None;
        yield return _blackoutController.BlackoutScreen(_respawnFadeSpeed);
    } 

    private IEnumerator PlayerRespawnWait()
    {
        _blackoutController.Label.SetText("Respawning...");
        yield return new WaitForSeconds(_respawnDelay);
    }
    
    private IEnumerator PlayerRespawnEnd() {
        this.transform.position = _spawnPoint.GetTransform().position;
        _currentHealth = _health;

        yield return _blackoutController.BlackoutScreen(-_respawnFadeSpeed);
        _blackoutController.Label.SetText(String.Empty);
        
        if (_animateRespawn)
        {
            anim.SetTrigger("Respawn");
        }
        else
        {
            _inputController.CurrentControlType = GatherInput.ControlType.Player;
        }
    }
    
    public int GetHealth()
    {
        return _currentHealth;
    }

    #endregion

    #region Locker

    private bool _isHiding = false;
    public bool IsHiding => _isHiding;

    public void ExitLocker()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<CapsuleCollider2D>().enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        _isHiding = false;
    }
    
    public void EnterLocker(Vector3 lockerPosition)
    {
        transform.position = lockerPosition;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;
        _isHiding = true;
    }

    #endregion
}
