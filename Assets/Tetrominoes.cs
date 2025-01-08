using UnityEngine;

public enum Rotation
{
    LEFT, RIGHT, TOP, BOTTOM
}

public class TetroT
{
    public Rotation rotation = Rotation.LEFT;
    public Vector2 position = new Vector2(8, 8);
    public TetroT() { }

    public int[,] GetBlocks()
    {
        switch (rotation)
        {
            case Rotation.LEFT:
                return new int[,] {
                    {0, 0},
                    {-1, 0},
                    {0, -1},
                    {0, 1}
                };
            case Rotation.RIGHT:
                return new int[,] {
                    {0, 0},
                    {-1, 0},
                    {0, -1},
                    {0, 1}
                };
            case Rotation.TOP:
                return new int[,] {
                    {0, 0},
                    {-1, 0},
                    {0, -1},
                    {0, 1}
                };
            case Rotation.BOTTOM:
                return new int[,] {
                    {0, 0},
                    {-1, 0},
                    {0, -1},
                    {0, 1}
                };
            default:
                return new int[0, 0];
        }
    }
}