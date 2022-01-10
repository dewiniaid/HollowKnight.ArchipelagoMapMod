﻿using GlobalEnums;
using System;
using UnityEngine;
using MapModS.Data;
using MapModS.Settings;

namespace MapModS.Map
{
    internal class Pin : MonoBehaviour
    {
        public PinDef PinData { get; private set; } = null;
        private SpriteRenderer _SR => gameObject.GetComponent<SpriteRenderer>();

        private readonly Color _inactiveColor = Color.gray;
        private Color _origColor;

        public void SetPinData(PinDef pd)
        {
            PinData = pd;
            _origColor = _SR.color;
        }

        public void UpdatePin(MapZone mapZone)
        {
            if (PinData == null)
            {
                throw new Exception("Cannot enable pin with null pindata. Ensure game object is disabled before adding as component, then call SetPinData(<pd>) before enabling.");
            }

            try
            {
                ShowBasedOnMap(mapZone);
                HideIfFound();
                ModifyScaleAndColor();
            }
            catch (Exception e)
            {
                MapModS.Instance.LogError(message: $"Failed to update pin! ID: {PinData.name}\n{e}");
            }
        }

        // Hides or shows the pin depending on the state of the map (NONE is World Map)
        private void ShowBasedOnMap(MapZone mapZone)
        {
            if (mapZone == MapZone.NONE)
            {
                //gameObject.SetActive(true); return;

                if (MapModS.LS.mapState != MapState.PinsOverMap)
                {
                    gameObject.SetActive(true); return;
                }

                if (MapModS.AdditionalMapsInstalled)
                {
                    if (PinData.pinSceneAM != null)
                    {
                        if (SettingsUtil.GetMapSetting(PinData.mapZoneAM))
                        {
                            gameObject.SetActive(true); return;
                        }
                    }
                    else if (SettingsUtil.GetMapSetting(PinData.mapZone))
                    {
                        gameObject.SetActive(true); return;
                    }
                }

                if (SettingsUtil.GetMapSetting(PinData.mapZone))
                {
                    gameObject.SetActive(true); return;
                }
            }
           
            if (PinData.pinSceneAM != null && mapZone == PinData.mapZoneAM)
            {
                gameObject.SetActive(true); return;
            }

            if (mapZone == PinData.mapZone)
            {
                gameObject.SetActive(true); return;
            }

                //// Show everything if full map was revealed
                //// Or, if it's a Map, always show the pin
                //if (MapModS.LS.RevealFullMap || PinData.vanillaPool == Pool.Map)
                //{
                //    gameObject.SetActive(true);
                //    return;
                //}

                //// Show these pins if the corresponding map item has been picked up
                //if (SettingsUtil.GetMMSMapSetting(PinData.mapZone))
                //{
                //    if (PinData.vanillaPool == Pool.Skill
                //    || PinData.vanillaPool == Pool.Charm
                //    || PinData.vanillaPool == Pool.Key
                //    || PinData.vanillaPool == Pool.Notch
                //    || PinData.vanillaPool == Pool.Mask
                //    || PinData.vanillaPool == Pool.Vessel
                //    || PinData.vanillaPool == Pool.Ore
                //    || PinData.vanillaPool == Pool.EssenceBoss)
                //    {
                //        gameObject.SetActive(true);
                //        return;
                //    }

                //    // Only show the rest if the corresponding scene/room has been mapped
                //    if (PinData.pinScene != null)
                //    {
                //        if (PlayerData.instance.scenesMapped.Contains(PinData.pinScene))
                //        {
                //            gameObject.SetActive(true);
                //            return;
                //        }
                //    }
                //    else
                //    {
                //        if (PlayerData.instance.scenesMapped.Contains(PinData.sceneName))
                //        {
                //            gameObject.SetActive(true);
                //            return;
                //        }
                //    }
                //}

            gameObject.SetActive(false);
        }

        private void HideIfFound()
        {
            if (RandomizerMod.RandomizerMod.RS.TrackerData.clearedLocations.Contains(PinData.name))
            {
                gameObject.SetActive(false);
                return;
            }

            // For non-randomized items
            if (MapModS.LS.ObtainedVanillaItems.ContainsKey(PinData.objectName + PinData.sceneName))
            {
                gameObject.SetActive(false);
            }
        }

        private void ModifyScaleAndColor()
        {
            if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableLocations.Contains(PinData.name))
            {
                _SR.color = _origColor;
            }
            else
            {
                // Non-randomized items also fall here
                transform.localScale = 0.7f * transform.localScale;
                _SR.color = _inactiveColor;
            }
        }
    }
}