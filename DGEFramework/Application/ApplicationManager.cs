using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Application
{
    public static class ApplicationManager
    {

        private static List<Application> applications = new List<Application>();

        /// <summary>
        /// Adds an application and returns its Id, returns -1 if it was already added (The app will be automatically shutdown when the main app is shutdown)
        /// </summary>
        /// <param name="application"></param>
        /// <returns>The id of the IApplication</returns>
        public static int Add(Application application)
        {
            if (applications.Contains(application))
                return -1;
            applications.Add(application);
            application.Id = applications.Count - 1;
            Main.OnShutdown += (s, e) => application.Stop();
            AssemblyFramework.logger.Log($"Application {application.GetType().Name} added (id: {application.Id})", EnderEngine.Logger.LogLevel.INFO);
            return application.Id;
        }

        public static Application Get(int id)
        {
            if (id < 0 || id >= applications.Count)
                throw new Exception($"No application of id {id} exists");
            return applications[id];
        }

        public static Application[] GetAll()
        {
            return applications.ToArray();
        }

        public static int GetCount()
        {
            return applications.Count;
        }

        internal static void Dispose()
        {
            int c = applications.Count;
            foreach (IApplication app in applications)
                app.Dispose();
            applications.Clear();
            AssemblyFramework.logger.Log($"Disposed of {c} applications", EnderEngine.Logger.LogLevel.INFO);


        }
    }
}
