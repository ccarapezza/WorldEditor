using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

enum EditMode
{
    Pencil,
    Rubber
}

[CustomEditor(typeof(WorldMap))]
public class WorldMapEditor : Editor {
    private int m_editMode;
    private int m_tileSelected;
    void OnSceneGUI()
    {
        WorldMap worldMap = (WorldMap)target;
        float worldWidth = worldMap.columns * worldMap.tileWidth;
        float worldHeight = worldMap.rows * worldMap.tileHeight;
        Vector3 worldPosition = worldMap.transform.position;

        Handles.BeginGUI();

        Vector3 worldPos = Camera.current.ScreenToWorldPoint(worldPosition);
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
        mousePosition = Camera.current.ScreenToWorldPoint(mousePosition);

        Vector2 gridPosition = new Vector2((int)mousePosition.x, (int)mousePosition.y);

        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Tileset/mountain_landscape.png").OfType<Sprite>().ToArray();

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
                        worldMap.CreateTile(sprites[m_tileSelected], gridPosition);

                    if (m_editMode == (int)EditMode.Rubber)
                        worldMap.DeleteTile(gridPosition);
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
        new GUIContent();
        Texture pencilIcon = (Texture)EditorGUIUtility.Load("pencil.png");
        Texture rubberIcon = (Texture)EditorGUIUtility.Load("rubber.png");

        Texture[] textures = new Texture[]{ pencilIcon, rubberIcon };
        m_editMode = GUILayout.Toolbar(m_editMode, textures, GUILayout.Width(48 * textures.Length), GUILayout.Height(48));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        Color tmpColor = GUI.backgroundColor;
        for (int i = 0; i < 10; i++)
        {
            Texture2D texture = AssetPreview.GetAssetPreview(sprites[i]);
            GUIStyle btnStyle = GUI.skin.button;
            if (m_tileSelected == i)
            {
                GUI.backgroundColor = Color.red;
                GUI.skin.button.border = new RectOffset(5,5,5,5);
            }

            if (GUILayout.Button(texture, btnStyle))
                m_tileSelected = i;

            GUI.backgroundColor = tmpColor;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        Handles.EndGUI();

        SceneView.RepaintAll();
    }
}