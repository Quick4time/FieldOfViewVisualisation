using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonTester : MonoBehaviour
{
    private void Start()
    {
        // Create Vector2 vertices
        // Создаем Vector2 вершины (vertices)
        Vector2[] vertices2D = new Vector2[]
            {
            new Vector2(0,0),
            new Vector2(0,50),
            new Vector2(50,50),
            new Vector2(50,100),
            new Vector2(0,100),
            new Vector2(0,150),
            new Vector2(150,150),
            new Vector2(150,100),
            new Vector2(100,100),
            new Vector2(100,50),
            new Vector2(150,50),
            new Vector2(150,0),
            };

        // Use the triangulator to get indices for creating triangles
        // Получаем индексы спомощью триангулятора для создания  триугольников
        Triangulator tr = new Triangulator(vertices2D);
        int[] vertex = tr.Triangulate();

        // Create the Vector3 vertices
        // Создаем Vector3 вершины (vertices)
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        // Создаем Меш
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = vertex;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        // Устанавливаем для обьекта с мешем
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
    }

}
