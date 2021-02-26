using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using EnderEngine.Core;
using System.Linq;
using Discord.Commands;
using System.IO;
using System.Text.Json;

namespace DiscordGameEngine.Core
{
    public abstract class ProgramModule
    {
        private static readonly string SaveFileName = "ProgramsData" + ".json"; //TODO: Fix with extension naming (EE ?)
        private static bool loaded = false;
        private static Dictionary<int, ProgramModule> programs = new Dictionary<int, ProgramModule>();
        private static int nextId = 0;

        public static ProgramModule GetProgramById(int id) { return programs[id]; }

        private Dictionary<string, Action<SocketUserMessage>> triggerContentCallback = new Dictionary<string, Action<SocketUserMessage>>();
        protected List<ISocketMessageChannel> interactionChannels = new List<ISocketMessageChannel>();

        public int Id { get; private set; }

        static ProgramModule()
        {
            DGEMain.OnShutdown += SaveProgramsData;
        }

        public ProgramModule(ProgramData programData) //When loading from a save file (restoring program state)
        {
            programData.listenedChannelsId.ForEach((channelId) => { AddChannel(channelId); });
            LoadData(programData.data);
            AddProgram(this);
        }

        public ProgramModule(SocketCommandContext context)
        {
            AddProgram(this);
        }

        private static void AddProgram(ProgramModule program)
        {
            nextId++;
            program.Id = nextId;
            programs.Add(program.Id, program);
        }
        
        public static void DeleteProgram(int id)
        {
            programs[id].Broadcast($"{LogManager.DGE_LOG} The program {id} ({programs[id].GetType().Name}) has been deleted");
            programs.Remove(id);

        }

        public static void ClearPrograms()
        {
            foreach (int i in programs.Keys)
                DeleteProgram(i);
        }

        protected void AddInteraction(string trigger, Action<SocketUserMessage> callback)
        {
            triggerContentCallback.Add(trigger, callback);
        }

        /// <summary>
        /// Adds a channel that can be used to interact with the program
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns>Returns true if the item was added, false if it was already added</returns>
        public bool AddChannel(ulong channelId)
        {
            if (interactionChannels.Exists(x => x.Id == channelId))
                return false;
            ChannelListener.AddListenedChannel(channelId, OnInteraction);
            interactionChannels.Add(DGEMain._client.GetChannel(channelId) as ISocketMessageChannel);
            return true;
        }

        protected abstract void CallbackNoTriggerMessageRecieved(SocketUserMessage umessage);

        private void OnInteraction(ulong channelId, SocketUserMessage umessage)
        {
            int indexOfFirstSpace = umessage.Content.IndexOf(' ');
            string triggerWord = umessage.Content.Substring(0, indexOfFirstSpace < 0 ? umessage.Content.Length : indexOfFirstSpace);
            if (triggerContentCallback.ContainsKey(triggerWord))
                triggerContentCallback[triggerWord](umessage);
            else
                CallbackNoTriggerMessageRecieved(umessage);
        }

        protected void Broadcast(string message)
        {
            foreach (ISocketMessageChannel channel in interactionChannels)
                channel.SendMessageAsync(message);
        }

        //TODO: Loading data fails at json deserialization (deserialized to List<JsonElement> instead of List<object> being the elements saved)

        protected abstract List<object> GetData();

        protected abstract void LoadData(List<object> data);

        public static void SaveProgramsData(object sender, EventArgs e)
        {
            List<ProgramData> programsData = new List<ProgramData>(programs.Count);
            foreach (ProgramModule program in programs.Values)
                programsData.Add(new ProgramData(program));

            File.WriteAllText(Core.pathToSavedData + SaveFileName, JsonSerializer.Serialize(programsData));
        }

        internal static void RestoreSavedPrograms()
        {
            if (loaded)
                return;
            InstantiatePrograms(LoadProgramsData());
            loaded = true;
        }

        private static List<ProgramData> LoadProgramsData()
        {
            if (File.Exists(Core.pathToSavedData + SaveFileName))
                return JsonSerializer.Deserialize<List<ProgramData>>(File.ReadAllText(Core.pathToSavedData + SaveFileName));
            return new List<ProgramData>();
        }

        internal static void DeleteProgramsSaveFile()
        {
            File.Delete(Core.pathToSavedData + SaveFileName);
        }

        private static void InstantiatePrograms(List<ProgramData> programsData)
        {
            foreach (ProgramData programData in programsData)
            {
                ProgramModule program = (ProgramModule)Activator.CreateInstance(Type.GetType(programData.typeName), new object[] { programData });
                program.Broadcast($"Restored saved program {program.GetType().Name} with the new Id {program.Id}");
            }
        }

        public class ProgramData
        {
            public string typeName { get; set; }
            public List<ulong> listenedChannelsId { get; set; }
            public List<object> data { get; set; }

            public ProgramData(ProgramModule program)
            {
                typeName = program.GetType().AssemblyQualifiedName;
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

    }

}
