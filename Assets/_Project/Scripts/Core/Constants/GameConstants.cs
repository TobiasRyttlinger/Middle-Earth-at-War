namespace BFME2.Core
{
    public static class GameConstants
    {
        // Layer Names
        public const string LAYER_TERRAIN = "Terrain";
        public const string LAYER_UNIT = "Unit";
        public const string LAYER_BUILDING = "Building";
        public const string LAYER_BUILD_PLOT = "BuildPlot";
        public const string LAYER_HERO = "Hero";
        public const string LAYER_WALL = "Wall";
        public const string LAYER_RESOURCE = "Resource";
        public const string LAYER_FOG_OF_WAR = "FogOfWar";
        public const string LAYER_MINIMAP = "Minimap";
        public const string LAYER_PROJECTILE = "Projectile";
        public const string LAYER_SELECTION_PLANE = "SelectionPlane";
        public const string LAYER_OBSTACLE = "Obstacle";

        // Layer Indices
        public const int LAYER_INDEX_TERRAIN = 6;
        public const int LAYER_INDEX_UNIT = 7;
        public const int LAYER_INDEX_BUILDING = 8;
        public const int LAYER_INDEX_BUILD_PLOT = 9;
        public const int LAYER_INDEX_HERO = 10;
        public const int LAYER_INDEX_WALL = 11;
        public const int LAYER_INDEX_RESOURCE = 12;
        public const int LAYER_INDEX_FOG_OF_WAR = 13;
        public const int LAYER_INDEX_MINIMAP = 14;
        public const int LAYER_INDEX_PROJECTILE = 15;

        // Tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_NEUTRAL = "Neutral";
        public const string TAG_FORTRESS = "Fortress";
        public const string TAG_BUILD_PLOT = "BuildPlot";
        public const string TAG_RESOURCE_NODE = "ResourceNode";
        public const string TAG_GATE = "Gate";
        public const string TAG_RALLY_POINT = "RallyPoint";

        // Selection LayerMask (units + heroes + buildings + build plots + resource nodes + walls)
        public static readonly int SelectableLayerMask =
            (1 << LAYER_INDEX_UNIT) |
            (1 << LAYER_INDEX_HERO) |
            (1 << LAYER_INDEX_BUILDING) |
            (1 << LAYER_INDEX_BUILD_PLOT) |
            (1 << LAYER_INDEX_RESOURCE) |
            (1 << LAYER_INDEX_WALL);

        public static readonly int TerrainLayerMask = (1 << LAYER_INDEX_TERRAIN);

        public static readonly int AttackableLayerMask =
            (1 << LAYER_INDEX_UNIT) |
            (1 << LAYER_INDEX_HERO) |
            (1 << LAYER_INDEX_BUILDING) |
            (1 << LAYER_INDEX_WALL);

        // Game Tuning
        public const float RESOURCE_TICK_INTERVAL = 6f;
        public const float RESOURCE_LEVEL_2_MULTIPLIER = 1.2f;
        public const float RESOURCE_LEVEL_3_MULTIPLIER = 1.35f;
        public const int DEFAULT_STARTING_RESOURCES = 2000;
        public const int DEFAULT_MAX_COMMAND_POINTS = 300;
        public const float UNIT_AUTO_ACQUIRE_RANGE = 15f;
        public const float BUILDING_SELL_REFUND_PERCENT = 0.5f;
        public const float HERO_REVIVE_BASE_TIME = 60f;

        // Physics
        public const float GROUND_RAYCAST_DISTANCE = 1000f;
    }
}
