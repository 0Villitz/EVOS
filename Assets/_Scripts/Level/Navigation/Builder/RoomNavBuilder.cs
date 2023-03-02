using System.Collections;
using System.Collections.Generic;
using Game2D;
using UnityEngine;

namespace Game.Room.Builder
{
    public class RoomNavBuilder : MonoBehaviour
    {
        private const string LayerMaskName = "RoomNavBuilder";
        private const string PrefabPath = "Resources/Prefabs/Rooms/";
        
        public Camera buildCamera;

        public NavigationController navigationController;

        public List<UnitMovement> nodeActions { get; private set; } = new List<UnitMovement>()
        {
            UnitMovement.Idle,
            UnitMovement.MoveRight,
            UnitMovement.MoveLeft,
            UnitMovement.Jump,
            UnitMovement.Falling,
            UnitMovement.Crawl,
            UnitMovement.Climb
        };

        public UnitMovement nodeAction { get; private set; } = UnitMovement.Idle;
        
        public bool building { get; private set; } = false;
        public int currentId { get; private set; } = 0;
        
        public void SetBuildingFlag(bool startBuilding)
        {
            building = startBuilding;
        }

        public void SavePrefab()
        {
            building = false;
            nodeAction = UnitMovement.Idle;
        }

        public void Discard()
        {
            nodeAction = UnitMovement.Idle;
            currentId = 0;
            navigationController.Discard();
            building = false;
        }

        public void AddNodeAction(UnitMovement action)
        {
            nodeAction |= action;
        }

        public void RemoveAction(UnitMovement action)
        {
            nodeAction &= (~action);
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