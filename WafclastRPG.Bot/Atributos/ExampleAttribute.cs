﻿using System;

namespace WafclastRPG.Bot.Atributos
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExampleAttribute : Attribute
    {
        public string Command { get; }
        public string Description { get; }

        public ExampleAttribute(string command, string description)
        {
            Command = command;
            Description = description;
        }
    }
}