using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static Vector3 nextSpawnPoint;
    public static bool hasSpawnPoint = false;

    public static void SetSpawnPoint(Vector3 position)
    {
        nextSpawnPoint = position;
        hasSpawnPoint = true;
    }
}
