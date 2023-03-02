
using System.Collections.Generic;
using Game2D;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Room.Builder
{
    public class RoomNavBuilder : MonoBehaviour
    {
        private const string LayerMaskName = "RoomNavBuilder";
        private const string PrefabPath = "Resources/Prefabs/Rooms/";
        
        public Camera buildCamera;

        public NavigationController navigationController;

        public int nodeActionIndex { get; set; } = 0;
        public HashSet<UnitMovement> cachedNodeActions { get; private set; } = new HashSet<UnitMovement>();
        
        public bool building { get; private set; } = false;
        public int currentId { get; private set; } = 0;
        
        public void SetBuildingFlag(bool startBuilding)
        {
            building = startBuilding;
        }

        public void SavePrefab()
        {
            building = false;
            cachedNodeActions = new HashSet<UnitMovement>(); 
            cachedNodeActions.Add(UnitMovement.Idle);
            nodeActionIndex = 0;
        }

        public void Discard()
        {
            nodeActionIndex = 0;
            cachedNodeActions = new HashSet<UnitMovement>(); 
            cachedNodeActions.Add(UnitMovement.Idle);
            currentId = 0;
            navigationController.Discard();
            building = false;
        }

        public void AddNodeAction(UnitMovement nodeAction)
        {
            cachedNodeActions.Add(nodeAction);
        }

        public void RemoveAction(UnitMovement nodeAction)
        {
            cachedNodeActions.Remove(nodeAction);
        }
        
        public void ConnectNodes(List<Object> possibleNodes)
        {
            
        }
        
        #region Monobehavior
        void Update()
        {
            if (!building)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = buildCamera.ScreenPointToRay(Input.mousePosition);
                LayerMask puzzleMapLayer = 1 << LayerMask.NameToLayer(LayerMaskName);
                if (Physics.Raycast(
                        ray,
                        out RaycastHit hit,
                        float.MaxValue,
                        puzzleMapLayer
                    ))
                {
                    navigationController.AddNode(hit.point, currentId);
                    currentId++;
                }
            }
        }
        #endregion
    }
}