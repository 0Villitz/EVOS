using System.Collections.Generic;
using UnityEngine;

namespace Puzzles
{
    public class PuzzleAudioPlayer : MonoBehaviour, IPuzzleInteractable
    {
        public Collider2D  _CollisionCollider;
        public AudioSource _AudioSource;
        
        [Range(0, 100)]
        public int _ChanceToPlay = 100;
        
        public List<AudioClip> _AudioList;


        public void PlayerCollision(Collider2D player)
        {
            if (_AudioList.Count == 0)
                return;
                
            int randomChanceToPlayValue = Random.Range(0, 101);

            if (randomChanceToPlayValue <= _ChanceToPlay)
            {
                AudioClip clip = _AudioList[Random.Range(0, _AudioList.Count)];
                _AudioSource.PlayOneShot(clip);
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            
        }
    }
}
