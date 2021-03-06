﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapLayers
{
    Terrain,
    TerrainDetails,
    Middle,
    Roof
}

public class WorldMap : MonoBehaviour {

    public int rows;
    public int columns;

    public float tileWidth = 1;
    public float tileHeight = 1;

    public GameObject grassPrefab;
    private Dictionary<MapLayers, GameObject[,]> m_worldMap;

    public WorldMap()
    {
        columns = 20;
        rows = 10;
        m_worldMap = new Dictionary<MapLayers, GameObject[,]>();
        RecreateMapMatrix();
    }

    public void RecreateMapMatrix()
    {
        foreach (MapLayers layer in Enum.GetValues(typeof(MapLayers)))
        {
            if (!m_worldMap.ContainsKey(layer))
                m_worldMap[layer] = new GameObject[columns, rows];

            GameObject[,] layerMap = m_worldMap[layer];
            ResizeLayerMap(layer);
        }
    }

    public void ResizeLayerMap(MapLayers layer)
    {
        GameObject[,] mapLayer = m_worldMap[layer];

        int oldCols = mapLayer.GetLength(0);
        int oldRows = mapLayer.GetLength(1);

        if ((oldRows > rows) || (oldCols > columns))
        {
            for (int c = columns; c < oldCols; c++)
            {
                for (int r = 0; r < oldRows; r++)
                    DeleteTile(new Vector2(c, r), layer);
            }
            for (int r = rows; r < oldRows; r++)
            {
                for (int c = 0; c < oldCols; c++)
                    DeleteTile(new Vector2(c, r), layer);
            }
        }

        m_worldMap[layer] = ResizeMatrix(mapLayer, columns, rows);
    }

    private GameObject[,] ResizeMatrix(GameObject[,] matrix, int newCols, int newRows)
    {
        GameObject[,] newMatrix = new GameObject[newCols, newRows];
        int currentCols = matrix.GetLength(0);
        int currentRows = matrix.GetLength(1);
        int maxCols = Mathf.Max(newCols, currentCols);
        int maxRow = Mathf.Max(newRows, currentRows);

        for (int i = 0; i < maxCols; i++)
        {
            for (int j = 0; j < maxRow; j++)
            {
                if (newMatrix.GetLength(0) <= i) continue;
                if (newMatrix.GetLength(1) <= j) continue;
                if (matrix.GetLength(0) <= i) continue;
                if (matrix.GetLength(1) <= j) continue;
                newMatrix[i, j] = matrix[i, j];
            }
        }

        return newMatrix;
    }

    public void CreateTile(Tile tile, Vector2 gridPosition, MapLayers layer)
    {
        if (m_worldMap[layer][(int)gridPosition.x, (int)gridPosition.y] != null)
        {
            DeleteTile(gridPosition, layer);
        }
        GameObject tileGo = Instantiate(grassPrefab, transform.position + new Vector3(gridPosition.x, gridPosition.y, 0), Quaternion.identity);

        SpriteRenderer sr = tileGo.GetComponentInChildren<SpriteRenderer>();
        sr.sortingOrder = (int)layer;
        sr.sprite = tile.sprite;
        if (!tile.isWalkable)
            sr.gameObject.AddComponent<BoxCollider2D>();
        tileGo.transform.SetParent(transform);
        m_worldMap[layer][(int)gridPosition.x, (int)gridPosition.y] = tileGo;
    }

    public void DeleteTile(Vector2 gridPosition, MapLayers layer)
    {
        GameObject tile = m_worldMap[layer][(int)gridPosition.x, (int)gridPosition.y];
        if (tile == null) return;
        DestroyImmediate(tile);
        m_worldMap[layer][(int)gridPosition.x, (int)gridPosition.y] = null;
    }

    public void PaintTile(Tile tile, Vector2 gridPosition, MapLayers layer)
    {
        Sprite lastSprite = null;
        GameObject go = m_worldMap[layer][(int)gridPosition.x, (int)gridPosition.y];
        if (go != null)
        {
            lastSprite = go.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        Paint(m_worldMap[layer], tile, lastSprite, layer, (int)gridPosition.x, (int)gridPosition.y);
    }

    private void Paint(GameObject[,] worldMap, Tile tile, Sprite lastTile, MapLayers layer, int x, int y)
    {
        if (x < 0 || y < 0 || x >= worldMap.GetLength(0) || y >= worldMap.GetLength(1))
            return;

        if (lastTile == null)
        {
            if (worldMap[x, y] != null)
                return;
        }
        else
        {
            GameObject currentTile = worldMap[x, y];
            if (currentTile == null)
                return;

            if (currentTile.GetComponentInChildren<SpriteRenderer>().sprite != lastTile)
                return;
        }

        CreateTile(tile, new Vector2(x,y), layer);
        Paint(worldMap, tile, lastTile, layer, x + 1, y);
        Paint(worldMap, tile, lastTile, layer, x, y + 1);
        Paint(worldMap, tile, lastTile, layer, x - 1, y);
        Paint(worldMap, tile, lastTile, layer, x, y - 1);
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