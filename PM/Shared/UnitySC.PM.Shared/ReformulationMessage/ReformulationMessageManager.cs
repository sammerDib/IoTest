using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.ReformulationMessage
{
    public static class ReformulationMessageManager
    {
        private static Reformulation s_reformulation;
        static private ILogger s_logger;

        public static void Init(string reformulationFileName,ILogger logger)
        {
            try
            {
                s_logger = logger;
                s_reformulation = XML.Deserialize<Reformulation>(reformulationFileName);
                s_logger?.Information("ReformulationMessageManager has been initialized");
            }
            catch (Exception exception)
            {
                s_logger?.Error(exception.Message);
            }
        }

        public static string GetUserContent(string source, string content, string defaultUserContent = null)
        {
            var reformulationMessage = FindReformulationMessage(source, content);
            if (reformulationMessage == null)
            {
                s_logger?.Error($"The UserContent for [{source}] - [{content}] doesn't exist in the reformulation file");
                return defaultUserContent;
            }

            var arguments = GetContentArguments(reformulationMessage, content);

            return FormatUserContent(reformulationMessage.UserContent, arguments);
        }

        public static MessageLevel GetLevel(string source, string content, MessageLevel defaultLevel = MessageLevel.None)
        {
            var reformulationMessage = FindReformulationMessage(source, content);
            if (reformulationMessage == null)
            {
                s_logger?.Error($"The Level for [{source}] - [{content}] doesn't exist in the reformulation file");
                return defaultLevel;
            }
            return reformulationMessage.Level;
        }

        // Looks for a ReformulationMessage that matches source and content
        private static ReformulationMessage FindReformulationMessage(string source, string content)
        {
            if ((s_reformulation == null) || (s_reformulation.ReformulationMessageList == null))
            {
                return null;
            }
            return s_reformulation.ReformulationMessageList.FirstOrDefault(re => MatchContent(source, content, re));
        }

        // Checks if a source and content matches a ReformulationMessage
        private static bool MatchContent(string source, string content, ReformulationMessage reformulationMessage)
        {
            // If the content contains a wildcard
            if (reformulationMessage.Content.Contains("*"))
            {
                string pattern = reformulationMessage.Content.Replace("*", "(.*)");
                bool resultFound = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase).IsMatch(content) && string.Equals(reformulationMessage.Source, source, StringComparison.OrdinalIgnoreCase);
                return resultFound;
            }

            return string.Equals(reformulationMessage.Content, content, StringComparison.OrdinalIgnoreCase) && string.Equals(reformulationMessage.Source, source, StringComparison.OrdinalIgnoreCase);
        }

        // Gets a list of strings replaced by *
        private static List<string> GetContentArguments(ReformulationMessage reformulationMessage, string content)
        {
            var arguments = new List<string>();
            string pattern = reformulationMessage.Content.Replace("*", "(.*)");
            var groups = Regex.Match(content, pattern).Groups;
            for (int i = 1; i < groups.Count; i++)
            {
                arguments.Add(groups[i].ToString());
            }
            return arguments;
        }

        // Replaces the {N} by the corresponding string from the arguments list
        private static string FormatUserContent(string userContent, List<string> arguments)
        {
            if (arguments != null)
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    Regex regex = new Regex(@"\{\s*[" + i + @"]\s*\}");
                    if (regex.Match(userContent).Success)
                    {
                        userContent = regex.Replace(userContent, arguments[i], 1);
                    }
                    else
                    {
                        s_logger?.Warning($"Argument id {i}, is not used in userContent");
                    }
                }
            }

            return userContent;
        }
    }
}
