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
        int currentXPos = (int)position.x;
        int currentYPos = (int)position.y;
        int newXPos = currentXPos + axisHori;
        int newYPos = currentYPos + axisVerti;
        bool isBlocked = false;
        TileState[,] newGrid = (TileState[,])currentGrid.Clone();
        int[,] blocks = GetBlocks();

        // 1. Remove the blocks in current position
        for(int row=0; row<4; row++) {
            int bx = blocks[row, 0];
            int by = blocks[row, 1];

            // TODO: Add check if newGrid tile is already PLACED??
            newGrid[currentYPos+by, currentXPos+bx] = TileState.FREE;
        }

        // 2. Perform checks for isBlocked
        for (int row = 0; row < 4; row++)
        {
            int bx = blocks[row, 0];
            int by = blocks[row, 1];

            // -- Check 1: Check if block in new pos is within the grid
            if (
                newXPos + bx < 0 || newXPos + bx > GameGrid.GRID_WIDTH - 1 ||
                newYPos + by < 0 || newYPos + by > GameGrid.GRID_HEIGHT - 1
            )
            {
                isBlocked = true;
                break;
            }
            // TODO: Check 2: Check if block in new pos will hit already placed blocks
            else {
                if (newGrid[newYPos + by, newXPos + bx] == TileState.PLACED) {
                    isBlocked = true;
                    break;
                }
            }

            newGrid[newYPos + by, newXPos + bx] = TileState.BLOCK;
        }

        // 3. Return the grid
        if (isBlocked) return currentGrid;
        else
        {   
            // Set the new position
            position = new Vector2(newXPos, newYPos); 
            return (TileState[,])newGrid.Clone();
        }
            
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