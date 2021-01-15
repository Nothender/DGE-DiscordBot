using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEnginePlus.Programs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordGameEnginePlus.Commands
{
    public class ProgramsCommands : ModuleBase<SocketCommandContext>
    {

        Dictionary<string, Type> programTypes = new Dictionary<string, Type>()
        {
            { "interaction", typeof(InteractionProgram) },
            { "counting", typeof(CountingProgram) },
            { "bonapio", typeof(BonapioProgram) }
        };

        [Command("CreateProgram")]
        public async Task CreateProgram(params string[] args)
        {
            ProgramModule program = (ProgramModule)Activator.CreateInstance(programTypes[args[0].ToLower()], Context);
            await ReplyAsync($"{program.Id}");
        }

        [Command("AddChannelToProgram")]
        public async Task AddListenedChannelToProgram(int id)
        {
            ProgramModule.GetProgramById(id).AddChannel(Context.Channel.Id);
            await ReplyAsync($"This channel is now listend by the program {id}");
        }

    }
}
