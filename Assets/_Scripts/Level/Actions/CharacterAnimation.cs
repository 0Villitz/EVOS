
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Game2D
{
    public class CharacterAnimation : MonoBehaviour
    {
        public enum Usage
        {
            AsTrigger,
            AsInt
        }
        public Usage _usage;
        
        [SerializeField] private Animator _animator;

        [Serializable]
        public class SetupData
        {
            public UnitMovement UnitMovement;
            public string triggerOverride = null;
            public int intValue = int.MinValue;
        }

        [SerializeField] private string _animatorParameter;
        
        [Space(20)]
        [Header("Optional Animation Settings")]
        [SerializeField] private List<SetupData> _setupData = new List<SetupData>();
        private Dictionary<UnitMovement, int> _animIntMap;
        private Dictionary<UnitMovement, string> _animTriggerMap;

        public void TriggerAnimation(UnitMovement unitMovement)
        {
            if (_animator != null)
            {
                switch (_usage)
                {
                    case Usage.AsTrigger:
                        if (_animTriggerMap.TryGetValue(unitMovement, out string triggerName)
                            && !string.IsNullOrEmpty(triggerName)
                           )
                        {
                            _animator.SetTrigger(triggerName);
                        }
                        else
                        {
                            Debug.LogError("Animation trigger not set for "
                                           + nameof(unitMovement)
                                           + ": "
                                           + unitMovement
                            );
                        }
                        break;
                    
                    case Usage.AsInt when !string.IsNullOrEmpty(_animatorParameter):
                        if (_animIntMap.TryGetValue(unitMovement, out int intValue)
                            && intValue > int.MinValue
                           )
                        {
                            _animator.SetInteger(_animatorParameter, intValue);
                        }
                        else
                        {
                            Debug.LogError("Animation int value not set for "
                                           + nameof(unitMovement)
                                           + ": "
                                           + unitMovement
                            );
                        }
                        break;
                }
            }
        }

        #region Monobehavior

        private void Awake()
        {
            _animIntMap = new Dictionary<UnitMovement, int>();
            _animTriggerMap = new Dictionary<UnitMovement, string>();

            HashSet<UnitMovement> unitMovements = new HashSet<UnitMovement>();
            foreach (UnitMovement unitMovement in Enum.GetValues(typeof(UnitMovement)))
            {
                unitMovements.Add(unitMovement);
            }
            
            foreach (SetupData data in _setupData)
            {
                if (unitMovements.Contains(data.UnitMovement))
                {
                    unitMovements.Remove(data.UnitMovement);
                    _animTriggerMap[data.UnitMovement] = !string.IsNullOrEmpty(data.triggerOverride)
                        ? data.triggerOverride
                        : data.UnitMovement.ToString();

                    _animIntMap[data.UnitMovement] = int.MinValue != data.intValue
                        ? data.intValue
                        : int.MinValue;
                }
                else
                {
                    Debug.LogError("Duplicated data for "
                                   + nameof(UnitMovement)
                                   + ": "
                                   + unitMovements.ToString()
                    );
                }
            }

            foreach (UnitMovement unitMovement in unitMovements)
            {
                _animTriggerMap[unitMovement] = unitMovement.ToString();
            }
        }

        #endregion
    }
}