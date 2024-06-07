#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    public Rect rect; // Rectángulo que representa el nodo
    public string title; // Título del nodo
    public bool isDragged; // Indica si el nodo está siendo arrastrado

    public Node(Vector2 position, float width, float height, string title)
    {
        rect = new Rect(position.x, position.y, width, height);
        this.title = title;
    }
}

public enum NodeType
{
    Type1,
    Type2,
    Type3
}
public class NodeEditor : EditorWindow
{
    private List<Node> nodes = new List<Node>();
    // Esta clase representa un nodo en el editor
    private NodeType selectedNodeType = NodeType.Type1; // Establece el tipo de nodo predeterminado

    private Node selectedNode; // Nodo seleccionado actualmente
    private Vector2 dragStartPos; // Posición inicial del arrastre
    private Vector2 offset; // Offset para ajustar la posición del nodo mientras se arrastra
    private Vector2 scrollPos; // Posición del scroll

    [MenuItem("Window/Node Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(NodeEditor), false, "Node Editor");
    }
    private void ProcessContextMenu(Event e)
    {
        if (e.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Node Type 1"), false, () => { selectedNodeType = NodeType.Type1; });
            menu.AddItem(new GUIContent("Node Type 2"), false, () => { selectedNodeType = NodeType.Type2; });
            menu.AddItem(new GUIContent("Node Type 3"), false, () => { selectedNodeType = NodeType.Type3; });
            menu.ShowAsContext();
            e.Use(); // Este evento debe ser consumido para evitar que otros sistemas lo procesen
        }
    }
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Node Type 1"), false, () => { selectedNodeType = NodeType.Type1; });
        menu.AddItem(new GUIContent("Node Type 2"), false, () => { selectedNodeType = NodeType.Type2; });
        menu.AddItem(new GUIContent("Node Type 3"), false, () => { selectedNodeType = NodeType.Type3; });
        menu.ShowAsContext();
    }
    void OnGUI()
    {
        DrawNodes();
        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);
    }
    private void DrawNodeWindow(int id)
    {
        // Contenido del nodo aquí
        GUI.DragWindow();
    }
    // Dibuja los nodos en la ventana del editor
    private void DrawNodes()
    {
        BeginWindows();
        foreach (var node in nodes)
        {
            node.rect = GUI.Window(nodes.IndexOf(node), node.rect, DrawNodeWindow, node.title);
        }
        EndWindows();
    }
  private void CreateNode(NodeType nodeType, Vector2 position)
{
    string title = "New Node"; // Puedes cambiar el título según el tipo de nodo
    switch (nodeType)
    {
        case NodeType.Type1:
            title = "Node Type 1";
            break;
        case NodeType.Type2:
            title = "Node Type 2";
            break;
        case NodeType.Type3:
            title = "Node Type 3";
            break;
    }
    nodes.Add(new Node(position, 200, 100, title));
}
    // Procesa eventos de los nodos (selección, arrastre, etc.)
    private void ProcessNodeEvents(Event e)
    {
        // Procesar eventos de los nodos aquí
    }

    // Procesa eventos generales del editor (p.ej. scroll)
    private void ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1) // Botón derecho del ratón
                {
                    ShowContextMenu(e.mousePosition);
                    CreateNode(selectedNodeType, e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
            case EventType.ScrollWheel:
                OnScroll(e.delta);
                break;
        }
    }

    // Procesa el arrastre del mouse
    private void OnDrag(Vector2 delta)
    {
        // Mover todos los nodos si no hay ninguno seleccionado
        if (selectedNode == null)
        {
            scrollPos += delta;
        }
        // Si hay un nodo seleccionado, mover ese nodo
        else
        {
            selectedNode.rect.position += delta;
            selectedNode.isDragged = true;
            GUI.changed = true;
        }
    }

    // Procesa el scroll del mouse
    private void OnScroll(Vector2 delta)
    {
        scrollPos += delta * 10f; // Ajusta la velocidad de desplazamiento
        GUI.changed = true;
    }

    // Desmarca todos los nodos seleccionados
    private void ClearNodeSelection()
    {
        selectedNode = null;
        GUI.changed = true;
    }
}
#endif