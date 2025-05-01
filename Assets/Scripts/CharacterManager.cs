using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3 playerSpawnOffset = Vector3.zero;

    private void OnEnable()
    {
        LibraryGridGenerator.OnGridReady += HandleGridReady;
    }


    private void OnDisable()
    {
        LibraryGridGenerator.OnGridReady -= HandleGridReady;
    }

    private void HandleGridReady()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("Player prefab not assigned in CharacterManager.");
            return;
        }

        Vector3 spawnPosition = Vector3.zero + playerSpawnOffset;
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        player.name = "Player";
    }
}
