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
            
            Task dgeMain = DGEMain.MainAsync();
            DGEMain.AddCommandModule(typeof(CommandsExemple));
            DGEMain.AddCommandModule(typeof(ProgramsCommands));
            dgeMain.Wait();
        }
    }
}
