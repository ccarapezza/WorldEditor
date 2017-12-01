using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour {

    public int rows;
    public int columns;

    public float tileWidth = 1;
    public float tileHeight = 1;

    public GameObject grassPrefab;

    public WorldMap()
    {
        columns = 20;
        rows = 10;
    }

    private void OnDrawGizmosSelected()
    {
        float worldWidth = columns * tileWidth;
        float worldHeight = rows * tileHeight;
        Vector3 worldPosition = transform.position;

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(worldWidth/2, worldHeight/2, 0), new Vector3(worldWidth, worldHeight, 0));
        
        Gizmos.color = Color.grey;
        for (float i = 1; i < columns; i++)
            Gizmos.DrawLine(worldPosition + new Vector3(i * tileWidth, 0, 0), worldPosition + new Vector3(i * tileWidth, worldHeight, 0));

        for (float i = 1; i < rows; i++)
            Gizmos.DrawLine(worldPosition + new Vector3(0, i * tileHeight, 0), worldPosition + new Vector3(worldWidth, i * tileHeight, 0));

        
    }
}
