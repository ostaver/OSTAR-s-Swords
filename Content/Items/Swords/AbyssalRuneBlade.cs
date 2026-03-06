using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Luminance.Core.Graphics;
using System;
using OSTARsSWORDS.Content.Particles;
using OSTARsSWORDS.Content.Players;
using OSTARsSWORDS.Content.Projectiles;

namespace OSTARsSWORDS.Content.Items.Swords;

public class AbyssalRuneBlade : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 200;
		Item.height = 200;
		Item.scale = 1.0f;
		Item.rare = ItemRarityID.Purple;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.damage = 190;
		Item.knockBack = 8.5f;
		Item.UseSound = SoundID.Item71;
		Item.shoot = ModContent.ProjectileType<ChaosRuneShard>();
		Item.shootSpeed = 18f;
		Item.value = Item.sellPrice(0, 45, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			var entropyPlayer = player.GetModPlayer<AbyssalEntropyPlayer>();
			if (entropyPlayer.NovaCooldownTimer > 0)
				return false;

			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.noMelee = true;
			Item.noUseGraphic = false;
			Item.shootSpeed = 0f;
			return true;
		}

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 18;
		Item.useAnimation = 18;
		Item.noMelee = false;
		Item.noUseGraphic = false;
		Item.shootSpeed = 18f;
		return base.CanUseItem(player);
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		var entropyPlayer = player.GetModPlayer<AbyssalEntropyPlayer>();
		entropyPlayer.AddEntropy(hit.Crit ? 10 : 6, 90);

		switch (Main.rand.Next(6))
		{
			case 0:
				target.AddBuff(BuffID.ShadowFlame, 360);
				break;
			case 1:
				target.AddBuff(BuffID.Confused, 120);
				break;
			case 2:
				target.AddBuff(BuffID.CursedInferno, 180);
				break;
			case 3:
				target.AddBuff(BuffID.Ichor, 240);
				break;
			case 4:
				target.AddBuff(BuffID.Weak, 180);
				break;
			default:
				target.AddBuff(BuffID.Bleeding, 240);
				break;
		}

		float intensity = 3.5f + entropyPlayer.Entropy * 0.04f;
		ScreenShakeSystem.StartShake(intensity, 1f, null, 0.75f);

		// Luminance hit flash.
		float hue = (Main.GlobalTimeWrappedHourly * 0.45f + entropyPlayer.Entropy * 0.007f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);
		for (int i = 0; i < 14; i++)
		{
			Vector2 v = Main.rand.NextVector2Circular(10f, 10f);
			Particle1Glow p = new()
			{
				Position = target.Center + Main.rand.NextVector2Circular(10f, 10f),
				Velocity = v,
				RotationSpeed = Main.rand.NextFloat(-0.25f, 0.25f),
				Scale = Vector2.One * Main.rand.NextFloat(0.35f, 0.95f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(16, 32)
			};
			p.Spawn();
		}
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		var entropyPlayer = player.GetModPlayer<AbyssalEntropyPlayer>();
		float hue = (Main.GlobalTimeWrappedHourly * 0.6f + entropyPlayer.Entropy * 0.01f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		// Vanilla dust for readability, Luminance particles for the "weirdness".
		if (Main.rand.NextBool(2))
		{
			int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Shadowflame, 0f, 0f, 100, chroma, 1.3f);
			Main.dust[d].noGravity = true;
			Main.dust[d].velocity *= 0.4f;
		}

		if (Main.rand.NextBool(2))
		{
			ChromaticStreak s = new()
			{
				Position = new Vector2(Main.rand.Next(hitbox.Left, hitbox.Right), Main.rand.Next(hitbox.Top, hitbox.Bottom)),
				Velocity = player.velocity * 0.2f + Main.rand.NextVector2Circular(2.5f, 2.5f),
				RotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f),
				Scale = Vector2.One * Main.rand.NextFloat(0.22f, 0.36f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(10, 18),
				Stretch = Main.rand.NextFloat(2.4f, 4.4f)
			};
			s.Spawn();
		}
	}
	
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		var entropyPlayer = player.GetModPlayer<AbyssalEntropyPlayer>();

		// Right-click: dump entropy into a nova.
		if (player.altFunctionUse == 2)
		{
			int entropy = entropyPlayer.ConsumeAllEntropy();
			float strength = 0.35f + entropy / 140f;

			int novaDamage = (int)(damage * (1.25f + strength * 0.9f));
			Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<EntropyNova>(), novaDamage, knockback, player.whoAmI, entropy, 0f);
			entropyPlayer.NovaCooldownTimer = 300;

			// Immediate "reality snap" particles.
			float hue = (Main.GlobalTimeWrappedHourly * 0.45f + entropy * 0.006f) % 1f;
			Color chroma = Main.hslToRgb(hue, 1f, 0.65f);
			for (int i = 0; i < 30 + entropy / 2; i++)
			{
				Vector2 v = Main.rand.NextVector2Circular(12f, 12f) * strength;
				Particle1Glow p = new()
				{
					Position = player.MountedCenter,
					Velocity = v,
					RotationSpeed = Main.rand.NextFloat(-0.25f, 0.25f),
					Scale = Vector2.One * Main.rand.NextFloat(0.35f, 1.0f) * strength,
					DrawColor = chroma,
					Lifetime = Main.rand.Next(20, 40)
				};
				p.Spawn();
			}

			return false;
		}

		// Normal swing: the shard pattern mutates with entropy.
		int entropyNow = entropyPlayer.Entropy;
		int numberProjectiles = 2 + entropyNow / 35; // 2..4
		float spread = MathHelper.ToRadians(10f + entropyNow * 0.10f);

		position += Vector2.Normalize(velocity) * 52f;

		for (int i = 0; i < numberProjectiles; i++)
		{
			Vector2 perturbed = velocity.RotatedBy(MathHelper.Lerp(-spread, spread, numberProjectiles == 1 ? 0.5f : i / (numberProjectiles - 1f)));
			perturbed *= Main.rand.NextFloat(0.65f, 1.05f);

			int mode;
			int roll = Main.rand.Next(100);
			if (entropyNow >= 70 && roll < 18)
				mode = 4; // blink
			else if (roll < 28)
				mode = 2; // ricochet
			else if (roll < 58)
				mode = 1; // hunt
			else if (roll < 78)
				mode = 3; // orbit-launch
			else
				mode = 0; // serpentine

			Projectile.NewProjectile(source, position, perturbed, ModContent.ProjectileType<ChaosRuneShard>(), damage, knockback, player.whoAmI, mode, 0f);
		}

		// Small entropy tick for casting chaos (even on whiffs).
		if (Main.rand.NextBool(3))
			entropyPlayer.AddEntropy(1, 45);

		return false;
	}

	public override void UseItemFrame(Player player)
	{
		// Fancy windup animation for the entropy nova.
		if (player.altFunctionUse != 2 || player.itemAnimation <= 0)
			return;

		// Slightly tilt the sword back and then whip it further as the cast charges.
		float progress = 1f - player.itemAnimation / (float)player.itemAnimationMax;
		float dir = player.direction;
		float whip = (float)Math.Sin(progress * MathHelper.Pi) * 0.45f;
		player.itemRotation = MathHelper.Lerp(-0.45f * dir, -1.75f * dir, progress) + whip * dir;

		var entropyPlayer = player.GetModPlayer<AbyssalEntropyPlayer>();
		int entropy = entropyPlayer.Entropy;

		float hue = (Main.GlobalTimeWrappedHourly * 0.45f + entropy * 0.006f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		// Spawn tightening + expanding rune rings that climb and fall — chaotic lensing.
		Vector2 center = player.MountedCenter;
		float outerRadius = MathHelper.Lerp(96f, 40f, progress);
		float innerRadius = MathHelper.Lerp(40f, 8f, progress);
		float heightOffset = MathHelper.Lerp(32f, -24f, progress);

		for (int i = 0; i < 6; i++)
		{
			float baseAngle = Main.GlobalTimeWrappedHourly * 7f + i * MathHelper.TwoPi / 6f;
			float spiral = progress * 3f;
			float angle = (baseAngle + spiral) * (dir > 0 ? 1f : -1f);

			// Two rings moving in opposite "logic".
			float lerp = (float)Math.Sin(progress * MathHelper.Pi * 0.5f) * 0.5f + 0.5f;
			float r1 = MathHelper.Lerp(outerRadius, innerRadius, lerp);
			float r2 = MathHelper.Lerp(innerRadius * 0.6f, outerRadius * 0.8f, progress);

			Vector2 basis = new((float)Math.Cos(angle), (float)Math.Sin(angle) * 0.35f);
			Vector2 pos1 = center + basis * r1 + new Vector2(0f, heightOffset);
			Vector2 pos2 = center + basis * r2 + new Vector2(0f, -heightOffset * 0.25f);

			if (Main.rand.NextBool(2))
			{
				Particle1Glow p = new()
				{
					Position = pos1,
					Velocity = (center - pos1).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.4f, 1.4f),
					RotationSpeed = Main.rand.NextFloat(-0.15f, 0.15f),
					Scale = Vector2.One * Main.rand.NextFloat(0.25f, 0.55f),
					DrawColor = chroma,
					Lifetime = Main.rand.Next(16, 30)
				};
				p.Spawn();
			}

			if (Main.rand.NextBool(3))
			{
				ChromaticStreak s = new()
				{
					Position = pos2,
					Velocity = (pos2 - center).SafeNormalize(Vector2.One) * Main.rand.NextFloat(2.0f, 5.0f),
					RotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f),
					Scale = Vector2.One * MathHelper.Lerp(0.2f, 0.45f, progress),
					DrawColor = chroma,
					Lifetime = Main.rand.Next(10, 18),
					Stretch = MathHelper.Lerp(2.4f, 5.4f, progress)
				};
				s.Spawn();
			}

			// Occasional vertical pillar flickers to sell the "imploding reality" look.
			if (Main.rand.NextBool(12))
			{
				Vector2 pillarPos = center + new Vector2(basis.X * innerRadius * 0.3f, -32f);
				ChromaticStreak pillar = new()
				{
					Position = pillarPos,
					Velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 7f),
					RotationSpeed = 0f,
					Scale = new Vector2(0.4f, 0.9f),
					DrawColor = chroma,
					Lifetime = Main.rand.Next(12, 20),
					Stretch = 6.5f
				};
				pillar.Spawn();
			}
		}
	}

	public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
	{
		var entropyPlayer = Main.LocalPlayer.GetModPlayer<AbyssalEntropyPlayer>();
		int e = entropyPlayer.Entropy;
		Color c = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.45f + e * 0.006f) % 1f, 1f, 0.65f);

		tooltips.Add(new TooltipLine(Mod, "Entropy", $"Entropy: {e}/100  (Right-click to detonate)") { OverrideColor = c });
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.LunarBar, 12);
		recipe.AddIngredient(ItemID.FragmentNebula, 10);
		recipe.AddIngredient(ItemID.Ectoplasm, 10);
		recipe.AddIngredient(ItemID.DarkShard, 2);
		recipe.AddTile(TileID.LunarCraftingStation);
		recipe.Register();
	}
}
