
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

#nullable enable

namespace Chat.Web.Helpers
{
    public static class ConnStringUtils
    {
        public static string FormConnectionString(IConfiguration configuration)
        {
            Dictionary<string, string> connStringDictionary =
                configuration.GetConnectionString("DefaultConnection")
                    .Split(";", StringSplitOptions.RemoveEmptyEntries)
                    .ToDictionary(
                        source => source.Split("=")[0],
                        source => source.Split("=")[1]);
            
            connStringDictionary.EnsureValueOrThrow("Server", "DB_HOST");
            connStringDictionary.EnsureValueOrThrow("Database", "DB_NAME");
            connStringDictionary.EnsureValueOrThrow("User", "DB_USER");
            connStringDictionary.EnsureValueOrThrow("Password", "DB_PASS");
            
            return string.Join(";", connStringDictionary.Select(pair => $"{pair.Key}={pair.Value}"));
        }

        private static void EnsureValueOrThrow(this IDictionary<string, string> source, string key, string envKey)
        {
            if (!source.ContainsKey(key))
            {
                string? envValue = Environment.GetEnvironmentVariable(envKey);
                
                if (string.IsNullOrWhiteSpace(envValue))
                {
                    throw new ArgumentNullException(key);
                }
                
                source.Add(key, envValue);
            }
        }
    }
}