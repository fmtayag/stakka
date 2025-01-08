using UnityEngine;
using UnityEngine.Animations;

public enum Rotation
{
    NONE, LEFT, RIGHT, TOP, BOTTOM
}

public abstract class Tetromino
{
    public Rotation rotation = Rotation.LEFT;
    public Vector2 position = new Vector2(1, 5); // TODO: Test
    public void Rotate(TileState[,] currentGrid) {
        /*
        Purpose: Rotate the tetro after ensuring it will not hit a will or a placed block
        */
        // 1. Set the new rotation
        Rotation newRotation = Rotation.NONE;
        switch (rotation) {
            case Rotation.LEFT:
                newRotation = Rotation.TOP;
                break;
            case Rotation.TOP:
                newRotation = Rotation.RIGHT;
                break;
            case Rotation.RIGHT:
                newRotation = Rotation.BOTTOM;
                break;
            case Rotation.BOTTOM:
                newRotation = Rotation.LEFT;
                break;
            default:
                newRotation = Rotation.NONE;
                break;
        }
            
        int[,] rotated_blocks = GetBlocks(newRotation);
        bool isBlocked = false;
        int wallkickDirection = 0;

        // 2. Check if the rotated blocks will hit a wall or a placed block
        for (int col=0;col<4;col++) {
            int bx = (int) position[0] + rotated_blocks[col, 0];
            int by = (int) position[1] + rotated_blocks[col, 1];
            // -- Check if block in new pos will hit wall
            if (
                bx < 0 || bx > GameGrid.GRID_WIDTH-1 ||
                by < 0 || by > GameGrid.GRID_HEIGHT-1
            )
                isBlocked = true;

            // -- Check if block in new pos will hit already placed blocks
            bool within_vergrid = by > 0 && by < GameGrid.GRID_HEIGHT- 1;
            bool within_horgrid = bx > 0 && bx < GameGrid.GRID_WIDTH - 1;
            if (within_vergrid && within_horgrid) {
                if (currentGrid[by, bx] == TileState.PLACED)
                    isBlocked = true;
            }
            else {
                isBlocked = true;
                break;
            }
        }

        // -- Wall kick mechanism (Steps 3 and 4) --
        // 3. If blocked, try wall kick to the right
        if(isBlocked) {
            for(int kick=0; kick<3; kick++) {
                for (int col=0;col<4;col++) {
                    int bx = (int) position[0] + rotated_blocks[col, 0];
                    int by = (int) position[1] + rotated_blocks[col, 1];
                    int wallk_right = bx + kick;

                    // -- Check if block in rotated pos will collide with ceiling
                    if (by < 0) {
                        isBlocked = true;
                        break;
                    }
                    
                    // -- Out of bounds check
                    if (wallk_right > 0 && wallk_right <= GameGrid.GRID_WIDTH-1) {
                        // -- Check if block in rotated pos will intersect with a placed block
                        if (currentGrid[by, wallk_right] == TileState.PLACED && by > 0) {
                            wallkickDirection = 0;
                            isBlocked = true;
                            break;
                        }
                        else {
                            wallkickDirection = kick;
                            isBlocked = false;
                        }
                    }
                    else {
                        isBlocked = true;
                        break;
                    }
                }

                if (!isBlocked) break;
            }
        }

        // 4. If still blocked, try wall kick to left
        if(isBlocked) {
            for(int kick=0; kick<3; kick++) {
                for (int col=0;col<4;col++) {
                    int bx = (int) position[0] + rotated_blocks[col, 0];
                    int by = (int) position[1] + rotated_blocks[col, 1];
                    int wallk_left = bx - kick;

                    // -- Check if block in rotated pos will collide with ceiling
                    if (by < 0) {
                        isBlocked = true;
                        break;
                    }

                    // -- Out of bounds check
                    if (wallk_left > 0 && wallk_left <= GameGrid.GRID_WIDTH-1) 
                    {
                        // -- Check if block in rotated pos will intersect with a placed block
                        if (currentGrid[by, wallk_left] == TileState.PLACED) {
                            wallkickDirection = 0;
                            isBlocked = true;
                            break;
                        }
                        else {
                            wallkickDirection = -kick;
                            isBlocked = false;
                        }
                    }
                    else {
                        wallkickDirection = 0;
                        isBlocked = true; 
                        break;
                    }
                }

                if (!isBlocked) break;
            }
        }

        // 5. Set current rotation to new rotation if all is good
        if(!isBlocked) {
            Debug.Log("Not blocked. Rotating.");

            // -- "Delete" the blocks in the old position
            for (int col=0;col<4;col++) {
                int bx = (int) position.x + GetBlocks(rotation)[col, 0];
                int by = (int) position.y + GetBlocks(rotation)[col, 1];

                currentGrid[by, bx] = TileState.FREE;
            }
            // -- Apply the new rotation
            rotation = newRotation;

            // -- Apply wallkick, if any
            if(wallkickDirection != 0) position.x += wallkickDirection; 
        }
    }
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
        int[,] blocks = GetBlocks(rotation);

        Debug.Log(rotation);

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
            // Check 2: Check if block in new pos will hit already placed blocks
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

    abstract public int[,] GetBlocks(Rotation on_rotation);
}

public class TetroT : Tetromino
{
    public override int[,] GetBlocks(Rotation on_rotation)
    {
        switch (on_rotation)
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
                    {1, 0},
                    {0, -1},
                    {0, 1}
                };
            case Rotation.TOP:
                return new int[,] {
                    {0, 0},
                    {0, 1},
                    {1, 1},
                    {-1, 1}
                };
            case Rotation.BOTTOM:
                return new int[,] {
                    {0, 0},
                    {0, 1},
                    {1, 0},
                    {-1, 0}
                };
            default:
                return new int[0, 0];
        }
    }
}