﻿using TorreRPG.Entidades;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TorreRPG.Atributos;
using TorreRPG.Services;

namespace TorreRPG.Comandos.Exibir
{ 
    [Group("wiki")]
    [Description("Permite ver a wiki oficial.")]
    [ComoUsar("wiki ler [#ID]")]
    [Exemplo("wiki ler axd")]
    public class ComandoWiki : BaseCommandModule
    {
        public Banco banco;

        [GroupCommand()]
        public async Task ExecuteGroupAsync(CommandContext ctx)
        {
            int pageSize = 10;
            int currentPage = (int)1;
            if (currentPage != 0)
                currentPage = currentPage - 1;
            double totalDocuments = await banco.Wikis.CountDocumentsAsync(FilterDefinition<Wiki>.Empty);
            double totalPages = Math.Ceiling(totalDocuments / pageSize);

            List<Wiki> list = new List<Wiki>
                (banco.Wikis.Find(FilterDefinition<Wiki>.Empty)
               .Skip(currentPage * pageSize)
               .Limit(pageSize).ToList());
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
                str.AppendLine($"{list[i].Nome} - ID: {list[i].Id}");
            await ctx.RespondAsync($"Utilize `!wiki ler [#ID] para ler`\n{str.ToString()}");
        }

        [Command("ler")]
        [Cooldown(1, 5, CooldownBucketType.User)]
        public async Task WikiLerAsync(CommandContext ctx, string wiki = null, int pagina = 0)
        {
            if (string.IsNullOrEmpty(wiki))
            {
                await ctx.RespondAsync("Você precisa informar uma wiki pelo o ID");
                return;
            }

            var wikipedia = await banco.Wikis.Find(x => x.Id == wiki).FirstOrDefaultAsync();
            if (wikipedia == null)
            {
                await ctx.RespondAsync("Wiki não encontrada, você informou o ID corretamente?");
                return;
            }

            await ctx.RespondAsync(wikipedia.Texto[pagina]);
        }

        [Command("criar")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task WikiCriarAsync(CommandContext ctx)
        {
            Wiki wiki = new Wiki()
            {
                Id = "asdxz",
                Nome = "Wiki",
            };
            wiki.Texto = new List<string>();
            wiki.Texto.Add("wiki");
            wiki.Texto.Add("wiki");
            await banco.Wikis.InsertOneAsync(wiki);
            await ctx.RespondAsync("Criado!");
        }
    }
}
