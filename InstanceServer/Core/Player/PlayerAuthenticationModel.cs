namespace InstanceServer.Core.Player
{
    public struct PlayerAuthenticationModel
    {
        public string SessionID;
        public string UserID;
        public string UserName;

        public PlayerAuthenticationModel(string sessionID, string userID, string userName)
        {
            SessionID = sessionID;
            UserID = userID;
            UserName = userName;
        }
    }
}
