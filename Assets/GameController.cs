using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    PlayerInfo pInfo;
    public List<Sprite> tiles; // TODO: test

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

        dbgIndex = 3;
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

        pInfo.Score = 0;
        pInfo.Level = GameDefines.MIN_LEVEL;
        // gameInput.GravityDelay = GameDefines.DEFAULT_GRAVITY_DELAY - (GameDefines.GRAVITY_MULTIPLICAND * pInfo.Level);

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
                UpdateDisplayGrid(gameGrid.Grid);

                break;
            case GameControllerState.LINECLEAR_ANIMATION:
                if (Time.time - timerAnim0 >= delayAnim0) {
                    myState = GameControllerState.PLAYING;
                    gameGrid.Grid = LineClearHandler.Handle(gameGrid.Grid, fullRows);

                    // Add score
                    int rowsCleared = fullRows.Count;
                    pInfo.Score += pInfo.Level * rowsCleared * GameDefines.SCORE_PER_ROW_CLEARED;
                    pInfo.DoLevelUp(rowsCleared);

                    // Update gravity delay
                    gameInput.GravityDelay = GameDefines.DEFAULT_GRAVITY_DELAY - (GameDefines.GRAVITY_MULTIPLICAND * pInfo.Level);

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

        // Debug.Log($"LEVEL: {pInfo.Level}, Score: {pInfo.Score}, LvProg: {pInfo.LevelProgress}");
       
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
                    overlayGrid[row, col].State = gameGrid.Grid[row, col].State;
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
                    overlayGrid[row, col].State = TileState.FREE;
                }
            }
        }

        UpdateDisplayGrid(overlayGrid);
    }

    void UpdateDisplayGrid(Tile[,] grid)
    {
        for (int y = 0; y < GameGrid.GRID_HEIGHT; y++)
        {
            for (int x = 0; x < GameGrid.GRID_WIDTH; x++)
            {
                Tile tile = grid[y, x];
                TileColor color = gameGrid.Grid[y, x].Color;

                switch (tile.State)
                {
                    case TileState.FREE:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = null;
                        break;
                    case TileState.BLOCK:
                        // gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = tile.Color;
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = MatchColors(color);

                        break;
                    case TileState.PLACED:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = MatchColors(color);
                        
                        
                        break;
                    default:
                        break;
                }
            }
        }
    }

    Sprite MatchColors(TileColor color) {

        switch(color) {
            case TileColor.RED:
                return tiles[0];
            case TileColor.GREEN:
                return tiles[1];
            case TileColor.BLUE:
                return tiles[2];
            case TileColor.YELLOW:
                return tiles[3];
            case TileColor.ORANGE:
                return tiles[4];
            case TileColor.CYAN:
                return tiles[5];
            case TileColor.PURPLE:
                return tiles[6];
            case TileColor.WHITE:
                return null; // TODO
            default:
                return tiles[0]; // TODO
        }
    }
}
