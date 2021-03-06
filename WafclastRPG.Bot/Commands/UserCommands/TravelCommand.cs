﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MongoDB.Driver;
using System.Threading.Tasks;
using WafclastRPG.Bot.Atributos;
using WafclastRPG.Bot.Database;
using WafclastRPG.Bot.Extensions;
using WafclastRPG.Game.Entities;

namespace WafclastRPG.Bot.Commands.UserCommands
{
    public class TravelCommand : BaseCommandModule
    {
        public BotDatabase banco;

        [Command("viajar")]
        [Description("Permite viajar para outra região.")]
        [Usage("viajar")]
        public async Task TravelCommandAsync(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var map = await banco.CollectionMaps.Find(x => x.Id == ctx.Channel.Id).FirstOrDefaultAsync();
            if (map == null)
            {
                await ctx.ResponderAsync("você não pode viajar até aqui!");
                return;
            }

            Task<Response> result;
            using (var session = await this.banco.StartDatabaseSessionAsync())
            {
                result = await session.WithTransactionAsync(async (s, ct) =>
                {
                    var player = await session.FindPlayerAsync(ctx.User);
                    if (player == null)
                        return Task.FromResult(new Response() { IsPlayerFound = false });

                    if (player.Character.LocalId == ctx.Channel.Id)
                        return Task.FromResult(new Response() { IsSamePlace = true });

                    if (player.Character.Karma < 0)
                        if (map.Tipo == WafclastMapaType.Cidade)
                            return Task.FromResult(new Response() { IsKarmaNegative = true });

                    player.Character.LocalId = ctx.Channel.Id;
                    player.Character.ServerId = ctx.Guild.Id;
                    await player.SaveAsync();

                    return Task.FromResult(new Response());
                });
            };
            var _response = await result;

            if (_response.IsPlayerFound == false)
            {
                await ctx.ResponderAsync(Strings.NovoJogador);
                return;
            }

            if (_response.IsSamePlace)
            {
                await ctx.ResponderAsync("não tem como viajar para o mesmo lugar! Você precisa ir em outro canal de texto.");
                return;
            }

            if (_response.IsKarmaNegative)
            {
                await ctx.ResponderAsync("seu Karma está negativo, os guardas não deixarão você entrar na cidade!");
                return;
            }

            await ctx.ResponderAsync($"você acaba de chegar em {Formatter.Bold(ctx.Channel.Name)}!");
        }

        public class Response
        {
            public bool IsPlayerFound = true;
            public bool IsSamePlace = false;
            public bool IsKarmaNegative = false;
        }
    }
}
