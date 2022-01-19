﻿using GlobalEnums;
using MapModS.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapModS.Map
{
    public class PinsCustom : MonoBehaviour
    {
        private readonly Dictionary<PoolGroup, GameObject> _Groups = new();

        private readonly List<Pin> _pins = new();

        public void MakePins(GameMap gameMap)
        {
            DestroyPins();

            foreach (PinDef pinData in DataLoader.GetPinArray())
            {
                try
                {
                    MakePin(pinData, gameMap);
                }
                catch (Exception e)
                {
                    MapModS.Instance.LogError(e);
                }
            }
        }

        private void MakePin(PinDef pinData, GameMap gameMap)
        {
            if (pinData.disable) return;

            // Create new pin GameObject
            GameObject goPin = new($"pin_mapmod_{pinData.name}")
            {
                layer = 30
            };

            // Attach sprite renderer to the GameObject
            SpriteRenderer sr = goPin.AddComponent<SpriteRenderer>();

            // Initialize sprite to vanillaPool
            sr.sprite = SpriteManager.GetSpriteFromPool(pinData.vanillaPool, PinBorderColor.Normal);
            sr.sortingLayerName = "HUD";
            sr.size = new Vector2(1f, 1f);

            // Attach pin data to the GameObject
            Pin pin = goPin.AddComponent<Pin>();
            pin.SetPinData(pinData);
            _pins.Add(pin);

            // Rename pin if there are two items at the same location
            if (RandomizerMod.RandomizerMod.RS.GenerationSettings.PoolSettings.LoreTablets
                && RandomizerMod.RandomizerMod.RS.GenerationSettings.NoveltySettings.RandomizeFocus
                && pin.PinData.name == "Focus")
            {
                pin.PinData.name = "Lore_Tablet-King's_Pass_Focus";
            }

            // Set pin transform (by pool)
            AssignGroup(goPin, pinData);

            if (MapModS.AdditionalMapsInstalled)
            {
                foreach (PinDef pinDataAM in DataLoader.GetPinAMArray())
                {
                    if (pinDataAM.name == pin.PinData.name)
                    {
                        pin.PinData.pinScene = pinDataAM.pinScene;
                        pin.PinData.mapZone = pinDataAM.mapZone;
                        pin.PinData.offsetX = pinDataAM.offsetX;
                        pin.PinData.offsetY = pinDataAM.offsetY;
                        break;
                    }
                }
            }

            string roomName = pinData.pinScene ?? pinData.sceneName;

            Vector3 vec = GetRoomPos(roomName, gameMap);
            vec.Scale(new Vector3(1.46f, 1.46f, 1));

            vec += new Vector3(pinData.offsetX, pinData.offsetY, pinData.offsetZ);

            goPin.transform.localPosition = new Vector3(vec.x, vec.y, vec.z - 0.01f);
        }

        private void AssignGroup(GameObject newPin, PinDef pinData)
        {
            if (!_Groups.ContainsKey(pinData.spoilerPool))
            {
                _Groups[pinData.spoilerPool] = new GameObject("PinGroup " + pinData.spoilerPool);
                _Groups[pinData.spoilerPool].transform.SetParent(transform);
                _Groups[pinData.spoilerPool].SetActive(true);
            }

            newPin.transform.SetParent(_Groups[pinData.spoilerPool].transform);
        }

        private Vector3 GetRoomPos(string roomName, GameMap gameMap)
        {
            foreach (Transform areaObj in gameMap.transform)
            {
                foreach (Transform roomObj in areaObj.transform)
                {
                    if (roomObj.gameObject.name == roomName)
                    {
                        Vector3 roomVec = roomObj.transform.localPosition;
                        roomVec.Scale(areaObj.transform.localScale);
                        return areaObj.transform.localPosition + roomVec;
                    }
                }
            }

            MapModS.Instance.LogWarn($"{roomName} is not a valid room name!");
            return new Vector3(0, 0, 0);
        }

        // Used for updating button states
        public List<PoolGroup> RandomizedGroups = new();

        public List<PoolGroup> OthersGroups = new();

        internal void FindRandomizedGroups()
        {
            RandomizedGroups.Clear();
            OthersGroups.Clear();

            foreach (PoolGroup group in _Groups.Keys)
            {
                AddGroupToList(group);
            }
        }

        // If any pin has vanillaPool != spoilerPool, consider the whole group Randomized
        internal void AddGroupToList(PoolGroup group)
        {
            if (group == PoolGroup.Shop)
            {
                RandomizedGroups.Add(group);
                return;
            }

            foreach (Transform pinT in _Groups[group].GetComponentsInChildren<Transform>())
            {
                Pin pin = pinT.GetComponent<Pin>();

                if (pin == null) continue;

                if (pin.PinData.vanillaPool != pin.PinData.spoilerPool)
                {
                    RandomizedGroups.Add(group);
                    return;
                }
            }

            OthersGroups.Add(group);
        }

        internal void ToggleSpoilers()
        {
            MapModS.LS.SpoilerOn = !MapModS.LS.SpoilerOn;
            RefreshSprites();
        }

        internal void TogglePinStyle()
        {
            switch (MapModS.LS.pinStyle)
            {
                case PinStyle.Normal:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_1;
                    break;

                case PinStyle.Q_Marks_1:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_2;
                    break;

                case PinStyle.Q_Marks_2:
                    MapModS.LS.pinStyle = PinStyle.Q_Marks_3;
                    break;

                case PinStyle.Q_Marks_3:
                    MapModS.LS.pinStyle = PinStyle.Normal;
                    break;
            }

            RefreshSprites();
        }

        internal void ToggleRandomized()
        {
            MapModS.LS.RandomizedOn = !MapModS.LS.RandomizedOn;
            foreach (PoolGroup group in RandomizedGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.LS.RandomizedOn);
            }

            RefreshGroups();
        }

        internal void ToggleOthers()
        {
            MapModS.LS.OthersOn = !MapModS.LS.OthersOn;
            foreach (PoolGroup group in OthersGroups)
            {
                MapModS.LS.SetOnFromGroup(group, MapModS.LS.OthersOn);
            }

            RefreshGroups();
        }

        public void UpdatePins(MapZone mapZone, HashSet<string> transitionPinScenes)
        {
            foreach (Pin pin in _pins)
            {
                pin.UpdatePin(mapZone, transitionPinScenes);
            }
        }

        // Called every time when any relevant setting is changed, or when the Map is opened
        public void RefreshGroups()
        {
            foreach (PoolGroup group in _Groups.Keys)
            {
                _Groups[group].SetActive(MapModS.LS.GetOnFromGroup(group));
            }
        }

        public void RefreshSprites()
        {
            foreach (Pin pin in _pins)
            {
                if (RandomizerMod.RandomizerMod.RS.TrackerData.previewedLocations.Contains(pin.PinData.name))
                {
                    pin.SR.sprite = SpriteManager.GetSpriteFromPool(pin.PinData.spoilerPool, PinBorderColor.Previewed);
                    continue;
                }

                PinBorderColor pinColor = PinBorderColor.Normal;

                if (RandomizerMod.RandomizerMod.RS.TrackerData.uncheckedReachableLocations.Contains(pin.PinData.name)
                    && !RandomizerMod.RandomizerMod.RS.TrackerDataWithoutSequenceBreaks.uncheckedReachableLocations.Contains(pin.PinData.name))
                {
                    pinColor = PinBorderColor.OutOfLogic;
                }

                if (MapModS.LS.SpoilerOn)
                {
                    pin.SR.sprite = SpriteManager.GetSpriteFromPool(pin.PinData.spoilerPool, pinColor);
                }
                else
                {
                    pin.SR.sprite = SpriteManager.GetSpriteFromPool(pin.PinData.vanillaPool, pinColor);
                }
            }
        }

        public void ResizePins()
        {
            foreach (Pin pin in _pins)
            {
                pin.SetSizeAndColor();
            }
        }

        public void DestroyPins()
        {
            foreach (Pin pin in _pins)
            {
                Destroy(pin.gameObject);
            }

            _pins.Clear();
            RandomizedGroups.Clear();
            OthersGroups.Clear();
        }

        protected void Start()
        {
            gameObject.SetActive(false);
        }
    }
}