namespace BFME2.Core
{
    public enum FactionId
    {
        Gondor,
        Mordor
    }

    public enum GameState
    {
        Loading,
        Initializing,
        Playing,
        Paused,
        GameOver
    }

    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard,
        Brutal
    }

    public enum SelectableType
    {
        Unit,
        Hero,
        Building
    }

    public enum BuildingPlacementType
    {
        BuildPlot,
        FreePlacement,
        Wall
    }

    public enum BuildPlotSize
    {
        Small,
        Medium,
        Large
    }

    public enum BuildingState
    {
        Constructing,
        Active,
        Damaged,
        Destroyed
    }

    public enum AbilityTargetType
    {
        Self,
        SingleTarget,
        Area,
        Passive
    }

    public enum PowerTargetType
    {
        Global,
        Area,
        Unit
    }

    public enum WallSegmentType
    {
        Wall,
        Tower,
        Gate
    }

    public enum MinimapIconType
    {
        Unit,
        Hero,
        Building,
        Resource
    }

    public enum UIPanel
    {
        HUD,
        BuildMenu,
        PowerTree,
        PauseMenu,
        GameOver
    }
}
