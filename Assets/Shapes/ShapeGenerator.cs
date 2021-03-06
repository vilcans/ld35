﻿using UnityEngine;
using UnityEngine.Assertions;

public class ShapeGenerator : MonoBehaviour {
    /// Number of points around the circumference of the shape
    private const int outerPointCount = 16;

    private MeshFilter myMeshFilter;
    private Mesh mesh;

    private Vector3[] currentVertices;
    private Vector3[] sourceVertices;
    private Vector3[] targetVertices;

    private float morphSpeed = 2f;
    private float morphProgress;

    void Start() {
        currentVertices = new Vector3[outerPointCount + 1];

        sourceVertices = Shapes.squareVertices;
        targetVertices = sourceVertices;
        //targetVertices = CreateStarPoints(.3f, .5f);

        myMeshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = CreateMesh(currentVertices);
        myMeshFilter.mesh = mesh;
    }

    public void Morph(Vector3[] newVertices) {
        targetVertices = newVertices;
        morphProgress = 0;
    }

    public void SetShape(Vector3[] newVertices) {
        sourceVertices = targetVertices = newVertices;
    }

    public void Update() {
        if(morphProgress >= 1) {
            return;
        }
        morphProgress += morphSpeed * Time.deltaTime;
        if(morphProgress >= 1) {
            sourceVertices = targetVertices;
            morphProgress = 1;
        }

        for(int i = 0; i < outerPointCount; ++i) {
            currentVertices[i] = Vector3.LerpUnclamped(
                sourceVertices[i], targetVertices[i], morphProgress
            );
        }
        mesh.vertices = currentVertices;
    }

    /// Creates a mesh with a fixed number of triangles.
    public static Mesh CreateMesh(Vector3[] vertices) {
        Mesh mesh = new Mesh();
        int centerPoint = outerPointCount;
        int[] triangles = new int[(outerPointCount + 1) * 3];
        for(int i = 0; i < outerPointCount; ++i) {
            triangles[i * 3 + 0] = (i + 1) % outerPointCount;
            triangles[i * 3 + 1] = i % outerPointCount;
            triangles[i * 3 + 2] = centerPoint;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.MarkDynamic();
        return mesh;
    }

    /// Create a mesh consisting of outerPointCount+1 vertices.
    /// The shapeVertices describe the outside of the polygon.
    public static Vector3[] CreateMeshPoints(Vector3[] shapeVertices) {
        Vector3[] result = new Vector3[outerPointCount + 1];

        int resultIndex = 0;
        float verticesPerLine = (float)outerPointCount / shapeVertices.Length;

        for(int lineNumber = 0; lineNumber < shapeVertices.Length; ++lineNumber) {
            Vector3 v0 = shapeVertices[lineNumber];
            Vector3 v1 = shapeVertices[(lineNumber + 1) % shapeVertices.Length];
            int fromIndex = (int)(lineNumber * verticesPerLine);
            int toIndex = (int)((lineNumber + 1) * verticesPerLine);
            //Debug.LogFormat("Line {0}", lineNumber);
            for(int i = fromIndex; i < toIndex; ++i) {
                Assert.AreEqual(i, resultIndex);
                float t = (float)(i - fromIndex) / (toIndex - fromIndex);
                Vector3 v = Vector3.Lerp(v0, v1, t);
                //Debug.LogFormat("{0}: {1}", i, v);
                result[i] = v;
                ++resultIndex;
            }
        }
        Assert.AreEqual(outerPointCount, resultIndex);
        return result;
    }

    public static Vector3[] CreateStarPoints(float innerRadius, float outerRadius) {
        Vector3[] vertices = new Vector3[outerPointCount + 1];
        for(int i = 0; i < outerPointCount; ++i) {
            float radius = (i % 2 == 0) ? innerRadius : outerRadius;
            float angle = Mathf.PI * 2 * i / outerPointCount;
            vertices[i] = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
        }
        return vertices;
    }
}
