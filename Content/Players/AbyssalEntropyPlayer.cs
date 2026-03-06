using System;
using Microsoft.Xna.Framework;
using OSTARsSWORDS.Content.Items.Swords;
using Terraria;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Players;

public class AbyssalEntropyPlayer : ModPlayer
{
	public int Entropy;
	public int EntropyDecayDelay;
	public int NovaCooldownTimer;

	public override void ResetEffects()
	{
		// Keep the meter scoped to this weapon to avoid surprising persistence.
		if (Player.HeldItem?.type != ModContent.ItemType<AbyssalRuneBlade>())
		{
			Entropy = 0;
			EntropyDecayDelay = 0;
			NovaCooldownTimer = 0;
		}
	}

	public override void PostUpdate()
	{
		if (NovaCooldownTimer > 0)
			NovaCooldownTimer--;

		if (EntropyDecayDelay > 0)
			EntropyDecayDelay--;
		else if (Entropy > 0)
			Entropy = Math.Max(0, Entropy - 1);
	}

	public void AddEntropy(int amount, int delayFrames = 60)
	{
		Entropy = (int)MathHelper.Clamp(Entropy + amount, 0, 100);
		EntropyDecayDelay = Math.Max(EntropyDecayDelay, delayFrames);
	}

	public int ConsumeAllEntropy()
	{
		int value = Entropy;
		Entropy = 0;
		EntropyDecayDelay = 0;
		return value;
	}
}
