using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private static List<SpawnPoints> points = new List<SpawnPoints>();

    private void OnEnable()
    {
        points.Add(this);
    }
    public static Vector3 GetRandomSpawnPoint()
    {
        if(points.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 spawnPoint = points[Random.Range(0, points.Count)].transform.position;
        return spawnPoint;
    }

    private void OnDisable()
    {
        points.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
