﻿using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WafclastRPG.Bot.Extensions;

namespace WafclastRPG.Bot.Events
{
    public static class CommandExecuted
    {
        public static Task Event(CommandsNextExtension cne, CommandExecutionEventArgs e)
        {
            cne.Client.Logger.LogInformation(new EventId(600, "Comando exec"), $"{e.Context.Guild.Name.RemoverAcentos()} - {e.Context.User.Id} executou '{e.Command.QualifiedName}'.", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
