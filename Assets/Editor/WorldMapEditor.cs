using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

enum EditMode
{
    Pencil,
    Rubber,
    Bucket
}

[CustomEditor(typeof(WorldMap))]
public class WorldMapEditor : Editor {
    private int m_editMode;
    private int m_tileSelected;
    private MapLayers m_layerSelected = MapLayers.Terrain;

    private int m_lastRows;
    private int m_lastColumns;

    void OnSceneGUI()
    {
        WorldMap worldMap = (WorldMap)target;
        Vector3 worldPosition = worldMap.transform.position;
        float worldWidth = worldMap.columns * worldMap.tileWidth;
        float worldHeight = worldMap.rows * worldMap.tileHeight;
        if (worldMap.rows != m_lastRows || worldMap.columns != m_lastColumns)
        {
            m_lastRows = worldMap.rows;
            m_lastColumns = worldMap.columns;
            worldMap.RecreateMapMatrix();
        }

        Handles.BeginGUI();
        


        Vector3 worldPos = Camera.current.ScreenToWorldPoint(worldPosition);
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
        mousePosition = Camera.current.ScreenToWorldPoint(mousePosition);

        Vector2 gridPosition = new Vector2((int)mousePosition.x, (int)mousePosition.y);

        string[] tileGuids = AssetDatabase.FindAssets("t:Tile", null);
        List<Tile> tileset = new List<Tile>();
        foreach (var tileGuid in tileGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(tileGuid);
            tileset.Add(AssetDatabase.LoadAssetAtPath<Tile>(path));
        }

        //Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Tileset/mountain_landscape.png").OfType<Sprite>().ToArray();

        if (gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < worldWidth && gridPosition.y < worldHeight)
        {
            Vector3 vertOne = Camera.current.WorldToScreenPoint(worldPosition);
            Vector3 vertTwo = Camera.current.WorldToScreenPoint(worldPosition+Vector3.right);

            Vector3 realVertOne = new Vector3(vertOne.x, Camera.current.pixelHeight - vertOne.y, 0);
            Vector3 realVertTwo = new Vector3(vertTwo.x, Camera.current.pixelHeight - vertTwo.y, 0);

            Vector3 pos = Camera.current.WorldToScreenPoint((worldPosition + Vector3.up) + (Vector3.up * gridPosition.y) + (Vector3.right * gridPosition.x));
            Vector3 realPos = new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0);
            Color tmp = GUI.color;
            GUI.color = new Color(1, 1, 1, 0.1f);
            if (GUI.Button(new Rect(realPos, Vector3.one * Vector3.Distance(realVertOne, realVertTwo)), ""))
            {
                if (Event.current.button == 0)
                {
                    if(m_editMode == (int)EditMode.Pencil)
                        worldMap.CreateTile(tileset[m_tileSelected], gridPosition, m_layerSelected);

                    if (m_editMode == (int)EditMode.Rubber)
                        worldMap.DeleteTile(gridPosition, m_layerSelected);

                    if (m_editMode == (int)EditMode.Bucket)
                        worldMap.PaintTile(tileset[m_tileSelected], gridPosition, m_layerSelected);
                }

                if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, null);                  
                    //menu.AddSeparator("");
                    menu.ShowAsContext();
                }
            
            }

            pos = Camera.current.WorldToScreenPoint((worldPosition + Vector3.up*0.5f + Vector3.right * 0.5f) + (Vector3.up * gridPosition.y) + (Vector3.right * gridPosition.x));
            realPos = new Vector3(pos.x, Camera.current.pixelHeight - pos.y, 0);
            if (m_editMode == (int)EditMode.Pencil)
                Handles.color = Color.green;
            else
                Handles.color = Color.red;
            Handles.DrawWireCube(realPos, Vector3.one * Vector3.Distance(realVertOne, realVertTwo)*1.1f);
            GUI.color = tmp;
        }

        GUILayout.BeginHorizontal();
        Texture pencilIcon = (Texture)EditorGUIUtility.Load("pencil.png");
        Texture rubberIcon = (Texture)EditorGUIUtility.Load("rubber.png");
        Texture bucketIcon = (Texture)EditorGUIUtility.Load("bucket.png");
        Texture[] textures = new Texture[]{ pencilIcon, rubberIcon, bucketIcon };
        m_editMode = GUILayout.Toolbar(m_editMode, textures, GUILayout.Width(48 * textures.Length), GUILayout.Height(48));

        GUILayout.FlexibleSpace();
        Texture layersIcon = (Texture)EditorGUIUtility.Load("layers.png");

        GUILayout.BeginVertical();
        GUILayout.Label("Current Layer:", new GUIStyle("WhiteBoldLabel"));
        GUILayout.Label(Enum.GetName(typeof(MapLayers), m_layerSelected), new GUIStyle("AssetLabel"));
        GUILayout.EndVertical();

        if (GUILayout.Button(layersIcon, GUILayout.Width(50), GUILayout.Height(50)))
        {
            GenericMenu menu = new GenericMenu();
            Array enums = Enum.GetValues(typeof(MapLayers));
            Array.Reverse(enums);
            foreach (MapLayers layer in enums)
            {
                bool selected = (layer == m_layerSelected);
                menu.AddItem(new GUIContent(Enum.GetName(typeof(MapLayers), layer)), selected, ChangeLayer, layer);
            }
            //menu.AddSeparator("");
            menu.ShowAsContext();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        Color tmpColor = GUI.backgroundColor;
        for (int i = 0; i < tileset.Count; i++)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(tileset[i].sprite);
            GUIStyle btnStyle = GUI.skin.button;
            if (m_tileSelected == i)
            {
                GUI.backgroundColor = Color.red;
                GUI.skin.button.border = new RectOffset(5,5,5,5);
            }

            if (GUILayout.Button(texture, btnStyle, GUILayout.Width(50), GUILayout.Height(50)))
                m_tileSelected = i;

            GUI.backgroundColor = tmpColor;
        }
        
        GUILayout.EndHorizontal();

        Handles.EndGUI();

        SceneView.RepaintAll();
    }

    void ChangeLayer(object layer)
    {
        m_layerSelected = (MapLayers)layer;
    }
}