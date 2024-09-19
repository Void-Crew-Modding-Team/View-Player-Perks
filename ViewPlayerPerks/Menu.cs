using ExitGames.Client.Photon;
using Gameplay.Perks;
using Photon.Realtime;
using ResourceAssets;
using System.Collections.Generic;
using UnityEngine;
using VoidManager.CustomGUI;

namespace ViewPlayerPerks
{
    internal class PlayerPerkSettings : PlayerSettingsMenu
    {
        private GUISkin _cachedSkin;
        private GUISkin _defaultSkin;
        private int spentPerks;
        public override void Draw(Player selectedPlayer)
        {
            _cachedSkin = GUISkin.Instantiate(GUI.skin);
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        object obj;
                        if (selectedPlayer.CustomProperties.TryGetValue("RP_PR", out obj)) GUILayout.Label($"Perk Points: {((int)obj + 1) - spentPerks}");
                        if (selectedPlayer.CustomProperties.TryGetValue("RP_FR", out obj)) GUILayout.Label($"Player Level: {(int)obj}");
                    }
                    List<PerkRef> activePerks = [.. GetActivePerks(selectedPlayer.CustomProperties)];
                    PerkRef Engineer = activePerks[0]; PerkRef Scavenger = activePerks[3];
                    activePerks.RemoveAt(3); activePerks.RemoveAt(0);
                    activePerks.Insert(2, Engineer); activePerks.Insert(3, Scavenger);
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
                                    if (asset != null)
                                    {
                                        int buffLevel = asset.GetBuffLevel();
                                        if (asset.DisplayName != "Coming Soon!" && buffLevel > 0)
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
            if (!hashtable.TryGetValue("OCP", out obj) && !hashtable.TryGetValue("ACP", out obj))
                return perks;

            int[] array = (int[])obj;
            for (int i = 0; i < array.Length; i += 4)
            {
                perks.Add(new PerkRef(new int[] { array[i], array[i + 1], array[i + 2], array[i + 3] }));
            }
            return perks;
        }
    }
}
