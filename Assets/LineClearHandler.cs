using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

class LineClearHandler
{
    public static Tile[,] Handle(Tile[,] grid)
    {
        // DONE: Check for cleared rows
        // DONE: Shift down the rows above the cleared rows
        // Optimization idea: Check only from the topmost row to the bottom row
        // Optimization idea: Run only when there is input

        // Scan for cleared rows
        const int MAX_PASSES = 3; // to check for cascading line clears
        int pass = 0;

        while(pass < MAX_PASSES) {
            bool isLineClear = true;
            for (int row = 0; row < GameGrid.GRID_HEIGHT; row++)
            {
                for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
                {
                    if (grid[row, col].State != TileState.PLACED)
                    {
                        isLineClear = false;
                        break;
                    }
                    else
                    {
                        isLineClear = true;
                    }
                }

                if (isLineClear)
                {
                    Debug.Log($"Cleared row: {row}");
                    // Remove cleared rows
                    for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
                    {
                        grid[row, col].State = TileState.FREE;
                        grid[row, col].Color = Color.black;
                    }

                     // 1. Mark segments
                    Debug.Log("Marking segments");
                    List<List<(int, int)>> allSegments = new();
                    MarkSegments(grid, allSegments);

                    // 2. Shift down rows to fill the gaps created by removed rows  
                    foreach(List<(int, int)> segment in allSegments) {
                        grid = MoveSegmentDown(grid, segment);
                    }   
                }
            }
            pass++;
        }
        
        return grid;
    }

    public static Tile[,] MoveSegmentDown(Tile[,] currentGrid, List<(int, int)> segment)
    {
        // TODO: Test and debug later
        Tile[,] tempGrid = (Tile[,])currentGrid.Clone();
        List<(int, int, Color)> validCoords = new();

        // 1. Remove the blocks in current pos in temp grid
        foreach((int, int) block in segment) {
            int row = block.Item1;
            int col = block.Item2;
            // Debug.Log($"FREEING {row}, {col}");
            tempGrid[row, col].State = TileState.FREE;
            tempGrid[row, col].Color = Color.white;

            validCoords.Add((row, col, Color.white)); // Fallback position
        }

        // 2. Move down by 1 block each iteration
        List<(int, int, Color)> lastCoords = new(); // last valid coords
        for(int moveY=0; moveY<GameGrid.GRID_HEIGHT; moveY++) {
            List<(int, int, Color)> newCoords = new();
            bool isBlocked = false;

            foreach((int, int) block in segment) {
                int row = block.Item1;
                int col = block.Item2;

                int bx = col;
                int by = row + moveY;
                Color color = currentGrid[row, col].Color; // TODO: It seems to work now...
                
                if (by > GameGrid.GRID_HEIGHT - 1) 
                    isBlocked = true;
                else 
                    if (tempGrid[by, bx].State == TileState.PLACED)
                        isBlocked = true;

                if (!isBlocked)
                    newCoords.Add((by, bx, color));
                else
                    break;
            }

            if(isBlocked) {
                Debug.Log("Blocked. Copying last valid coords");
                validCoords = new List<(int, int, Color)>(lastCoords);
                break;
            }
            else 
                lastCoords = new List<(int, int, Color)>(newCoords);
        }
        
        // Place blocks in their new position, if any
        foreach((int, int, Color) coord in validCoords) {
            int row = coord.Item1;
            int col = coord.Item2;
            Color color = coord.Item3;
            // Debug.Log($"NEW {row}, {col}");
            tempGrid[row, col] = new Tile(TileState.PLACED, color);
        }   
        
        currentGrid = (Tile[,]) tempGrid.Clone();
        return currentGrid;
    }

    static void MarkSegments(Tile[,] grid, List<List<(int, int)>> allSegments)
    {
        // -- Search for segments
        for (int row = 0; row < GameGrid.GRID_HEIGHT; row++)
        {
            for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
            {
                // Skip this tile if it is already in all_segments
                bool doSkip = false;
                foreach (List<(int, int)> seg in allSegments)
                {
                    if (seg.Contains((row, col)))
                    {
                        doSkip = true;
                        break;
                    }

                }

                if (!doSkip)
                {
                    // Do recursive search for interconnected blocks
                    List<(int, int)> segment = new();
                    SearchAdjacent(grid, row, col, segment);

                    if (segment.Count > 0)
                        allSegments.Add(segment);
                }
            }
        }


        // TODO: Just for debugging purposes
        // int segCount = 0;
        // foreach (List<(int, int)> seg in allSegments)
        // {
        //     Debug.Log($"Segment {segCount}");
        //     foreach ((int, int) s in seg)
        //     {
        //         Debug.Log($"{s}");
        //     }

        //     segCount++;
        // }
    }

    static void SearchAdjacent(Tile[,] grid, int row, int col, List<(int, int)> segment)
    {
        bool visited = segment.Contains((row, col));

        // Lazy fix
        try
        {
            if (grid[row, col].State == TileState.FREE || visited)
                return;
        }
        catch (IndexOutOfRangeException e)
        {
            return;
        }

        segment.Add((row, col));

        // -- Recursive flood fill
        SearchAdjacent(grid, row, col - 1, segment); // Search left
        SearchAdjacent(grid, row, col + 1, segment); // Search right
        SearchAdjacent(grid, row - 1, col, segment); // Search up
        SearchAdjacent(grid, row + 1, col, segment); // Search down
    }
}