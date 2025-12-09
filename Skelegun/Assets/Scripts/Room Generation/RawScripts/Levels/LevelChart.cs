using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skelegun/RoomGen/LevelChart")]
public class LevelChart : ScriptableObject
{
    public List<LevelNode> nodes = new List<LevelNode>();

    public LevelNode GetNode(string id)
    {
        return nodes.Find(n => n.id == id);
    }
}
