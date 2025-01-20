using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;

class GameInputNew {
    public static event Action OnTetroReset;

    Vector2 velocity = Vector2.zero;
    int currentDirection = 0;
    float keyLeftTimer  = 0f;
    float keyRightTimer  = 0f;
    float keyDownTimer = 0f;
    const float keyHoldDelay = 0.3f;
    const float keyRepeatRate = 0.25f;
    const float keyDownHoldDelay = 0.2f;
    const float keyDownRepeatRate = 0.15f;
    float gravityTimer;
    private float _gravityDelay;
    public float GravityDelay
    {
        get
        {
            return _gravityDelay;
        }
        set
        {
            _gravityDelay = Mathf.Clamp(value, GameDefines.MIN_GRAVITY_DELAY, GameDefines.DEFAULT_GRAVITY_DELAY);
        }
    }
    bool canLock = false;
    bool doHardDrop = false;

    public GameInputNew () {
        Reset();
    }

    public void Reset() {
        gravityTimer = Time.time;
        GravityDelay = GameDefines.DEFAULT_GRAVITY_DELAY;
    }

    public void GetInput(GameGrid gameGrid, Tetromino tetro)
    {
        velocity = Vector2.zero;
        float now = Time.time;
        canLock = false;

        // -- Horizontal movement
        // ---- LEFT
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Pressed 'Left' key");
            velocity.x = -1;
            currentDirection = -1;
            keyLeftTimer = now;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && currentDirection == -1)
        {
            Debug.Log(now - keyLeftTimer);
            if (now - keyLeftTimer >= keyHoldDelay)
            {
                velocity.x = -1;
                keyLeftTimer = now - keyRepeatRate;
                // Debug.Log($"Now: {now} KeyLeftTimer: {keyLeftTimer}");
            }
        }

        // ---- RIGHT
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Pressed 'Right' key");
            velocity.x = 1;
            currentDirection = 1;
            keyRightTimer = now;
        }
        if (Input.GetKey(KeyCode.RightArrow) && currentDirection == 1)
        {
            Debug.Log(now - keyRightTimer);
            if (now - keyRightTimer >= keyHoldDelay)
            {
                velocity.x = 1;
                keyRightTimer = now - keyRepeatRate;
                // Debug.Log($"Now: {now} KeyRightTimer: {keyRightTimer}");
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) && currentDirection == -1) {
            Debug.Log("Key up Left!");
            currentDirection = 1;
            keyRightTimer = now;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow) && currentDirection == 1) {
            Debug.Log("Key up Right!");
            currentDirection = -1;
            keyLeftTimer = now;
        }

        // -- Vertical movement
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log("Pressed 'Down' key");
            velocity.y = 1;
            keyDownTimer = now;
            canLock = true;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            if (now - keyDownTimer >= keyDownHoldDelay)
            {
                velocity.y = 1;
                keyDownTimer = now - keyDownRepeatRate;
                // gravityTimer -= 0.1f; // NOTE: Not sure about this one's necessity hmm
                // canLock = true;
            }
        }

        // -- Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Pressed 'Rotate' key");
            tetro.Rotate(gameGrid.Grid);
        }

        // -- Hard drop
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Pressed 'Hard Drop' key");
            doHardDrop = true;
        }

    }

    public void PlaceTetro(GameGrid gameGrid, Tetromino tetro)
    {
        // Automatically go down after some specified time
        float now = Time.time;
        
        // bool lockedByInput = canLock;
        if (now - gravityTimer >= GravityDelay)
        {
            velocity.y = 1; 
            gravityTimer = now;
            canLock = true;

            var values = tetro.LockTetro(gameGrid.Grid, velocity, canLock);
            gameGrid.Grid = values.grid;
            bool doReset = values.doReset;

            if(doReset) {
                ResetTetro();
            }
        }

        if (doHardDrop) {
            Debug.Log("Doing hard drop");
            DoHardDrop(gameGrid, tetro);
            doHardDrop = false;
            ResetTetro();
        }
        else {
            // We have to separate Move() and DoHardDrop() since they both have their own 'lock the tetro' logic
            // Doing otherwise would lead to "ghost" blocks
            gameGrid.Grid = tetro.Move(gameGrid.Grid, velocity);
        }



        // if(velocity != Vector2.zero) {
        //     // gameGrid.Grid = tetro.Move(gameGrid.Grid, velocity);
        //     // gameGrid.Grid = tetro.LockTetro(gameGrid.Grid, velocity, canLock);
        // }
        
        // if (a|| isHardDropping)
        // {
        //     if (!isHardDropping) 
        //         gameGrid.Grid = tetro.Move(gameGrid.Grid, velocity, canLock);
        //     else
        //     {
        //         Debug.Log("Doing hard drop");
        //         DoHardDrop(gameGrid, tetro);
        //     }
        // }
        // else
        // {
        //     int[,] blocks = tetro.GetBlocks(tetro.rotation);

        //     for (int row = 0; row < 4; row++)
        //     {
        //         int bx = blocks[row, 0];
        //         int by = blocks[row, 1];
        //         gameGrid.Grid[(int)(tetro.position.y + by), (int)(tetro.position.x + bx)] = new Tile(TileState.BLOCK, tetro.myColor);
        //     }
        // }
    }

    void ResetTetro() {
        OnTetroReset?.Invoke();
    }

    void DoHardDrop(GameGrid gameGrid, Tetromino tetro) {
        // Convert tetro into a "segment"
        List<(int, int)> myTetro = new List<(int, int)>();
        for (int i = 0; i < 4; i++)
        {
            int bx = (int)tetro.position.x + tetro.GetBlocks(tetro.rotation)[i, 0];
            int by = (int)tetro.position.y + tetro.GetBlocks(tetro.rotation)[i, 1];

            myTetro.Add((by, bx));
        }

        // We're gonna reuse the MoveSegmentDown from the line clear handler since it does the same thing anyway
        gameGrid.Grid = LineClearHandler.MoveSegmentDown(gameGrid.Grid, myTetro);
    }
}