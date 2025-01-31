﻿using System.Linq;
using APMapMod.Data;
using APMapMod.Settings;
using Modding.Utils;
using UnityEngine;
using SM = UnityEngine.SceneManagement.SceneManager;

namespace APMapMod.UI
{
    internal class RouteCompass
    {
        private static GameObject compass;
        private static DirectionalCompass CompassC => compass.GetComponent<DirectionalCompass>();
        private static GameObject Knight => HeroController.instance?.gameObject;

        public static void CreateRouteCompass()
        {
            if (compass != null && CompassC != null) CompassC.Destroy();

            if (Knight == null
                || GUIController.Instance == null
                || GameManager.instance.IsNonGameplayScene()) return;

            Texture2D tex = GUIController.Instance.Images["arrow"];

            Sprite arrow = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

            compass = DirectionalCompass.Create
            (
                "Route Compass", // name
                Knight, // parent entity
                arrow, // sprite
                Colors.GetColor(ColorSetting.UI_Compass), // color
                1.5f, // radius
                2.0f, // scale
                IsCompassEnabled, // bool condition
                false, // lerp
                0.5f // lerp duration
            );

            compass.SetActive(false);
        }

        // public static void UpdateCompass()
        // {
        //     if (compass == null) return;
        //
        //     if (CompassC != null && TransitionPersistent.selectedRoute.Any())
        //     {
        //         string transition = TransitionPersistent.selectedRoute.First();
        //         string scene = PathfinderData.GetScene(transition);
        //         string gate = "";
        //
        //         if (Utils.CurrentScene() == scene)
        //         {
        //             if (PathfinderData.doorObjectsByTransition.ContainsKey(transition))
        //             {
        //                 gate = PathfinderData.doorObjectsByTransition[transition];
        //             }
        //             else if (TransitionData.IsInTransitionLookup(transition))
        //             {
        //                 gate = TransitionData.GetTransitionDoor(transition);
        //             }
        //             else if (transition.Contains("[") && transition.Contains("]"))
        //             {
        //                 gate = transition.Split(']')[0].Split('[')[1];
        //             }
        //         }
        //         else if ((transition.IsStagTransition() || transition.IsTramTransition())
        //             && PathfinderData.doorObjectsByScene.ContainsKey(Utils.CurrentScene()))
        //         {
        //             gate = PathfinderData.doorObjectsByScene[Utils.CurrentScene()];
        //         }
        //
        //         if (gate == "")
        //         {
        //             compass.SetActive(false);
        //             return;
        //         }
        //
        //         GameObject gateObject = UnityExtensions.FindGameObject(SM.GetActiveScene(), gate);
        //
        //         if (gateObject != null)
        //         {
        //             CompassC.trackedObjects = new() { gateObject };
        //             compass.SetActive(true);
        //             return;
        //         }
        //
        //         GameObject gateObject2 = UnityExtensions.FindGameObject(SM.GetActiveScene(), "_Transition Gates/" + gate);
        //
        //         if (gateObject2 != null)
        //         {
        //             CompassC.trackedObjects = new() { gateObject2 };
        //             compass.SetActive(true);
        //         }
        //     }
        //     else
        //     {
        //         compass.SetActive(false);
        //     }
        // }

        public static bool IsCompassEnabled()
        {
            return (APMapMod.LS.modEnabled
                && (APMapMod.LS.mapMode == MapMode.TransitionRando
                    || APMapMod.LS.mapMode == MapMode.TransitionRandoAlt)
                && APMapMod.GS.routeCompassEnabled);
        }
    }
}
