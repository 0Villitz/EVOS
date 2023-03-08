
using UnityEngine;

namespace Game2D
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private NavigationController _navigationController;
        public NavigationController NavigationController => _navigationController;

    }
}