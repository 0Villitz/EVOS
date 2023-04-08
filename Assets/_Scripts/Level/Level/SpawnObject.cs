using System.Collections;
using System.Collections.Generic;
using Game2D;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    [SerializeField] private Transform _npcSection;
    [SerializeField] private NPCController [] _npcPrefabs;
    [SerializeField] private List<NavigationNode> _path;

    public void SpawnNPC(Transform player)
    {
        StartCoroutine(SpawnNPCCorutine(player));
    }

    private IEnumerator SpawnNPCCorutine(Transform player)
    {
        // yield return new WaitForSeconds(_spawnDelay);
        int npcIdx = Random.Range(0, _npcPrefabs.Length - 1);

        NPCController npc = GameObject.Instantiate(_npcPrefabs[npcIdx], _npcSection);
        npc.transform.position = transform.position;
        
        yield return new WaitForEndOfFrame();
        
        npc.Initialize(_path, player.GetComponent<IPlayerCharacter>());
    }
}
