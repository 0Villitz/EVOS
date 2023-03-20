using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Puzzles
{
    public class PathPuzzle : PuzzleBase
    {
#region Editor Properties
        public ScriptableEventDispatcher  _GameEventDispatcher;
        public Camera                     _Camera;
        public PathPlayer _Player;

        public Button    _StartButton;
        public Button    _ExitButton;
        public Transform _ButtonGroup;
#endregion

#region Private vars
        private Vector2 _startOffset;
        private List<MovablePlatform> _movableObstacleList;
#endregion

#region Public API

        public override void Init()
        {
            _Player.Init(OnObstacleHit);

            _movableObstacleList = GetComponentsInChildren<MovablePlatform>().ToList();
        }
#endregion

#region Unity API
        private void Awake()
        {
            _StartButton.onClick.AddListener(OnStartButton);
            _ExitButton.onClick.AddListener(OnExitButton);
        }

        private void Update()
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                StartPuzzle();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                OnExitButton();
            }

            Vector2 worldMousePos =  CameraUtils.GetMouseWorldPoint(_Camera, Mouse.current.position.ReadValue(), GetScaleFactor());
            Vector2 worldMousePosWithOffset = worldMousePos - _startOffset;
            
            _Player.Move(worldMousePosWithOffset);

            // For Debugging
            Debug.DrawLine(_Camera.transform.position, worldMousePos, Color.blue);
        }
#endregion

#region Private Methods
        private void StartPuzzle()
        {
            _movableObstacleList.ForEach(x => x.Reset(false));

            _Player.Reset();

            Vector3 playerPos = _Player.transform.position;
            playerPos.z = _Camera.transform.position.z;

            Vector2 worldMousePos =  CameraUtils.GetMouseWorldPoint(_Camera, Mouse.current.position.ReadValue(), GetScaleFactor());

            _startOffset   = worldMousePos - (Vector2)playerPos;
            Cursor.visible = false;
        }

        private float GetScaleFactor()
        {
            return WindowCanvas != null ? WindowCanvas.scaleFactor : 1.0f;
        }

        private void OnStartButton()
        {
            _ButtonGroup.gameObject.SetActive(false);

            StartPuzzle();
        }

        private void OnExitButton()
        {
            _GameEventDispatcher.DispatchEvent(GameEventType.HidePuzzleWindow);
        }

        private void OnObstacleHit(IPuzzleInteractable interactable)
        {
            if (interactable is PuzzleAudioPlayer)
            {
                (interactable as PuzzleAudioPlayer).PlayerCollision(_Player._Collider);
            }
            else
            {
                _movableObstacleList.ForEach(x => x.IsPaused = true);

                _ButtonGroup.gameObject.SetActive(true);
                Cursor.visible   = true;
                Cursor.lockState = CursorLockMode.None;
                
                bool wasPuzzleSuccess = interactable is PuzzleGoal;

                TriggerPuzzleComplete(wasPuzzleSuccess);
            }
        }
#endregion
    }
}
