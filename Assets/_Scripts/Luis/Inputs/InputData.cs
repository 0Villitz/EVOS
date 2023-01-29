
using System;
using System.Collections.Generic;

namespace Game2D
{
    [Serializable]
    public class InputData
    {
        public int vertical = 0;
        public int horizontal = 0;
        public bool interactWithEntities { get; private set; } = false;

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