using Discord.Interactions;
using DGE.ProgramModules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.WebSocket;
using DGE.Bot;
using DGE.Exceptions;
using DGE.Discord;

namespace DGE.ProgramModules
{

    public class ProgramsCommands : DGEModuleBase
    {

        private static Dictionary<string, Type> programTypes = new Dictionary<string, Type>()
        {
            { "interaction", typeof(InteractionProgram) },
            { "counting", typeof(CountingProgram) },
            { "bonapio", typeof(BonapioProgram) },
            { "confusionrename", typeof(RenameConfusionProgram) },
            { "chat", typeof(ChatProgram) },
            { "void", typeof(TestVoidProgram) }
        };

        private static short maxUserProgramsCount = 4; // Need to remove the hardcode for this (DB update)
        private static Dictionary<ulong, char> userProgramsCount = new Dictionary<ulong, char>(); //char is used an 8bit unsigned integer

        [SlashCommand("PMVersion", "Returns the program command module version")]
        public async Task Version()
        {
            await ReplyAsync("ProgramModules V1.2.0"); // TODO: Program modules 2.0 with new implementation in mind ?
        }

        [SlashCommand("CreateProgram", "Creates a program")]
        public async Task CreateProgram(string programKey)
        {
            if (!userProgramsCount.ContainsKey(Context.User.Id)) //Adding a limit of programs that the user can instanciate
                userProgramsCount.Add(Context.User.Id, (char) 0);
            userProgramsCount[Context.User.Id]++;
            if (userProgramsCount[Context.User.Id] > maxUserProgramsCount)
                throw new Exception($"You cannot instance more than {maxUserProgramsCount} programs at once");

            if (!programTypes.ContainsKey(programKey.ToLower()))
                throw new Exception($"This program type was not found, try `/ShowPrograms`"); //TODO with prefix thingy later
            ProgramModule program = (ProgramModule)Activator.CreateInstance(programTypes[programKey.ToLower()], Context);
            await RespondAsync($"{program.Id}");
        }

        [SlashCommand("AddChannelToProgram", "Adds the context channel to the program of specified id")]
        public async Task AddListenedChannelToProgram(int programId)
        {
            if (ProgramNotExists(programId))
                return;
            ProgramModule.GetProgramById(programId).AddChannel(Context.Channel.Id, Context.Bot.client);
            await ReplyAsync($"This channel is now listened by the program {programId}");
        }

        [SlashCommand("RemoveChannelFromProgram", "Removes the current channel (where executed) from the program of specified id")]
        public async Task RemoveListenedChannelFromProgram(int programId)
        {
            if (ProgramNotExists(programId))
                return;
            ProgramModule.GetProgramById(programId).RemoveChannel(Context.Channel.Id);
            await RespondAsync($"This channel will not be listened by the program {programId} anymore");
        }

        [SlashCommand("DeleteProgram", "Deletes a program instance")]
        public async Task DeleteProgram(int programId)
        {
            if (ProgramNotExists(programId))
                return;

            ProgramModule program = ProgramModule.GetProgramById(programId);
            if (program.IsOwner(Context.User.Id))
            {
                userProgramsCount[program.GetOriginalUserId()]--; //Decreasing the points of the original program creator
                ProgramModule.DeleteProgram(programId);
            }
            else
                throw new Exception("You must be owner of the program to delete it");
            
            await RespondAsync($"Successfully deleted the program of id {programId}");
        }

        [SlashCommand("ClearPrograms", "Deletes every program instances")]
        [RequireOwner]
        public async Task ClearPrograms()
        {
            ProgramModule.ClearPrograms();
            await RespondAsync("Successfully deleted every programs");
        }

        [SlashCommand("ShowInstancedPrograms", "Shows every instanced program by showing their IDs and types")]
        public async Task ShowInstancedPrograms()
        {
            if (ProgramModule.GetPrograms().Count < 1)
                await RespondAsync("No program modules are currently instanced");
            await RespondAsync("Listing every existing programs :");
            string programsListing = "";
            foreach (ProgramModule program in ProgramModule.GetPrograms())
            {
                programsListing += $"- Program **{program.GetType().Name}** of id {program.Id}\n";
            }
            await RespondAsync(programsListing);
        }

        [SlashCommand("ShowPrograms", "Shows every program type instanciable")]
        public async Task ShowExistingPrograms()
        {
            await RespondAsync("The different programs that can be instanced are :\n- " + string.Join("\n- ", programTypes.Keys.ToArray()));
        }

        [SlashCommand("CreateAllPrograms", "Creates one program of each type")]
        [RequireOwner()]
        public async Task InstantiateAllPrograms()
        {
            string reply = "Instanced programs ids :";
            foreach (string programKey in programTypes.Keys)
                reply += $"\n- {((ProgramModule)Activator.CreateInstance(programTypes[programKey.ToLower()], Context)).Id}";
            await RespondAsync(reply);
        }

        [SlashCommand("ProgramsHelp", "Gives help and descriptions about programs")]
        public async Task CommandGetProgramModuleHelp()
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(Color.DarkBlue);

            embed.WithAuthor("ProgramModules :");
            //Listing the ProgramModules, with their description
            programTypes.ToList().ForEach(p => embed.AddField(
                $"{p.Value.Name} ({p.Key})",
                ProgramModule.GetDescription(p.Value)));

            await RespondAsync(embed: embed.Build());
        }

        [SlashCommand("AddProgramOwner", "Adds a user to a program as owner")]
        public async Task AddProgramOwner(int programId, IUser user )
        {
            if(ProgramNotExists(programId))
                return;

            ProgramModule program = ProgramModule.GetProgramById(programId);

            if (program.IsOwner(Context.User.Id))
                program.AddOwner(user.Id);
            else
                throw new Exception("You must be owner of the program give owner");

            await RespondAsync($"Made {user.Username} an owner of the program {program.Id}");
        }

        private bool ProgramNotExists(int programId)
        {
            if (!ProgramModule.ProgramExists(programId))
            {
                RespondAsync($"No ProgramModule of id {programId} was found (try `>ShowInstancedPrograms`)").GetAwaiter().GetResult(); //TODO: fix with custom prefix thingy
                return true;
            }
            return false;
        }

    }
}
