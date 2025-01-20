using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameControllerState {
    PLAYING, LINECLEAR_ANIMATION, GAME_OVER
}

public class GameController : MonoBehaviour
{
    GameGrid gameGrid;
    GameInputNew gameInput;
    Tetromino tetro;
    Tetromino[] myTetros;
    int index;
    GameControllerState myState;
    PlayerInfo pInfo;
    public List<Sprite> tiles; // TODO: test
    public GameObject textGameOver;
    public GameObject textScore;
    public GameObject textLevel;
    public GameObject textProgress;
    public List<Image> tetQueue;
    public List<Sprite> tetQueueSprites;
    List<int> currentBag;
    List<int> nextBag;
    List<int> WeightTable;

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
        
        WeightTable = new List<int>{
            0, 0,       // T tetro
            1, 1,       // O tetro
            2,       // I tetro
            3, 3, 3,    // J tetro
            4, 4, 4,    // L tetro
            5, 5, 5,    // S tetro
            6, 6, 6,    // Z tetro
        };
        nextBag = WeightTable.ToList();
        nextBag.Shuffle();
        currentBag = nextBag.Skip(0).Take(6).ToList();
        index = 0;
        
        myTetros = new Tetromino[] {
            new TetroTango(),
            new TetroOscar(),
            new TetroIndia(),
            new TetroJuliett(),
            new TetroLima(),
            new TetroSierra(),
            new TetroZulu()
        };
        tetro = myTetros[currentBag[index]];
        tetro = myTetros[3];
        gameInput = new();
        myState = GameControllerState.PLAYING;
        fullRows = new();

        pInfo.Score = 0;
        pInfo.Level = GameDefines.MIN_LEVEL;
        // gameInput.GravityDelay = GameDefines.DEFAULT_GRAVITY_DELAY - (GameDefines.GRAVITY_MULTIPLICAND * pInfo.Level);

        UpdateTextDisplays();
        textGameOver.GetComponent<TextMeshProUGUI>().text = GameDefines.TXT_NONE;

        UpdateTetroQueueDisplay();
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

                    UpdateTextDisplays();

                    break;
                }
                else {
                    DrawLineClearAnimation();
                }

                break;
            case GameControllerState.GAME_OVER:
                // Debug.Log("Game over, hoss");

                if (Input.GetKeyDown(KeyCode.Space)) {
                    myState = GameControllerState.PLAYING;
                    textGameOver.GetComponent<TextMeshProUGUI>().text = GameDefines.TXT_NONE;
                    gameGrid.ResetGrid();
                    gameInput.Reset();
                    ResetTetro();

                    pInfo.Score = 0;
                    pInfo.LevelProgress = 0;
                    pInfo.Level = 0;
                    UpdateTextDisplays();
                    
                    break;
                }

                break;
            default:
                Debug.Log("You're not supposed to reach this state D:");
                break;
        }

        // Debug.Log($"LEVEL: {pInfo.Level}, Score: {pInfo.Score}, LvProg: {pInfo.LevelProgress}");
       
    }

    void DrawLineClearAnimation() {
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
                    overlayGrid[row, col].State = TileState.PLACED;
                    overlayGrid[row, col].TileColor = gameGrid.Grid[row, col].TileColor;
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
                    overlayGrid[row, col].State = TileState.PLACED;
                    overlayGrid[row, col].TileColor = TileColor.WHITE;
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
                TileColor spriteColor = grid[y, x].TileColor;

                switch (tile.State)
                {
                    case TileState.FREE:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = GameUtils.NormalizedColor(16, 19, 46);
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = null;
                        break;
                    case TileState.BLOCK:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = GameUtils.NormalizedColor(255, 255, 255);
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = MatchColors(spriteColor);

                        break;
                    case TileState.PLACED:
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().color = GameUtils.NormalizedColor(255, 255, 255);
                        gameGrid.DisplayGrid[y, x].GetComponent<Image>().sprite = MatchColors(spriteColor);
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
                return tiles[7]; // TODO
            default:
                return tiles[0]; // TODO
        }
    }

    (Sprite matchedSprite, Vector2 matchedSize) MatchQueueSprites(TileColor color) {
        // TODO: This is a bit redundant

        Sprite matchedSprite = null;
        switch(color) {
            case TileColor.RED:
                matchedSprite = tetQueueSprites[0];
                break;
            case TileColor.GREEN:
                matchedSprite = tetQueueSprites[1];
                break;
            case TileColor.BLUE:
                matchedSprite = tetQueueSprites[2];
                break;
            case TileColor.YELLOW:
                matchedSprite = tetQueueSprites[3];
                break;
            case TileColor.ORANGE:
                matchedSprite = tetQueueSprites[4];
                break;
            case TileColor.CYAN:
                matchedSprite = tetQueueSprites[5];
                break;
            case TileColor.PURPLE:
                matchedSprite = tetQueueSprites[6];
                break;
            case TileColor.WHITE:
                matchedSprite = tetQueueSprites[0]; // TODO
                break;
            default:
                matchedSprite = tetQueueSprites[0]; // TODO
                break;
        }

        Rect matchedRect = matchedSprite.textureRect;

        return (matchedSprite, new Vector2(matchedRect.width, matchedRect.height));
    }

    void OnEnable()
    {
        GameInputNew.OnTetroReset += ResetTetro;
    }

    void OnDisable()
    {
        GameInputNew.OnTetroReset -= ResetTetro;
    }

    private void ResetTetro()
    {
        Debug.Log("Update the tetro");
        index++;
        index %= currentBag.Count;

        if(index == 0) {
            nextBag = WeightTable.ToList();
            nextBag.Shuffle();
            currentBag = nextBag.Skip(0).Take(6).ToList();
        }

        tetro = myTetros[currentBag[index]];
        // tetro = myTetros[2];
        tetro.Reset();

        // Gameover check
        for (int i = 0; i < 4; i++)
        {
            int bx = (int)tetro.position.x + tetro.GetBlocks(tetro.rotation)[i, 0];
            int by = (int)tetro.position.y + tetro.GetBlocks(tetro.rotation)[i, 1];

            if (gameGrid.Grid[by, bx].State == TileState.PLACED) {
                myState = GameControllerState.GAME_OVER;
                textGameOver.GetComponent<TextMeshProUGUI>().text = GameDefines.TXT_GAMEOVER;
                break;
            }
        }

        UpdateTetroQueueDisplay();
    }

    void UpdateTetroQueueDisplay() {

        Debug.Log($"Index is {index}");

        for (int i=0; i < tetQueue.Count; i++ ) {
            int futureIndex = index+1+i;

            if(futureIndex < currentBag.Count) {
                var match = MatchQueueSprites(myTetros[currentBag[futureIndex]].myColor);
                tetQueue[i].GetComponent<Image>().sprite = match.matchedSprite;
                tetQueue[i].GetComponent<Image>().rectTransform.sizeDelta = match.matchedSize;
            }
            else {
                int nextIndex = futureIndex - currentBag.Count;
                var match = MatchQueueSprites(myTetros[nextBag[nextIndex]].myColor);
                tetQueue[i].GetComponent<Image>().sprite = match.matchedSprite;
                tetQueue[i].GetComponent<Image>().rectTransform.sizeDelta = match.matchedSize;
            }
        }
    }

    void UpdateTextDisplays() {
        textScore.GetComponent<TextMeshProUGUI>().text = $"{pInfo.Score}";
        textLevel.GetComponent<TextMeshProUGUI>().text = $"{pInfo.Level}";
        textProgress.GetComponent<TextMeshProUGUI>().text = $"{pInfo.LevelProgress}";
    }
}
