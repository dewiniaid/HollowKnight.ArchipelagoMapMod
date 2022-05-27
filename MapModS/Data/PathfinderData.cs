﻿using RandomizerCore.Logic;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RD = RandomizerMod.RandomizerData.Data;
using RM = RandomizerMod.RandomizerMod;

namespace MapModS.Data
{
    public static class PathfinderData
    {
        internal static HashSet<string> persistentTerms;
        internal static Dictionary<string, string> conditionalTerms;

        private static Dictionary<string, string> benchwarpScenes;
        private static Dictionary<string, string> adjacentTransitions;
        private static Dictionary<string, string> scenesByTransition;
        private static Dictionary<string, HashSet<string>> transitionsByScene;

        internal static Dictionary<string, string> specialDoorsByScene;
        internal static Dictionary<string, string> specialDoorsByTransition;

        public static void Load()
        {
            persistentTerms = JsonUtil.Deserialize<HashSet<string>>("MapModS.Resources.Pathfinder.Data.persistentTerms.json");
            conditionalTerms = JsonUtil.Deserialize< Dictionary<string, string>> ("MapModS.Resources.Pathfinder.Data.conditionalTerms.json");
            benchwarpScenes = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.benchwarp.json");
            adjacentTransitions = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.adjacentTransitions.json");
            scenesByTransition = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Data.scenesByTransition.json");
            transitionsByScene = JsonUtil.Deserialize<Dictionary<string, HashSet<string>>>("MapModS.Resources.Pathfinder.Data.transitionsByScene.json");
            specialDoorsByScene = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Compass.specialDoorsByScene.json");
            specialDoorsByTransition = JsonUtil.Deserialize<Dictionary<string, string>>("MapModS.Resources.Pathfinder.Compass.specialDoorsByTransition.json");
        }

        private static readonly (LogicManagerBuilder.JsonType type, string fileName)[] files = new[]
        {
            (LogicManagerBuilder.JsonType.Macros, "macros"),
            (LogicManagerBuilder.JsonType.Waypoints, "waypoints"),
            (LogicManagerBuilder.JsonType.Transitions, "transitions"),
            (LogicManagerBuilder.JsonType.LogicEdit, "logicEdits"),
            (LogicManagerBuilder.JsonType.LogicSubst, "logicSubstitutions")
        };

        private static LogicManagerBuilder lmb;

        public static LogicManager lm;

        public static void MakeLogicManager()
        {
            lmb = new(RM.RS.Context.LM);

            foreach ((LogicManagerBuilder.JsonType type, string fileName) in files)
            {
                lmb.DeserializeJson(type, Assembly.GetExecutingAssembly().GetManifestResourceStream($"MapModS.Resources.Pathfinder.Logic.{fileName}.json"));
            }

            lm = new(lmb);
        }

        // Returns all benchwarps based on benches sat on + Start
        public static HashSet<string> GetBenchwarpTransitions()
        {
            IEnumerable<string> visitedBenches = Dependencies.GetVisitedBenchScenes();

            HashSet<string> transitions = new(benchwarpScenes.Where(b => visitedBenches.Contains(b.Value)).Select(b => b.Key));

            transitions.Add("Warp_Start");

            return transitions;
        }

        public static HashSet<string> GetTransitionsInScene(this string scene)
        {
            HashSet<string> transitions = TransitionData.GetTransitionsByScene(scene);

            if (transitionsByScene.ContainsKey(scene))
            {
                transitions.UnionWith(transitionsByScene[scene]);
            }

            return transitions;
        }

        public static string GetScene(this string transition)
        {
            if (transition == "Warp_Start")
            {
                return RD.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).SceneName;
            }

            if (scenesByTransition.ContainsKey(transition))
            {
                return scenesByTransition[transition];
            }
            else if (transition.IsSpecialTransition())
            {
                return adjacentTransitions[transition].GetScene();
            }

            return TransitionData.GetTransitionScene(transition);
        }

        public static string GetAdjacentTransition(this string source)
        {
            if (source == "Warp_Start")
            {
                return RD.GetStartDef(RM.RS.GenerationSettings.StartLocationSettings.StartLocation).Transition;
            }

            // Some top transitions don't have an adjacent transition
            if (TransitionData.IsInTransitionLookup(source))
            {
                return TransitionData.GetAdjacentTransition(source);
            }

            if (source.IsSpecialTransition())
            {
                return source;
            }

            MapModS.Instance.LogWarn($"No adjacent transition for {source}");

            return null;
        }

        public static bool IsSpecialTransition(this string transition)
        {
            return adjacentTransitions.ContainsKey(transition);
        }

        public static bool IsBenchwarpTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.StartsWith("Warp");
        }

        public static bool IsStagTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.StartsWith("Stag");
        }

        public static bool IsElevatorTransition(this string transition)
        {
            return transition.IsSpecialTransition() && (transition.StartsWith("Left_Elevator") || transition.StartsWith("Right_Elevator"));
        }

        public static bool IsTramTransition(this string transition)
        {
            return transition.IsSpecialTransition() && (transition.StartsWith("Lower_Tram") || transition.StartsWith("Upper_Tram"));
        }

        public static bool IsWarpTransition(this string transition)
        {
            return transition.IsSpecialTransition() && transition.Contains("[warp]");
        }

        public static string ToCleanName(this string transition)
        {
            if (transition.IsSpecialTransition())
            {
                return transition.Replace("_", " ").Replace("ARROW", "->");
            }
            else
            {
                return transition;
            }
        }
    }
}
