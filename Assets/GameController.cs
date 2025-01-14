using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    GameGrid gameGrid;
    GameInput gameInput;
    Tetromino tetro;
    Tetromino[] dbgTetros;
    int dbgIndex;
    
    void Start()
    {
        GameObject obj = new GameObject("GameGridObject");
        gameGrid = obj.AddComponent<GameGrid>();

        dbgIndex = 2;
        dbgTetros = new Tetromino[] {
            new TetroTango(),
            new TetroOscar(),
            new TetroIndia(),
            new TetroJuliett(),
            new TetroLima(),
            new TetroSierra(),
            new TetroZulu()
        };
        tetro = dbgTetros[dbgIndex];
        gameInput = new();
    }

    void Update()
    {
        gameInput.GetInput(gameGrid, tetro);
        gameInput.PlaceTetro(gameGrid, tetro);
        // TODO: Add condition to only run when necessary
        // README: It seems that adding an axisVerti != 0 condition forces the tetro to 'hard drop'. Weird.
        gameGrid.Grid = LineClearHandler.Handle(gameGrid.Grid); 
        DrawGrid();
    }

    void DrawGrid()
    {
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                Tile tile = gameGrid.Grid[y, x];
                switch (tile.State)
                {
                    case TileState.FREE:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = Color.white;
                        break;
                    case TileState.BLOCK:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = tile.Color;
                        break;
                    case TileState.PLACED:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = tile.Color;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
