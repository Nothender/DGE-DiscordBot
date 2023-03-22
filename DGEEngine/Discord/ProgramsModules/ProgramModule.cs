using DGE.Bot;
using DGE.Core;
using DGE.Discord;
using Discord.Commands;
using Discord.WebSocket;
using EnderEngine;
using EnderEngine.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DGE.ProgramModules
{
    public abstract class ProgramModule
    {
        private static readonly string SaveFileName = "ProgramsData" + ".json"; //TODO: Fix with extension naming (EE ?)
        private static bool loaded = false;
        private static Dictionary<int, ProgramModule> programs = new Dictionary<int, ProgramModule>();
        private static int nextId = 0;

        private Dictionary<string, Action<SocketUserMessage>> triggerWordsCallback = new Dictionary<string, Action<SocketUserMessage>>();
        protected List<ISocketMessageChannel> interactionChannels = new List<ISocketMessageChannel>();

        #region ProgramModulesHelpAndDescriptions

        // Help and descriptions are terrible, very badly made, disgusting, to improve in ProgramModules V2
        private static Dictionary<string, string> programTypesDescriptions = new Dictionary<string, string>();

        public static string GetDescription(Type programModuleType)
        { return programTypesDescriptions.Keys.Contains(programModuleType.Name) ? programTypesDescriptions[programModuleType.Name] : "This ProgramModule doesn't provide any description"; }

        protected static void SetDescription(Type programModuleType, string description)
        {
            programTypesDescriptions[programModuleType.Name] = description;
        }

        #endregion ProgramModulesHelpAndDescriptions

        public int Id { get; private set; }

        static ProgramModule()
        {
            Main.OnShutdown += SaveProgramsData;
        }

        public ProgramModule(ProgramData programData, IBot bot) //When loading from a save file (restoring program state)
        {
            programData.listenedChannelsId.ForEach((channelId) => { AddChannel(channelId, bot.client); });
            programOwnersUserId = (ulong[])programData.owners.Clone();
            LoadData(programData.data);
            AddProgram(this);
        }

        public ProgramModule(SocketCommandContext context)
        {
            AddProgram(this);
            programOwnersUserId = new ulong[1] { context.User.Id };
        }

        #region ProgramCollection

        public static ProgramModule GetProgramById(int id)
        { return programs[id]; }

        public static List<ProgramModule> GetPrograms()
        { return programs.Values.ToList(); }

        private static void AddProgram(ProgramModule program)
        {
            program.Id = nextId++;
            programs.Add(program.Id, program);
        }

        public static void DeleteProgram(int id)
        {
            programs[id].Broadcast($"{LogPrefixes.DGE_LOG} The program {id} ({programs[id].GetType().Name}) has been deleted");
            programs[id].ClearChannels();
            programs.Remove(id);
        }

        public static void ClearPrograms()
        {
            foreach (int i in programs.Keys)
                DeleteProgram(i);
            nextId = 0;
        }

        public static bool ProgramExists(int programId)
        {
            return programs.ContainsKey(programId);
        }

        #endregion ProgramCollection

        protected void AddInteraction(string trigger, Action<SocketUserMessage> callback, string help = null)
        {
            triggerWordsCallback.Add(trigger, callback);
        }

        public List<string> GetTriggerWords()
        { return triggerWordsCallback.Keys.ToList(); }

        #region Permissions

        private ulong[] programOwnersUserId;

        public void AddOwner(ulong userId)
        {
            programOwnersUserId = programOwnersUserId.Append(userId);
        }

        public bool IsOwner(ulong userId)
        {
            return programOwnersUserId.Contains(userId);
        }

        /// <summary>
        /// Removes every owner except the original one
        /// </summary>
        public void ClearOwners()
        {
            programOwnersUserId = new ulong[1] { GetOriginalUserId() }; //Clearing every user, keeping the original owner
        }

        public void RemoveOwner(ulong userId)
        {
            List<ulong> ownersList = programOwnersUserId.ToList();
            ownersList.Remove(userId);
            programOwnersUserId = ownersList.ToArray();
        }

        public ulong GetOriginalUserId()
        {
            return programOwnersUserId[0];
        }

        #endregion Permissions

        #region Channels

        /// <summary>
        /// Adds a channel that can be used to interact with the program
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns>Returns true if the item was added, false if it was already added</returns>
        public bool AddChannel(ulong channelId, DiscordSocketClient client)
        {
            if (interactionChannels.Exists(x => x.Id == channelId))
                return false;
            ChannelListener.AddListenedChannel(channelId, OnInteraction);
            interactionChannels.Add(client.GetChannel(channelId) as ISocketMessageChannel);
            OnChannelAdded(channelId);
            return true;
        }

        public bool RemoveChannel(ulong channelId)
        {
            if (!interactionChannels.Exists(x => x.Id == channelId))
                return false;
            OnChannelRemoving(channelId);
            ChannelListener.RemoveListenedChannel(channelId, OnInteraction);
            interactionChannels.RemoveAt(interactionChannels.FindIndex(s => s.Id == channelId));
            return true;
        }

        public void ClearChannels()
        {
            foreach (ulong channelId in interactionChannels.Select(i => i.Id).ToArray())
                RemoveChannel(channelId);
        }

        public ISocketMessageChannel GetChannelById(ulong channelId)
        {
            return interactionChannels.Find(c => c.Id == channelId);
        }

        /// <summary>
        /// called just before removing a channel
        /// </summary>
        /// <param name="channelId"></param>
        protected virtual void OnChannelRemoving(ulong channelId)
        { }

        /// <summary>
        /// called when a channel was added
        /// </summary>
        protected virtual void OnChannelAdded(ulong channelId)
        { }

        #endregion Channels

        protected abstract void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage);

        private void OnInteraction(ulong channelId, SocketUserMessage umessage)
        {
            int indexOfFirstSpace = umessage.Content.IndexOf(' ');
            string triggerWord = umessage.Content.Substring(0, indexOfFirstSpace < 0 ? umessage.Content.Length : indexOfFirstSpace);
            if (triggerWordsCallback.ContainsKey(triggerWord))
                triggerWordsCallback[triggerWord](umessage);
            else
                CallbackNoTriggerMessageRecieved(umessage);
        }

        protected void Broadcast(string message)
        {
            foreach (ISocketMessageChannel channel in interactionChannels)
                channel.SendMessageAsync(message);
        }

        #region JSONSerialization

        //TODO: Loading data fails at json deserialization (deserialized to List<JsonElement> instead of List<object> being the elements saved)

        protected abstract List<object> GetData();

        protected abstract void LoadData(List<object> data);

        public static void SaveProgramsData(object sender, EventArgs e)
        {
            try
            {
                List<ProgramData> programsData = new List<ProgramData>(programs.Count);
                foreach (ProgramModule program in programs.Values)
                    programsData.Add(new ProgramData(program));

                File.WriteAllText(Paths.Get("SaveData") + SaveFileName, JsonSerializer.Serialize(programsData));
                AssemblyEngine.logger.Log("Saved discord ProgramModules data", Logger.LogLevel.INFO);
            }
            catch (Exception exception)
            {
                AssemblyEngine.logger.Log("Exception occured when trying to save discord ProgramModules data : " + exception.Message, Logger.LogLevel.ERROR);
            }
        }

        public static void RestoreSavedPrograms(IBot bot)
        {
            if (loaded)
                return;
            InstantiatePrograms(LoadProgramsData(), bot);
            loaded = true;
            AssemblyEngine.logger.Log("Restored saved programs", EnderEngine.Logger.LogLevel.INFO);
        }

        private static List<ProgramData> LoadProgramsData()
        {
            if (File.Exists(Paths.Get("SaveData") + SaveFileName))
                return JsonSerializer.Deserialize<List<ProgramData>>(File.ReadAllText(Paths.Get("SaveData") + SaveFileName));
            return new List<ProgramData>();
        }

        internal static void DeleteProgramsSaveFile()
        {
            File.Delete(Paths.Get("SaveData") + SaveFileName);
        }

        private static void InstantiatePrograms(List<ProgramData> programsData, IBot bot)
        {
            foreach (ProgramData programData in programsData)
            {
                ProgramModule program = (ProgramModule)Activator.CreateInstance(Type.GetType(programData.typeName), new object[] { programData, bot });
                program.Broadcast($"Restored saved program **{program.GetType().Name}** with the new Id {program.Id}");
            }
        }

        public class ProgramData
        {
            public string typeName { get; set; }
            public ulong[] owners { get; set; }
            public List<ulong> listenedChannelsId { get; set; }
            public List<object> data { get; set; }

            public ProgramData(ProgramModule program)
            {
                typeName = program.GetType().AssemblyQualifiedName;
                owners = (ulong[])program.programOwnersUserId.Clone();
                listenedChannelsId = program.interactionChannels.Select(channel => channel.Id).ToList();
                data = program.GetData();
            }

            /// <summary>
            /// For JSON deserialization
            /// </summary>
            public ProgramData()
            {
            }
        }

        #endregion JSONSerialization
    }
}