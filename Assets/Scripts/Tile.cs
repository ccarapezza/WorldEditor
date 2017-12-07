using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Tile", order = 150)]
public class Tile : ScriptableObject
{
    public Sprite sprite;
    public string tileName;
    public bool isWalkable;
    public float movingCost;
}