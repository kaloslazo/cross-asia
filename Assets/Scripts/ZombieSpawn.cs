using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawn : MonoBehaviour
{
    public GameObject zombiePrefab; // Prefab del zombie
    public int maxZombies = 10; // M�ximo n�mero de zombies
    public float spawnRadius = 50f; // Radio de aparici�n alrededor del spawner
    public float spawnInterval = 5f; // Intervalo de tiempo entre spawns
    public float heightOffset = 0.5f; // Compensaci�n de altura para ajustar la posici�n del spawn si es necesario

    private List<GameObject> zombies; // Lista para mantener el seguimiento de los zombies

    void Start()
    {
        zombies = new List<GameObject>();
        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            if (zombies.Count < maxZombies)
            {
                Vector3 spawnPosition = RandomNavSphere(transform.position, spawnRadius, -1);
                if (spawnPosition != Vector3.zero)
                {
                    // Ajuste de la altura para asegurar que los zombies no floten ni est�n enterrados
                    spawnPosition += Vector3.up * heightOffset;
                    Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnPosition.normalized);
                    GameObject zombie = Instantiate(zombiePrefab, spawnPosition, spawnRotation);
                    zombies.Add(zombie);
                }
            }
            // Limpiar la lista de zombies que han sido destruidos (por ejemplo, si son eliminados del juego)
            zombies.RemoveAll(zombie => zombie == null);
        }
    }

    // Funci�n para encontrar una posici�n aleatoria que sea v�lida en la NavMesh
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist + origin;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
            {
                return navHit.position;
            }
        }
        return Vector3.zero; // Retornar Vector3.zero si no se encuentra una posici�n v�lida
    }
}
