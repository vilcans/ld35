using UnityEngine;

public static class Shapes {

    public static Vector3[] squareVertices = ShapeGenerator.CreateMeshPoints(new Vector3[] {
        new Vector3(-.5f, -.5f, 0),
        new Vector3(.5f, -.5f, 0),
        new Vector3(.5f, .5f, 0),
        new Vector3(-.5f, .5f, 0),
    });

    public static Vector3[] circleVertices = ShapeGenerator.CreateStarPoints(.5f, .5f);
    public static Vector3[] starVertices = ShapeGenerator.CreateStarPoints(.3f, .5f);
}
