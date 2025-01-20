using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

class LineClearHandler
{
    public static Tile[,] Handle(Tile[,] grid, List<int> fullRows)
    {
        
        foreach(int row in fullRows) {
            // Debug.Log($"Cleared row: {row}");
            // Remove cleared rows
            for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
            {
                grid[row, col].State = TileState.FREE;
                grid[row, col].TileColor = TileColor.WHITE;
            }

            // 1. Mark segments
            // Debug.Log("Marking segments");
            List<List<(int, int)>> allSegments = new();
            MarkSegments(grid, allSegments);

            // 2. Shift down rows to fill the gaps created by removed rows  
            foreach(List<(int, int)> segment in allSegments) {
                grid = MoveSegmentDown(grid, segment);
            }   

        }
        
        return grid;

    }

    public static List<int> ScanFullRows(Tile[,] grid) {
        // Scan for full rows
        bool isLineClear = true;
        List<int> fullRows = new();

        for (int row = 0; row < GameGrid.GRID_HEIGHT; row++)
        {   
            // Go through each tile in this row. If even a single tile is not 'placed' then skip
            for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
            {
                if (grid[row, col].State != TileState.PLACED)
                {
                    isLineClear = false;
                    break;
                }
                else
                    isLineClear = true;
            }

            // If all tiles in the row are placed, then append the row to fullRows
            if (isLineClear)
            {   
                fullRows.Add(row);
            }
        }

        return fullRows;
    }

    public static Tile[,] MoveSegmentDown(Tile[,] currentGrid, List<(int, int)> segment)
    {
        // TODO: Test and debug later
        Tile[,] tempGrid = (Tile[,])currentGrid.Clone();
        List<(int, int, TileColor)> validCoords = new();

        // 1. Remove the blocks in current pos in temp grid
        foreach((int, int) block in segment) {
            int row = block.Item1;
            int col = block.Item2;
            Debug.Log($"FREEING {row}, {col}");
            tempGrid[row, col].State = TileState.FREE;
            tempGrid[row, col].TileColor = TileColor.WHITE;

            validCoords.Add((row, col, TileColor.WHITE)); // Fallback position
        }

        // 2. Move down by 1 block each iteration
        List<(int, int, TileColor)> lastCoords = new(); // last valid coords
        for(int moveY=0; moveY<GameGrid.GRID_HEIGHT; moveY++) {
            List<(int, int, TileColor)> newCoords = new();
            bool isBlocked = false;

            foreach((int, int) block in segment) {
                int row = block.Item1;
                int col = block.Item2;

                int bx = col;
                int by = row + moveY;
                TileColor color = currentGrid[row, col].TileColor; // TODO: It seems to work now...
                
                if (by > GameGrid.GRID_HEIGHT - 1) 
                    isBlocked = true;
                else 
                    if (tempGrid[by, bx].State == TileState.PLACED)
                        isBlocked = true;

                if (!isBlocked) {
                    Debug.Log($"MoveY: {moveY}, {by}, {bx}");
                    newCoords.Add((by, bx, color));
                    // Debug.Log($"Not blocked {row} {col}");
                }
                else {
                    Debug.Log($"Blocked {row} {col}");
                    break;
                }
            }

            if(isBlocked) {
                Debug.Log("Blocked. Copying last valid coords");
                validCoords = new List<(int, int, TileColor)>(lastCoords);
                break;
            }
            else {
                Debug.Log("Not blocked. Copying to lastCoords");
                lastCoords = new List<(int, int, TileColor)>(newCoords);
                validCoords = new List<(int, int, TileColor)>(lastCoords);
            }
        }
        
        // Place blocks in their new position, if any
        foreach((int, int, TileColor) coord in validCoords) {
            int row = coord.Item1;
            int col = coord.Item2;
            TileColor color = coord.Item3;
            Debug.Log($"NEW {row}, {col}");
            tempGrid[row, col].State = TileState.PLACED;
            tempGrid[row, col].TileColor = color;
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
        
        if (grid[row, col].State == TileState.PLACED)
            segment.Add((row, col));
        else
            return;

        // -- Recursive flood fill
        SearchAdjacent(grid, row, col - 1, segment); // Search left
        SearchAdjacent(grid, row, col + 1, segment); // Search right
        SearchAdjacent(grid, row - 1, col, segment); // Search up
        SearchAdjacent(grid, row + 1, col, segment); // Search down
    }
}