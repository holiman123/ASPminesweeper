using Microsoft.AspNetCore.Identity;
using MinesweeperModel;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Minesweeper.Site.Services
{
    public class MinesweeperService
    {
        private Dictionary<string, MinesweeperGame> games;
        private Dictionary<string, MinesweeperGame> gamesByNames;
        public Dictionary<string, string> namesByConnectionID;

        public MinesweeperService()
        {
            namesByConnectionID = new Dictionary<string, string>();
            gamesByNames = new Dictionary<string, MinesweeperGame>();
            games = new Dictionary<string, MinesweeperGame>();
            Console.WriteLine("buga");

            if (File.Exists("savedGames.json"))
                gamesByNames = JsonConvert.DeserializeObject<Dictionary<string, MinesweeperGame>>(File.ReadAllText("savedGames.json"));
        }

        public void createGame(int x, int y, int bombs, string connectionID)
        {
            if (games.ContainsKey(connectionID))
                removeGame(connectionID);
            MinesweeperGame game = new MinesweeperGame(x, y, bombs);
            games.Add(connectionID, game);
        }

        public MinesweeperGame getGame(string connectionID)
        {
            return games[connectionID];
        }

        public void removeGame(string connectionID)
        {            
            games.Remove(connectionID);
        }

        public void saveGame(string connectionID, string name)
        {
            gamesByNames[name] = games[connectionID];
            string serializedStr = JsonConvert.SerializeObject(gamesByNames);
            File.WriteAllText("savedGames.json", serializedStr);
        }

        public MinesweeperGame loadGame(string name, string connectionID)
        {
            games[connectionID] = gamesByNames[name];
            return gamesByNames[name];
        }
    }
}
