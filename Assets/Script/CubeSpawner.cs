using System.Collections;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnHeight = 0f;
    public float spawnInterval = 3f;

    private void Start()
    {
        // Start spawning cubes
        StartCoroutine(SpawnCubes());
    }

    private IEnumerator SpawnCubes()
    {
        while (true)
        {
            // Wait for the specified spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Spawn a cube at the specified height
            Vector3 spawnPosition = new Vector3(transform.position.x, spawnHeight, transform.position.z);
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        }
    }
}
