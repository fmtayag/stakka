using UnityEngine;

public struct PlayerInfo {
    public int Score { get; set; }
    public int Level { get; set; }
    public int LevelProgress { get; set; }

    public void DoLevelUp(int rowsCleared) {
        LevelProgress += rowsCleared;
        if (LevelProgress >= GameDefines.ROWS_NEEDED_FOR_LEVEL_ADVANCE) {
            Debug.Log("Leveled up");
            Level += 1;
            Level = Mathf.Clamp(Level, GameDefines.MIN_LEVEL, GameDefines.MAX_LEVEL);

            // TODO: Roll-over excess rowsCleared
            int rollover = (LevelProgress + rowsCleared) % GameDefines.ROWS_NEEDED_FOR_LEVEL_ADVANCE;
            LevelProgress = rollover;
        }
    }
}