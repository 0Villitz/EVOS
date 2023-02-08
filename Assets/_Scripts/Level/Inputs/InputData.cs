
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game2D
{
    [Serializable]
    public class InputData
    {
        public int vertical { get; private set; } = 0;
        public int horizontal { get; private set; }= 0;
        public bool interactWithEntities { get; private set; } = false;

        public void SetHorizontal(int h)
        {
            horizontal = Mathf.Clamp( h, -1, 1);
        }
        
        public void SetVertical(int v)
        {
            vertical = Mathf.Clamp(v, -1, 1);
        }
        
        public void EnableInteractionWithEntities()
        {
            interactWithEntities = true;
        }

        public List<IInteractableObject> InteractableEntities
        {
            get
            {
                return (interactWithEntities)
                    ? new List<IInteractableObject>(_interactableEntities)
                    : null;
            }
        }

        public void AddInteractableEntity(IInteractableObject entity)
        {
            _interactableEntities.Add(entity);
        }

        public void RemoveInteractableEntity(IInteractableObject entity)
        {
            _interactableEntities.Remove(entity);
        }

        private HashSet<IInteractableObject> _interactableEntities = new HashSet<IInteractableObject>();

        public void ResetInputs()
        {
            vertical = 0;
            horizontal = 0;

            if (interactWithEntities)
            {
                interactWithEntities = _interactableEntities.Count > 0;
            }
        }
    }
}