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
        private Vector2                 _cameraPrevPosition;
        private List<MovablePlatform>   _movableObstacleList;
        private List<PuzzleAudioPlayer> _audioPlayerList;
#endregion

#region Public API

        public override void Init()
        {
            _Player.Init(OnObstacleHit);

            _movableObstacleList = GetComponentsInChildren<MovablePlatform>().ToList();
            _audioPlayerList     = GetComponentsInChildren<PuzzleAudioPlayer>().ToList();
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

            Vector2 mouseDelta   = Mouse.current.delta.ReadValue();
            Vector2 newPlayerPos = (Vector2)_Player.transform.position + mouseDelta * GetScaleFactor() * _Player._MouseSensitivity;
            
            Vector2 camPosition = _Camera.transform.position;
            Vector2 camDelta    = camPosition - _cameraPrevPosition;
            
            _Player.Move(newPlayerPos + camDelta);

            _cameraPrevPosition = camPosition;
        }
#endregion

#region Private Methods
        private void StartPuzzle()
        {
            _movableObstacleList.ForEach(x => x.Reset(false));
            _audioPlayerList.ForEach(x => x.Reset());
            
            _Player.Reset();

            Vector3 playerPos = _Player.transform.position;
            playerPos.z = _Camera.transform.position.z;

            _cameraPrevPosition = _Camera.transform.position;

            Cursor.visible   = false;
            Cursor.lockState = CursorLockMode.Locked;
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
        
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnObstacleHit(IPuzzleInteractable interactable)
        {

            switch (interactable)
            {
                case PuzzleAudioPlayer:
                    break;
                case PuzzleGoal:
                    PuzzleComplete(true);
                    break;
                 case MovablePlatform:
                 case ImpassableWall:
                 default:
                        PuzzleComplete(false);
                        break;
            }
        }

        private void PuzzleComplete(bool wasSuccessful)
        {
            _movableObstacleList.ForEach(x => x.IsPaused = true);
            _audioPlayerList.ForEach(x => x.Reset());
            
            _Player.IsAlive = false;
            
            _ButtonGroup.gameObject.SetActive(true);
            
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;
            
            TriggerPuzzleComplete(wasSuccessful);
        }
#endregion
    }
}
