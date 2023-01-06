using Minesweeper.Site.Hubs;
using Minesweeper.Site.Services;

namespace Minesweeper.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<MinesweeperService>();
            builder.Services.AddSignalR();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<GameHub>("/game");

            app.Run();
        }
    }
}