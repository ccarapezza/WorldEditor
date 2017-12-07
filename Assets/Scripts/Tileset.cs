using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "Tileset", order = 150)]
public class Tileset : ScriptableObject
{
    public Sprite tilesetSource;
    public string tilesetName;
    public bool isWalkable;
}