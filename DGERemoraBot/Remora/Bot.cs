using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DGE.Application;
using DGE.Core;
using DGE.Remora.Responders;
using EnderEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora;
using Remora.Discord;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace DGE.Remora
{
    public class Bot : ApplicationBase
    {
        #region IApplication

        public override event EventHandler OnStarting;
        public override event EventHandler OnStarted;
        public override event EventHandler OnShutdown;
        public override event EventHandler OnStopped;

        #endregion IApplication

        private string Token;

        public Bot(string Token)
        {
            this.Token = Token;
        }

        public override void Start()
        {
            if ((int)status > 1) //App already on or starting
                return;
            OnStarting?.Invoke(this, EventArgs.Empty);
            Task.Run(Main);
        }

        public override void Stop() { }

        public override void Dispose() { }

        public async Task Main()
        {
            var cancellationSource = new CancellationTokenSource();

            System.Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationSource.Cancel();
            };

            var botToken = Token;
            // Do not place your bot token in the source code of your program
            // when you write your real bot. It's a massive security risk,
            // and is only done here for the sake of this guide. You should store
            // your token outside of the program in some kind of database or file
            // (appsettings, plaintext file, etc) that is not directly accessible
            // from your source code.

            var services = new ServiceCollection()
                .AddDiscordGateway(_ => botToken)
                .AddResponder<PingPongResponder>()
                .BuildServiceProvider();

            var gatewayClient = services.GetRequiredService<DiscordGatewayClient>();
            var log = services.GetRequiredService<ILogger<Bot>>();

            var runResult = await gatewayClient.RunAsync(cancellationSource.Token);
            
            if (!runResult.IsSuccess)
            {
                switch (runResult.Error)
                {
                    case ExceptionError exe:
                        {
                            log.LogError
                            (
                                exe.Exception,
                                "Exception during gateway connection: {ExceptionMessage}",
                                exe.get_Message()
                            );

                            break;
                        }
                    case GatewayWebSocketError e:
                    case GatewayDiscordError exe:
                        {
                            log.LogError("Gateway error: {Message}", runResult.Error.Message);
                            break;
                        }
                    default:
                        {
                            log.LogError("Unknown error: {Message}", runResult.Error.Message);
                            break;
                        }
                }
            }

            System.Console.WriteLine("Bye bye");
        }

    }
}
