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

        private static Dictionary<string, Type> programTypes = new Dictionary<string, Type>()
        {
            { "interaction", typeof(InteractionProgram) },
            { "counting", typeof(CountingProgram) },
            { "bonapio", typeof(BonapioProgram) },
            { "confusionrename", typeof(RenameConfusionProgram) },
            { "void", typeof(TestVoidProgram) }
        };

        [Command("CreateProgram")]
        public async Task CreateProgram(string programKey)
        {
            ProgramModule program = (ProgramModule)Activator.CreateInstance(programTypes[programKey.ToLower()], Context);
            await ReplyAsync($"{program.Id}");
        }

        [Command("AddChannelToProgram")]
        public async Task AddListenedChannelToProgram(int id)
        {
            ProgramModule.GetProgramById(id).AddChannel(Context.Channel.Id);
            await ReplyAsync($"This channel is now listend by the program {id}");
        }

        [Command("DeleteProgram")]
        public async Task DeleteProgram(int id)
        {
        }

        [Command("ClearPrograms")]
        public async Task ClearPrograms()
        {
        }
    }
}
