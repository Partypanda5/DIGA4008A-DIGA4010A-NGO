using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private static Transform[] points;

    private void Awake()
    {
        points = GetComponentsInChildren<Transform>();

        // points[0] will be the parent, so ensure there are child points too
        if (points.Length <= 1)
            Debug.LogWarning("SpawnPoints: Add child spawn points under this object.");
    }

    public static Vector3 GetRandomSpawnPosition()
    {
        if (points == null || points.Length <= 1)
            return Vector3.zero;

        int index = Random.Range(1, points.Length);
        return points[index].position;
    }
}