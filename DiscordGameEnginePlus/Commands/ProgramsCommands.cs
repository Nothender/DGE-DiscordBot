﻿using Discord.Commands;
using DiscordGameEngine.Core;
using DiscordGameEnginePlus.Programs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using DiscordGameEngine.ProgramModules;
using DiscordGameEngine;
using DiscordGameEngine.Exceptions;
using Discord;
using Discord.WebSocket;

namespace DiscordGameEnginePlus.Commands
{

    [Summary("Commands to manage ProgramModules")]
    public class ProgramsCommands : ModuleBase<SocketCommandContext>
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

        [Command("CreateProgram")]
        public async Task CreateProgram(string programKey)
        {
            ProgramModule program = (ProgramModule)Activator.CreateInstance(programTypes[programKey.ToLower()], Context);
            await ReplyAsync($"{program.Id}");
        }

        [Command("AddChannelToProgram")]
        public async Task AddListenedChannelToProgram(int programId)
        {
            ProgramModule.GetProgramById(programId).AddChannel(Context.Channel.Id);
            await ReplyAsync($"This channel is now listend by the program {programId}");
        }

        [Command("RemoveChannelFromProgram")]
        public async Task RemoveListenedChannelFromProgram(int programId)
        {
            ProgramModule.GetProgramById(programId).RemoveChannel(Context.Channel.Id);
            await ReplyAsync($"This channel will not be listened by the program {programId} anymore");
        }

        [Command("DeleteProgram")]
        public async Task DeleteProgram(int programId)
        {
            if (ProgramModule.GetProgramById(programId).IsOwner(Context.User.Id))
                ProgramModule.DeleteProgram(programId);
            else
                throw new CommandExecutionException("You must be owner of the program to delete it");
            await ReplyAsync($"Successfully deleted the program of id {programId}");
        }

        [Command("ClearPrograms")]
        [RequireOwner]
        public async Task ClearPrograms()
        {
            ProgramModule.ClearPrograms();
            await ReplyAsync("Successfully deleted every programs");
        }

        [Command("ShowInstancedPrograms")]
        public async Task ShowInstancedPrograms()
        {
            await ReplyAsync("Listing every existing programs :");
            string programsListing = "";
            foreach(ProgramModule program in ProgramModule.GetPrograms())
            {
                programsListing += $"- Program **{program.GetType().Name}** of id {program.Id}\n";
            }
            await ReplyAsync(programsListing);
        }

        [Command("ShowPrograms")]
        public async Task ShowExistingPrograms()
        {
            await ReplyAsync("The different programs that can be instanced are :" + string.Join("\n- ", programTypes.Keys.ToArray()));
        }

        [Command("CreateAllPrograms")]
        [RequireOwner()]
        public async Task InstantiateAllPrograms()
        {
            string reply = "Instanced programs ids :";
            foreach (string programKey in programTypes.Keys)
                reply += $"\n- {((ProgramModule)Activator.CreateInstance(programTypes[programKey.ToLower()], Context)).Id}";
            await ReplyAsync(reply);
        }

        [Command("ProgramsHelp")]
        public async Task CommandGetProgramModuleHelp()
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithColor(Color.DarkBlue);

            embed.WithAuthor("ProgramModules :");
            //Listing the ProgramModules, with their description
            programTypes.ToList().ForEach(p => embed.AddField(
                $"{p.Value.Name} ({p.Key})",
                ProgramModule.GetDescription(p.Value)));

            await ReplyAsync(embed: embed.Build());
        }
        
        [Command("AddProgramOwner")]
        public async Task AddProgramOwner(int programId, IUser user)
        {
            ProgramModule program = ProgramModule.GetProgramById(programId);

            if (program.IsOwner(Context.User.Id))
                program.AddOwner(user.Id);
            else
                throw new CommandExecutionException("You must be owner of the program give owner");

            await ReplyAsync($"Made {user.Username} an owner of the program {program.Id}");
        }

    }
}
