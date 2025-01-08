using UnityEngine;
using UnityEngine.Animations;

public enum Rotation
{
    LEFT, RIGHT, TOP, BOTTOM
}

public abstract class Tetromino
{
    public Rotation rotation = Rotation.LEFT;
    public Vector2 position = new Vector2(5, 5); // TODO: Test

    public TileState[,] Move(TileState[,] currentGrid, int axisHori, int axisVerti)
    {
        /* 
        Purpose: Check if tetro is not blocked, if not then return the new grid
        */
        int newXPos = (int)position.x + axisHori;
        int newYPos = (int)position.y + axisVerti;
        bool isBlocked = false;
        TileState[,] newGrid = (TileState[,])currentGrid.Clone();
        int[,] blocks = GetBlocks();

        for (int row = 0; row < 4; row++)
        {
            int bx = blocks[row, 0];
            int by = blocks[row, 1];

            // -- Check 1: Check if block in block in new pos is within the grid
            if (
                newXPos + bx < 0 || newXPos + bx > GameGrid.GRID_WIDTH - 1 ||
                newYPos + by < 0 || newYPos + by > GameGrid.GRID_HEIGHT - 1
            )
            {
                isBlocked = true;
                break;
            }

            newGrid[newYPos + by, newXPos + bx] = TileState.BLOCK;
        }

        if (isBlocked) return currentGrid;
        else
            position = new Vector2(newXPos, newYPos); 
            return (TileState[,])newGrid.Clone();
    }

    abstract public int[,] GetBlocks();
}

public class TetroT : Tetromino
{
    public TetroT() { }

    public override int[,] GetBlocks()
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