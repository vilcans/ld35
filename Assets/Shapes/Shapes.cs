using UnityEngine;

public static class Shapes {

    public static Vector3[] squareVertices = ShapeGenerator.CreateMeshPoints(new Vector3[] {
        new Vector3(.5f, 0, 0),  // right
        new Vector3(.5f, .5f, 0),  // top right
        new Vector3(.0f, .5f, 0),  // top
        new Vector3(-.5f, .5f, 0),  // top left
        new Vector3(-.5f, .0f, 0),  // left
        new Vector3(-.5f, -.5f, 0),  // bottom left
        new Vector3(-.0f, -.5f, 0),  // bottom
        new Vector3(.5f, -.5f, 0),  // bottom right
    });

    public static Vector3[] circleVertices = ShapeGenerator.CreateStarPoints(.5f, .5f);
    public static Vector3[] starVertices = ShapeGenerator.CreateStarPoints(.3f, .5f);
    public static Vector3[] heartVertices = ShapeGenerator.CreateStarPoints(.1f, 1.3f);
}
