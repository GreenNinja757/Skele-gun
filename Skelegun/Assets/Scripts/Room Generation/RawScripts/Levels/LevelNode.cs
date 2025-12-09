using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class LevelNode
{
    public string id;
    public string displayName;
    public RoomType roomType;

    [Tooltip("Shared pool of rooms for this type")]
    public RoomPool pool; 

    public List<string> connectedNodeIDs = new List<string>();
    public Vector2 editorPosition, size;

    public LevelNode()
    {
        id = Guid.NewGuid().ToString();
        displayName = "Node";
        connectedNodeIDs = new List<string>();
    }
}