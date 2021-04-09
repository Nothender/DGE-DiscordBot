using System;
using System.Threading.Tasks;
using DiscordGameEngine;
using DiscordGameEnginePlus.Commands;
using DiscordGameEnginePlus.Programs;

namespace DiscordGameEngineProgram
{
    public static class Program
    {
        private static void Main(string[] args)
        {

            Task dgeMain = DiscordGameEngineBot.StartAsync();
            //DiscordGameEngineBot.RegisterCommandModule(typeof(CommandsExemple));
            DiscordGameEngineBot.RegisterCommandModule(typeof(ProgramsCommands));
            dgeMain.Wait();
        }
    }
}
