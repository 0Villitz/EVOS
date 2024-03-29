using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IPlayerInteractable
{
    private float interactCutoffDistance = 2f;
    public float InteractCutoffDistance => interactCutoffDistance;
    private GameObject player;

    private GatherInput inputController;
    private bool isPlayerInLocker = false;

    private bool isBusy = false;

    public void Interact()
    {
        if (_playerMoveControls == null)
        {
            _playerMoveControls = player.GetComponent<PlayerMoveControls>();
        }
        StartCoroutine(LockerInteractCo());
    }

    private PlayerMoveControls _playerMoveControls = null;
    private IEnumerator LockerInteractCo()
    {
        if(isPlayerInLocker && !isBusy)
        {
            isBusy = true;
            GetComponent<Animator>().Play("Locker_Open");
            yield return new WaitForSeconds(1f);
            // player.GetComponent<SpriteRenderer>().enabled = true;
            // player.GetComponent<CapsuleCollider2D>().enabled = true;
            // player.GetComponent<Rigidbody2D>().simulated = true;
            _playerMoveControls.ExitLocker();
            inputController.CurrentControlType = GatherInput.ControlType.Player;
            // isBusy = false;
            isPlayerInLocker = _playerMoveControls.IsHiding;
            isBusy = false;
            GetComponent<Animator>().Play("Locker_Closed");
        }
        else if (!isBusy)
        {
            isBusy = true;
            inputController.CurrentControlType = GatherInput.ControlType.Locker;
            GetComponent<Animator>().Play("Locker_Open");
            yield return new WaitForSeconds(1f);
            // player.transform.position = this.transform.position;
            // player.GetComponent<SpriteRenderer>().enabled = false;
            // player.GetComponent<CapsuleCollider2D>().enabled = false;
            // player.GetComponent<Rigidbody2D>().simulated = false;
            _playerMoveControls.EnterLocker(this.transform.position);
            GetComponent<Animator>().Play("Locker_Closed");
            isPlayerInLocker = _playerMoveControls.IsHiding;
            isBusy = false;
        }  
    }

    // Start is called before the first frame update
    void Start()
    {
        inputController = FindObjectOfType<GatherInput>();

        if (player == null)
        {
            player = inputController.gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInLocker && Input.GetKeyDown(KeyCode.Space) && !isBusy)
        {
            StartCoroutine(LockerInteractCo());
        }
    }


}
