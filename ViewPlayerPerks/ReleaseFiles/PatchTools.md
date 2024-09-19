```c#
		public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
		{
			if (targetPlayer.ActorNumber == base.photonView.Owner.ActorNumber)
			{
				if (changedProps.ContainsKey("ACP"))
				{
					this.UpdateActivePerks((int[])changedProps["ACP"]);
					return;
				}
				if (changedProps.ContainsKey("PBL"))
				{
					this.UpdatePerkBuffLevels((int[])changedProps["PBL"]);
					Action onPerkBuffLevelsUpdated = PlayerPerkLoader.OnPerkBuffLevelsUpdated;
					if (onPerkBuffLevelsUpdated == null)
					{
						return;
					}
					onPerkBuffLevelsUpdated();
				}
			}
		}
```

```c#
private void UpdatePerkBuffLevels(int[] serializedPerkBuffLevels)
		{
			this.PerkBuffLevels.Clear();
			for (int i = 0; i < serializedPerkBuffLevels.Length / 5; i++)
			{
				this.PerkBuffLevels.Add(new GUIDUnion(new int[]
				{
					serializedPerkBuffLevels[i * 5],
					serializedPerkBuffLevels[i * 5 + 1],
					serializedPerkBuffLevels[i * 5 + 2],
					serializedPerkBuffLevels[i * 5 + 3]
				}), serializedPerkBuffLevels[i * 5 + 4]);
			}
			this.ReApplyPerks();
		}
```