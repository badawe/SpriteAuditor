﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BrunoMikoski.SpriteAuditor.Serialization;
using UnityEditor;
using UnityEditor.Callbacks;

namespace BrunoMikoski.SpriteAuditor
{
    public static class AdditionalSpriteProvidersProcessor
    {
        private const string SPRITE_REPORTER_TYPE_KEY = "SPRITE_REPORTERS_TYPE_KEY";

        [DidReloadScripts]
        public static void AfterScriptsReload()
        {
            EditorApplication.delayCall += OnUnityCompilationFinished;
        }

        private static void OnUnityCompilationFinished()
        {
            Type spriteReporterType = typeof(ISpriteReporter);
            List<Type> foundImplementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x =>
                {
                    try
                    {
                        return x.GetTypes();
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        return Array.Empty<Type>();
                    }
                })
                .Where(x => x.IsClass && spriteReporterType.IsAssignableFrom(x)).ToList();

            if (foundImplementations.Count > 0)
            {
                EditorPrefs.SetString(SPRITE_REPORTER_TYPE_KEY, JsonWrapper.ToJson(foundImplementations));
            }
            else
            {
                EditorPrefs.DeleteKey(SPRITE_REPORTER_TYPE_KEY);
            }
        }
    }
}