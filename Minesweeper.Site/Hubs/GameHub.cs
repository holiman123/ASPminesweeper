using Minesweeper.Site.Services;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace Minesweeper.Site.Hubs
{
    public class GameHub : Hub
    {
        private MinesweeperService gameService;

        public GameHub(MinesweeperService gameService)
        {
            this.gameService = gameService;
            Console.WriteLine("ooga");
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connection ID: " + Context.ConnectionId);
            //TODO: if there is user with ID, load his field

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if(gameService.namesByConnectionID.ContainsKey(Context.ConnectionId))
                gameService.saveGame(Context.ConnectionId, gameService.namesByConnectionID[Context.ConnectionId]);

            gameService.removeGame(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task saveName(string name)
        {
            gameService.namesByConnectionID.Add(Context.ConnectionId, name);
        }

        public async Task loadGame(string name)
        {
            gameService.loadGame(name, Context.ConnectionId);
            await Clients.Caller.SendAsync("OpenField", gameService.getGame(Context.ConnectionId).sizeX, gameService.getGame(Context.ConnectionId).sizeY);
            await Clients.Caller.SendAsync("loadSizes", gameService.getGame(Context.ConnectionId).sizeX, gameService.getGame(Context.ConnectionId).sizeY, gameService.getGame(Context.ConnectionId).bombsCount);

            for (int i = 0; i < gameService.getGame(Context.ConnectionId).sizeY; i++)
                for (int j = 0; j < gameService.getGame(Context.ConnectionId).sizeX; j++)
                {
                    if (gameService.getGame(Context.ConnectionId).field[i, j] != -2 && gameService.getGame(Context.ConnectionId).field[i, j] != -1)
                        await Clients.Caller.SendAsync("OpenCell", j, i, gameService.getGame(Context.ConnectionId).field[i, j], gameService.getGame(Context.ConnectionId).bombsCount - gameService.getGame(Context.ConnectionId).placedFlagsCount);
                }
        }

        //public async Task SendMove(int x, int y, bool isToFlag)
        //{
        //    await Clients.Caller.SendAsync("OpenOneTile", x, y);            
        //}

        public async Task startGame(int sizeX, int sizeY, int bombsCount)
        {            
            gameService.createGame(sizeX, sizeY, bombsCount, Context.ConnectionId);
            await Clients.Caller.SendAsync("OpenField", sizeX, sizeY);            
        }

        public async Task digCell(string cords) // "x;y"
        {
            int tempX = Convert.ToInt32(cords.Split(';')[0]);
            int tempY = Convert.ToInt32(cords.Split(';')[1]);
            if(gameService.getGame(Context.ConnectionId).dig(tempX, tempY))
                await Clients.Caller.SendAsync("playSound", "dirt");

            for (int i = 0; i < gameService.getGame(Context.ConnectionId).sizeY; i++)
                for (int j = 0; j < gameService.getGame(Context.ConnectionId).sizeX; j++)
                {
                    if(gameService.getGame(Context.ConnectionId).field[i, j] != -2 && gameService.getGame(Context.ConnectionId).field[i, j] != -1)
                        await Clients.Caller.SendAsync("OpenCell", j, i, gameService.getGame(Context.ConnectionId).field[i, j], gameService.getGame(Context.ConnectionId).bombsCount - gameService.getGame(Context.ConnectionId).placedFlagsCount);
                }
            if (gameService.getGame(Context.ConnectionId).isLost)
                await Clients.Caller.SendAsync("loose");
            if (gameService.getGame(Context.ConnectionId).isWin)
                await Clients.Caller.SendAsync("win");
        }
        public async Task flagCell(string cords) // "x;y"
        {
            int tempX = Convert.ToInt32(cords.Split(';')[0]);
            int tempY = Convert.ToInt32(cords.Split(';')[1]);
            if(gameService.getGame(Context.ConnectionId).flag(tempX, tempY))
                await Clients.Caller.SendAsync("playSound", "flag");

            for (int i = 0; i < gameService.getGame(Context.ConnectionId).sizeY; i++)
                for (int j = 0; j < gameService.getGame(Context.ConnectionId).sizeX; j++)
                {
                    //if ((gameService.game.field[i, j] != -2 && gameService.game.field[i, j] != -1) || (i == tempY && j == tempX))
                        await Clients.Caller.SendAsync("OpenCell", j, i, gameService.getGame(Context.ConnectionId).field[i, j], gameService.getGame(Context.ConnectionId).bombsCount - gameService.getGame(Context.ConnectionId).placedFlagsCount);
                }
        }
    }
}
