using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour {

    public int rows;
    public int columns;

    private int m_lastRows;
    private int m_lastColumns;

    public float tileWidth = 1;
    public float tileHeight = 1;

    public GameObject grassPrefab;
    private GameObject[,] m_map;

    public WorldMap()
    {
        columns = 20;
        rows = 10;
        recreateMapArray();
    }

    private void recreateMapArray()
    {
        m_lastRows = rows;
        m_lastColumns = columns;
        m_map = new GameObject[rows, columns];
    }

    private void Update()
    {
        if (rows != m_lastRows || columns != m_lastColumns)
            recreateMapArray();
    }

    public void CreateTile(Sprite sprite, Vector2 gridPosition)
    {
        if (m_map[(int)gridPosition.x, (int)gridPosition.y] != null) return;
        GameObject tile = Instantiate(grassPrefab, transform.position + new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);
        tile.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        tile.transform.SetParent(transform);
        m_map[(int)gridPosition.x, (int)gridPosition.y] = tile;
    }

    public void DeleteTile(Vector2 gridPosition)
    {
        GameObject tile = m_map[(int)gridPosition.x, (int)gridPosition.y];
        if (tile == null) return;
        DestroyImmediate(tile);
        m_map[(int)gridPosition.x, (int)gridPosition.y] = null;
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
