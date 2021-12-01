using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DGE.Remora.Responders
{
    public class PingPongResponder : IResponder<IMessageCreate>
    {
        private readonly IDiscordRestChannelAPI _channelAPI;

        public PingPongResponder(IDiscordRestChannelAPI channelAPI)
        {
            _channelAPI = channelAPI;
        }

        public async Task<Result> RespondAsync
        (
            IMessageCreate gatewayEvent,
            CancellationToken ct = default
        )
        {
            if (gatewayEvent.Content != "!ping")
            {
                return Result.FromSuccess();
            }

            var embed = new Embed(Description: $"Pong\nLatency: {(DateTime.Now - gatewayEvent.Timestamp).TotalMilliseconds}ms", Colour: Color.LawnGreen);
            var replyResult = await _channelAPI.CreateMessageAsync
            (
                gatewayEvent.ChannelID,
                embeds: new[] { embed },
                ct: ct
            );

            return !replyResult.IsSuccess
                ? Result.FromError(replyResult)
                : Result.FromSuccess();
        }
    }
}
