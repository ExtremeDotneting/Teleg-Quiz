﻿using System;

namespace TelegаQuiz
{
    public static class BotTokenResolver
    {
        public static string GetToken()
        {
            try
            {
                //Read token from gitignored file.
                var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                    "test_token.txt"));
                var token = File.ReadAllText(path).Trim();
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception();
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.txt' exists in solution folder.", ex);
            }
        }
    }
}
