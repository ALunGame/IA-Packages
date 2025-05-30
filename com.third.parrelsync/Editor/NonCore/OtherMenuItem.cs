﻿namespace ParrelSync.NonCore
{
    using UnityEditor;
    using UnityEngine;

    public class OtherMenuItem
    {
        [MenuItem("Tools/多开工具/GitHub/View this project on GitHub", priority = 10)]
        private static void OpenGitHub()
        {
            Application.OpenURL(ExternalLinks.GitHubHome);
        }

        [MenuItem("Tools/多开工具/GitHub/View FAQ", priority = 11)]
        private static void OpenFAQ()
        {
            Application.OpenURL(ExternalLinks.FAQ);
        }

        [MenuItem("Tools/多开工具/GitHub/View Issues", priority = 12)]
        private static void OpenGitHubIssues()
        {
            Application.OpenURL(ExternalLinks.GitHubIssue);
        }
    }
}