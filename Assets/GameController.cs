using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameControllerState {
    PLAYING, LINECLEAR_ANIMATION
}

public class GameController : MonoBehaviour
{
    GameGrid gameGrid;
    GameInput gameInput;
    Tetromino tetro;
    Tetromino[] dbgTetros;
    int dbgIndex;
    GameControllerState myState;

    // For line clearing animation
    List<int> fullRows;
    Tile[,] overlayGrid = new Tile[20, 10]; 
    float timerAnim0 = 0f;
    float delayAnim0 = 0.8f;
    float timerAnim1 = 0f;
    float delayAnim1 = 0.2f;
    float timerAnim2 = 0f;
    float delayAnim2 = 0.2f;
    
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
        myState = GameControllerState.PLAYING;
        fullRows = new();
    }

    void Update()
    {

        switch(myState) {
            case GameControllerState.PLAYING:
                gameInput.GetInput(gameGrid, tetro);
                gameInput.PlaceTetro(gameGrid, tetro);
                
                fullRows = LineClearHandler.ScanFullRows(gameGrid.Grid); // TODO: Add condition to only run this when necessary
                bool hasRows = fullRows.Count != 0;
                
                if (hasRows) {
                    Debug.Log("Transition to 'line clear animation' state ");
                    myState = GameControllerState.LINECLEAR_ANIMATION;
                    overlayGrid = (Tile[,]) gameGrid.Grid.Clone();
                    timerAnim1 = Time.time;
                    timerAnim0 = Time.time;
                    break;
                }

                // gameGrid.Grid = LineClearHandler.Handle(gameGrid.Grid, fullRows);
                DrawGrid(gameGrid.Grid);

                break;
            case GameControllerState.LINECLEAR_ANIMATION:
                if (Time.time - timerAnim0 >= delayAnim0) {
                    myState = GameControllerState.PLAYING;
                    gameGrid.Grid = LineClearHandler.Handle(gameGrid.Grid, fullRows);
                    break;
                }
                else {
                    DrawLineClearAnimation();
                }

                break;
            default:
                Debug.Log("You're not supposed to reach this state D:");
                break;
        }
       
    }

    void DrawLineClearAnimation() {
        // TODO: Do line clear animation here
        float now = Time.time;

        // HACK: This is fucking voodoo so don't even try to wrap your head around it
        // Just know that it switches the blinkState after a specified amount of time
        int blinkState;
        if(now - timerAnim1 >= delayAnim1) {
            if(now - timerAnim2 >= delayAnim2) {
                timerAnim1 = now;
            }
            
            blinkState = 1;
        }
        else {
            blinkState = 0;
            timerAnim2 = now;
        }

        if (blinkState == 0)
        {
            // Original color
            foreach (int row in fullRows)
            {
                for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
                {
                    overlayGrid[row, col].Color = gameGrid.Grid[row, col].Color;
                }
            }
        }
        else
        {
            // Pure white color
            foreach (int row in fullRows)
            {
                for (int col = 0; col < GameGrid.GRID_WIDTH; col++)
                {
                    overlayGrid[row, col].Color = Color.white;
                }
            }
        }

        DrawGrid(overlayGrid);
    }

    void DrawGrid(Tile[,] grid)
    {
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                Tile tile = grid[y, x];
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
