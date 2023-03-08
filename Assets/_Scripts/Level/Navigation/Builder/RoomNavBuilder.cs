
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Game2D;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Game.Room.Builder
{
    public class RoomNavBuilder : MonoBehaviour
    {
        private const string LayerMaskName = "RoomNavBuilder";
        private const string PrefabPath = "Resources/Prefabs/Rooms/";
        
        public Camera buildCamera;

        public LevelController LevelControllerPrefab;
        
        // [SerializeField]
        private LevelController _buildingLevelInstance;
        public NavigationController NavigationController { get; private set; }
        
        public int nodeActionIndex { get; set; } = 0;
        public HashSet<UnitMovement> cachedNodeActions { get; private set; } = new HashSet<UnitMovement>();
        
        public bool building { get; private set; } = false;
        public int currentId { get; private set; } = 0;
        
        public void SetBuildingFlag(bool startBuilding)
        {
            if (LevelControllerPrefab.NavigationController == null)
            {
                Debug.LogError("Cannot build level, missing " + nameof(Game2D.NavigationController));
                return;
            }
            
            _buildingLevelInstance = GameObject.Instantiate(LevelControllerPrefab, Vector3.zero, Quaternion.identity);
            NavigationController = _buildingLevelInstance.NavigationController;
            
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
            NavigationController.Discard();
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

        // public void SavePrefab()
        // {
            // if (!Directory.Exists(PUZZLE_RELATIVE_FOLDER_PATH))
            // {
            //     Debug.LogError("Create Folder Path:" + PUZZLE_RELATIVE_FOLDER_PATH);
            //     return;
            // }
            //
            // string localPath = PUZZLE_RELATIVE_FOLDER_PATH 
            //                    + "/" 
            //                    + _puzzleInstance.gameObject.name 
            //                    + ".prefab";
            //
            // localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            //
            // while (_puzzleInstance.GroundPlane.childCount > 0)
            // {
            //     GameObject.DestroyImmediate(_puzzleInstance.GroundPlane.GetChild(0).gameObject);
            // }
            //
            // GameObject savedGO = PrefabUtility.SaveAsPrefabAssetAndConnect(
            //     _puzzleInstance.gameObject,
            //     localPath,
            //     InteractionMode.UserAction,
            //     out bool prefabSuccess
            // );
            //
            // if (prefabSuccess)
            // {
            //     Debug.Log("Prefab was saved successfully");
            //
            //     DestroyImmediate(_puzzleInstance.gameObject);
            //
            //     _puzzleInstance = null;
            // }
            // else
            // {
            //     Debug.LogError("Prefab failed to save" + prefabSuccess);
            // }
        // }
        
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
                    NavigationController.AddNode(hit.point, currentId);
                    currentId++;
                }
            }
        }
        #endregion
    }
}