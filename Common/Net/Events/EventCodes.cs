namespace Common.Net.Events
{
    public static class EventCodes
    {
        public const byte PLAYERS_JOINED_LOBBY =    0;
        public const byte PLAYER_READY =            1;
        public const byte PLAYER_UNREADY =          2;
        public const byte PLAYER_DISCONNECTED =     3;
        public const byte LOAD_WORLD =              4;
        public const byte PLAYERS_JOINED_WORLD =    5;
        public const byte PLAYERS_JOINED_ZONE =     6;
        public const byte ZONE_STARTED =            7;
        public const byte COMBAT_TURN =             8;
        public const byte ZONE_COMPLETED =          9;
        public const byte GAME_COMPLETED =          10;
    }
}
