
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInteractableSystem", menuName = "EVOS/PlayerInteractableSystem")]
public class PlayerInteractableSystem : ScriptableObject
{
    private const int       MaxHitsColliderCacheCount = 50;
    
    public        LayerMask _InteractableLayer;
    
    private Camera         _camera;
    private Collider2D[]   _colliderCache   = new Collider2D[MaxHitsColliderCacheCount];

    public bool Interact(Vector3 playerPosition, Vector2 mousePosition, out IPlayerRespawn spawnPoint)
    {
        spawnPoint = null;
        
        Vector3 worldMousePos = CameraUtils.GetMouseWorldPoint(Camera, mousePosition, 1.0f);
        int colliderCount = Physics2D.OverlapPointNonAlloc(worldMousePos, _colliderCache, _InteractableLayer);

        bool didInteract = false;
        
        for(int i = 0; i < colliderCount; ++i)
        {
            Collider2D collider = _colliderCache[i];
            
            var interactable = collider.GetComponentInChildren<IPlayerInteractable>();
            if (interactable == null)
            {
                continue;            
            }

            Vector3 closestPoint = collider.ClosestPoint(playerPosition);
            Vector2 closesDelta  = playerPosition - closestPoint;
            
            
            // Check to make sure the player is close enough to interact with
            // the interactable object
            bool canInteractWith = interactable.InteractCutoffDistance > 0.0f &&
                                   closesDelta.magnitude < interactable.InteractCutoffDistance;

            Color lineColor = canInteractWith ? Color.green : Color.red;
            
            Debug.Log($"Player/Interactable distance:{closesDelta.magnitude}");
            Debug.DrawLine(playerPosition, closestPoint, lineColor, 2.0f);
            
            if (canInteractWith)
            {
                interactable.Interact();
                if (interactable is IPlayerRespawn respawnObject)
                {
                    spawnPoint = respawnObject;
                }
                
                didInteract = true;
                Debug.Log($"Player Interacted with: {interactable}");
            }
        }

        return didInteract;
    }

    private Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }
}

