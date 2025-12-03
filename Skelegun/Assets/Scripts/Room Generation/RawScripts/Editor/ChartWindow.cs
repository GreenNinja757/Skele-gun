using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ChartWindow : EditorWindow
{
    private LevelNode resizingNode = null;
    private ResizeDirection resizeDir = ResizeDirection.None;
    private Vector2 resizeStartMouse;
    private Rect originalNodeRect;

    private LevelNode draggingNode = null;
    private Vector2 dragOffset;

    private LevelChart currentChart;
    private LevelNode firstSelectedNode = null;
    private LevelNode secondSelectedNode = null;

    private Dictionary<string, bool> nodeFoldouts = new Dictionary<string, bool>();
    private Vector2 scrollPosition;

    [MenuItem("Skelegun/Level Flow Editor")]
    public static void OpenWindow()
    {
        GetWindow<ChartWindow>("Level Flow Editor");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        // Select Flowchart asset
        currentChart = (LevelChart)EditorGUILayout.ObjectField(
            "Flowchart", currentChart, typeof(LevelChart), false);

        if (currentChart == null)
        {
            EditorGUILayout.HelpBox("Assign a Flowchart to begin editing.", MessageType.Info);
            return;
        }

        // Draw connections first (behind nodes)
        DrawConnections();

        // Draw nodes
        for (int i = 0; i < currentChart.nodes.Count; i++)
        {
            DrawNode(currentChart.nodes[i]);
        }

        ProcessEvents(Event.current);

        if (GUILayout.Button("Add Node"))
        {
            Undo.RecordObject(currentChart, "Add Node");

            var node = new LevelNode();
            node.editorPosition = Vector2.zero;

            currentChart.nodes.Add(node);

            EditorUtility.SetDirty(currentChart);
        }
    }

    private void DrawNode(LevelNode node)
    {
        if (node.size == Vector2.zero)
            node.size = new Vector2(250, 140);

        Rect rect = new Rect(node.editorPosition, node.size);

        // Highlight if selected for linking
        if (node == firstSelectedNode)
            GUI.color = Color.yellow;

        // Draw node box
        GUI.Box(rect, "");
        GUI.color = Color.white;

        GUILayout.BeginArea(rect);
        GUILayout.BeginVertical();

        // Editable name
        string idDisplay = string.IsNullOrEmpty(node.id) ? "No ID" :
            (node.id.Length > 8 ? node.id.Substring(0, 8) + "..." : node.id);
        EditorGUILayout.LabelField("Node ID: " + idDisplay, EditorStyles.miniLabel);

        node.displayName = EditorGUILayout.TextField("Name", node.displayName);
        node.roomType = (RoomType)EditorGUILayout.EnumPopup("Type", node.roomType);

        // Room Pool reference (ScriptableObject)
        node.pool = (RoomPool)EditorGUILayout.ObjectField("Room Pool", node.pool, typeof(RoomPool), false);

        if (node.pool != null)
        {
            int validRooms = 0;
            foreach (var prefab in node.pool.roomPrefabs)
            {
                if (prefab != null && prefab.GetComponent<RoomData>() != null)
                    validRooms++;
            }
            EditorGUILayout.LabelField($"Pool: {node.pool.poolType} ({validRooms} valid rooms)", EditorStyles.miniLabel);
        }

        GUILayout.Space(5);

        // Link button
        GUI.color = (firstSelectedNode == node) ? Color.yellow : Color.green;
        if (GUILayout.Button(firstSelectedNode == node ? "Selected (pick target)" : "Link Node"))
        {
            if (firstSelectedNode == null)
            {
                firstSelectedNode = node;
            }
            else if (firstSelectedNode != node)
            {
                secondSelectedNode = node;

                // Create bidirectional connection
                if (!firstSelectedNode.connectedNodeIDs.Contains(secondSelectedNode.id))
                    firstSelectedNode.connectedNodeIDs.Add(secondSelectedNode.id);

                if (!secondSelectedNode.connectedNodeIDs.Contains(firstSelectedNode.id))
                    secondSelectedNode.connectedNodeIDs.Add(firstSelectedNode.id);

                // Reset selections
                firstSelectedNode = null;
                secondSelectedNode = null;

                EditorUtility.SetDirty(currentChart);
            }
        }
        GUI.color = Color.white;

        // Delete button
        GUI.color = Color.red;
        if (GUILayout.Button("Delete Node"))
        {
            GUILayout.EndVertical();
            GUILayout.EndArea();

            // Remove connections to this node
            foreach (var otherNode in currentChart.nodes)
            {
                otherNode.connectedNodeIDs.RemoveAll(id => id == node.id);
            }

            currentChart.nodes.Remove(node);
            nodeFoldouts.Remove(node.id);
            EditorUtility.SetDirty(currentChart);

            GUI.color = Color.white;
            return;
        }
        GUI.color = Color.white;

        GUILayout.EndVertical();
        GUILayout.EndArea();

        // Draw resize handles
        DrawResizeHandles(node, rect);
    }

    private void DrawConnections()
    {
        if (currentChart == null) return;

        foreach (var node in currentChart.nodes)
        {
            Vector2 start = node.editorPosition + node.size / 2;

            foreach (var destID in node.connectedNodeIDs)
            {
                LevelNode other = currentChart.nodes.Find(n => n.id == destID);
                if (other != null)
                {
                    Vector2 end = other.editorPosition + other.size / 2;

                    // Draw arrow
                    Handles.color = Color.cyan;
                    Handles.DrawLine(start, end);

                    // Draw direction arrow
                    Vector2 direction = (end - start).normalized;
                    Vector2 arrowPos = Vector2.Lerp(start, end, 0.7f);
                    DrawArrow(arrowPos, direction, 10f);
                }
            }
        }
    }

    private void DrawArrow(Vector2 position, Vector2 direction, float size)
    {
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);
        Vector2 back = position - direction * size;
        Vector2 left = back + perpendicular * (size * 0.5f);
        Vector2 right = back - perpendicular * (size * 0.5f);

        Handles.DrawLine(position, left);
        Handles.DrawLine(position, right);
    }

    private void ProcessEvents(Event e)
    {
        // Begin drag only if not resizing
        if (e.type == EventType.MouseDown && resizingNode == null)
        {
            draggingNode = NodeAtPosition(e.mousePosition);
            if (draggingNode != null)
            {
                dragOffset = draggingNode.editorPosition - e.mousePosition;
                GUI.changed = true;
            }
        }

        // Drag node
        if (e.type == EventType.MouseDrag && draggingNode != null && resizingNode == null)
        {
            Undo.RecordObject(currentChart, "Move Node");
            draggingNode.editorPosition = e.mousePosition + dragOffset;
            GUI.changed = true;
        }

        // Resize node
        if (resizingNode != null && e.type == EventType.MouseDrag)
        {
            Vector2 delta = e.mousePosition - resizeStartMouse;
            Rect r = originalNodeRect;

            switch (resizeDir)
            {
                case ResizeDirection.Left: r.xMin += delta.x; break;
                case ResizeDirection.Right: r.xMax += delta.x; break;
                case ResizeDirection.Top: r.yMin += delta.y; break;
                case ResizeDirection.Bottom: r.yMax += delta.y; break;
                case ResizeDirection.TopLeft: r.xMin += delta.x; r.yMin += delta.y; break;
                case ResizeDirection.TopRight: r.xMax += delta.x; r.yMin += delta.y; break;
                case ResizeDirection.BottomLeft: r.xMin += delta.x; r.yMax += delta.y; break;
                case ResizeDirection.BottomRight: r.xMax += delta.x; r.yMax += delta.y; break;
            }

            // Min size
            r.width = Mathf.Max(200, r.width);
            r.height = Mathf.Max(150, r.height);

            resizingNode.editorPosition = new Vector2(r.xMin, r.yMin);
            resizingNode.size = new Vector2(r.width, r.height);

            GUI.changed = true;
        }

        // Stop drag/resize
        if (e.type == EventType.MouseUp)
        {
            draggingNode = null;
            resizingNode = null;
            resizeDir = ResizeDirection.None;
        }

        // Deselect on empty space click
        if (e.type == EventType.MouseDown && !AnyNodeAtPosition(e.mousePosition))
        {
            firstSelectedNode = null;
            secondSelectedNode = null;
            GUI.changed = true;
        }

        if (GUI.changed)
            Repaint();
    }

    private void DrawResizeHandles(LevelNode node, Rect rect)
    {
        float handleSize = 10f;

        // Corners
        Rect topLeft = new Rect(rect.xMin - handleSize / 2, rect.yMin - handleSize / 2, handleSize, handleSize);
        Rect topRight = new Rect(rect.xMax - handleSize / 2, rect.yMin - handleSize / 2, handleSize, handleSize);
        Rect bottomLeft = new Rect(rect.xMin - handleSize / 2, rect.yMax - handleSize / 2, handleSize, handleSize);
        Rect bottomRight = new Rect(rect.xMax - handleSize / 2, rect.yMax - handleSize / 2, handleSize, handleSize);

        // Edges
        Rect top = new Rect(rect.xMin + rect.width / 2 - handleSize / 2, rect.yMin - handleSize / 2, handleSize, handleSize);
        Rect bottom = new Rect(rect.xMin + rect.width / 2 - handleSize / 2, rect.yMax - handleSize / 2, handleSize, handleSize);
        Rect left = new Rect(rect.xMin - handleSize / 2, rect.yMin + rect.height / 2 - handleSize / 2, handleSize, handleSize);
        Rect right = new Rect(rect.xMax - handleSize / 2, rect.yMin + rect.height / 2 - handleSize / 2, handleSize, handleSize);

        // Set cursors
        EditorGUIUtility.AddCursorRect(topLeft, MouseCursor.ResizeUpLeft);
        EditorGUIUtility.AddCursorRect(topRight, MouseCursor.ResizeUpRight);
        EditorGUIUtility.AddCursorRect(bottomLeft, MouseCursor.ResizeUpRight);
        EditorGUIUtility.AddCursorRect(bottomRight, MouseCursor.ResizeUpLeft);
        EditorGUIUtility.AddCursorRect(top, MouseCursor.ResizeVertical);
        EditorGUIUtility.AddCursorRect(bottom, MouseCursor.ResizeVertical);
        EditorGUIUtility.AddCursorRect(left, MouseCursor.ResizeHorizontal);
        EditorGUIUtility.AddCursorRect(right, MouseCursor.ResizeHorizontal);

        Event e = Event.current;
        if (e.type == EventType.MouseDown)
        {
            if (topLeft.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.TopLeft);
            else if (topRight.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.TopRight);
            else if (bottomLeft.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.BottomLeft);
            else if (bottomRight.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.BottomRight);
            else if (top.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.Top);
            else if (bottom.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.Bottom);
            else if (left.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.Left);
            else if (right.Contains(e.mousePosition)) StartResizing(node, rect, ResizeDirection.Right);
        }
    }

    private void StartResizing(LevelNode node, Rect rect, ResizeDirection dir)
    {
        resizingNode = node;
        resizeDir = dir;
        resizeStartMouse = Event.current.mousePosition;
        originalNodeRect = rect;
        Event.current.Use();
    }

    private bool AnyNodeAtPosition(Vector2 pos)
    {
        return NodeAtPosition(pos) != null;
    }

    private LevelNode NodeAtPosition(Vector2 pos)
    {
        foreach (var node in currentChart.nodes)
        {
            Rect rect = new Rect(node.editorPosition, node.size);
            if (rect.Contains(pos))
                return node;
        }
        return null;
    }
}

public enum ResizeDirection
{
    None,
    Left, Right, Top, Bottom,
    TopLeft, TopRight, BottomLeft, BottomRight
}