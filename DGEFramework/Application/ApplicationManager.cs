using DGE.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DGE.Application
{
    public static class ApplicationManager
    {

        private static List<ApplicationBase> applications = new List<ApplicationBase>();

        /// <summary>
        /// Adds an application, sets and returns its Id, returns -1 if it was already added (The app will be automatically shutdown when the main app is shutdown)
        /// </summary>
        /// <param name="application"></param>
        /// <returns>The id of the IApplication</returns>
        public static int Add(ApplicationBase application)
        {
            if (applications.Contains(application))
                return -1;
            applications.Add(application);
            application.Id = applications.Count - 1;
            Main.OnShutdown += (s, e) => 
            {
                try
                {
                    application.Stop();
                }
                catch (Exception ex)
                {
                    AssemblyFramework.logger.Log($"An error happened while stopping the application {application.GetType()} (id: {application.Id}) :\n{ex.Message}", EnderEngine.Logger.LogLevel.ERROR);
                }
            };
            AssemblyFramework.logger.Log($"Application {application.GetType().Name} added (id: {application.Id})", EnderEngine.Logger.LogLevel.INFO);
            return application.Id;
        }

        public static ApplicationBase Get(int id)
        {
            if (id < 0 || id >= applications.Count)
                throw new Exception($"No application of id {id} exists");
            return applications[id];
        }

        public static ApplicationBase[] GetAll()
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
            int ad = 0;
            foreach (IApplication app in applications)
            {
                try
                {
                    app.Dispose();
                    ad++;
                }
                catch (Exception ex)
                {
                    AssemblyFramework.logger.Log($"An error happened while disposing of the application {app.GetType()} (id: {app.Id}) :\n{ex.Message}", EnderEngine.Logger.LogLevel.ERROR);
                }
            }
            applications.Clear();
            AssemblyFramework.logger.Log($"Disposed of {ad}/{c} applications", EnderEngine.Logger.LogLevel.INFO);
        }
    }
}
