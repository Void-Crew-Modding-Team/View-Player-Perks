using CG.Game;
using ExitGames.Client.Photon;
using Gameplay.Perks;
using Photon.Realtime;
using ResourceAssets;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UI.Token;
using UnityEngine;
using VoidManager.CustomGUI;
using VoidManager.Utilities;

namespace ViewPlayerPerks
{
    internal class PlayerPerkSettings : PlayerSettingsMenu
    {
        private GUISkin _cachedSkin;
        private GUISkin _defaultSkin;
        private int spentPerks;

        private int PerkPoints;
        private int PlayerLevel;
        private List<PerkRef> activePerks;
        Dictionary<GUIDUnion, int> buffLevels;
        public override void Refresh(Player selectedPlayer)
        {
            if (selectedPlayer == null) return;
            object obj;
            if (selectedPlayer.CustomProperties.TryGetValue("RP_PR", out obj)) PerkPoints = (int)obj + 1;
            if (selectedPlayer.CustomProperties.TryGetValue("RP_FR", out obj)) PlayerLevel = (int)obj;
            activePerks = GetActivePerks(selectedPlayer.CustomProperties);
            PerkRef Engineer = activePerks[0]; PerkRef Scavenger = activePerks[3];
            activePerks.RemoveAt(3); activePerks.RemoveAt(0);
            activePerks.Insert(2, Engineer); activePerks.Insert(3, Scavenger);
            buffLevels = GetPerkBuffLevels(selectedPlayer.CustomProperties);
        }
        public override void Draw(Player selectedPlayer)
        {
            _cachedSkin = GUISkin.Instantiate(GUI.skin);
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label($"Perk Points: {PerkPoints - spentPerks}");
                        GUILayout.Label($"Player Level: {PlayerLevel}");
                        if (GUILayout.Button("Refresh")) Refresh(selectedPlayer);
                    }
                    using (new GUILayout.HorizontalScope())
                    {
                        spentPerks = 0;
                        foreach (PerkRef perkref in activePerks)
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                GUILayout.Label(perkref.Filename.Substring(9));
                                GUI.skin = _defaultSkin;
                                GUILayout.HorizontalSlider(0, 100, 100);
                                GUI.skin = _cachedSkin;
                                foreach (PerkBuffRef perkBuffRef in perkref.Asset.Buffs)
                                {
                                    PerkBuff asset = perkBuffRef.Asset;
                                    if (asset != null && asset.DisplayName != "Coming Soon!")
                                    {
                                        int buffLevel = 0;
                                        if (buffLevels.ContainsKey(perkBuffRef.AssetGuid))
                                        {
                                            buffLevel = buffLevels[perkBuffRef.AssetGuid];
                                        }
                                        if (buffLevel > 0)
                                        {
                                            using (new GUILayout.HorizontalScope())
                                            {
                                                spentPerks += buffLevel;
                                                GUILayout.Label(asset.DisplayType == PerkDisplayType.Hero ? $"<color=yellow><b>{asset.DisplayName}</b></color>" : (asset.DisplayType == PerkDisplayType.Wide ? $"<color=orange>{asset.DisplayName}</color>" : asset.DisplayName));
                                                GUILayout.Label(buffLevel.ToString(), new GUILayoutOption[] { GUILayout.Width(32f) });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public List<PerkRef> GetActivePerks(Hashtable hashtable)
        {
            List<PerkRef> perks = new List<PerkRef>();

            object obj;
            if (!hashtable.TryGetValue("ACP", out obj))
                return perks;

            int[] array = (int[])obj;
            for (int i = 0; i < array.Length / 4; i++)
            {
                perks.Add(new PerkRef(new int[]
                {
                    array[i * 4],
                    array[i * 4 + 1],
                    array[i * 4 + 2],
                    array[i * 4 + 3]
                }));
            }
            return perks;
        }

        public Dictionary<GUIDUnion, int> GetPerkBuffLevels(Hashtable hashtable)
        {
            Dictionary<GUIDUnion, int> perkLevels = new Dictionary<GUIDUnion, int>();

            object obj;
            if (!hashtable.TryGetValue("PBO", out obj) && !hashtable.TryGetValue("PBL", out obj))
                return perkLevels;

            int[] array = (int[])obj;
            for (int i = 0; i < array.Length / 5; i++)
            {
                perkLevels.Add(new GUIDUnion(new int[]
                {
                    array[i * 5],
                    array[i * 5 + 1],
                    array[i * 5 + 2],
                    array[i * 5 + 3]
                }), array[i * 5 + 4]);
            }
            return perkLevels;
        }
    }
}
