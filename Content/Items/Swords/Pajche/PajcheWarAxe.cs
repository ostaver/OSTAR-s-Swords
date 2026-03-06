using Microsoft.Xna.Framework;
using Luminance.Core.Graphics;
using OSTARsSWORDS.Content.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Pajche;

public class PajcheWarAxe : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 70;
		Item.height = 80;
		Item.scale = 2.0f;
		Item.rare = ItemRarityID.Yellow;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.damage = 130;
		Item.knockBack = 9f;
		Item.UseSound = SoundID.Item71;
		Item.value = Item.sellPrice(0, 25, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		// Heavy impact — strong screen shake
		ScreenShakeSystem.StartShake(6f, 1f, null, 0.7f);

		// Bonus damage based on target's max health (8% of max HP)
		if (player.whoAmI == Main.myPlayer)
		{
			int bonusDamage = (int)(target.lifeMax * 0.08f);
			if (bonusDamage > 0)
			{
				NPC.HitInfo bonus = new()
				{
					Damage = bonusDamage,
					HitDirection = hit.HitDirection,
					Knockback = hit.Knockback * 0.5f,
					Crit = false
				};
				target.StrikeNPC(bonus);
				player.addDPS(bonusDamage);
			}
		}

		// Crimson impact particles — blood and iron
		Color impactColor = new(200, 60, 40);
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(7f, 7f);
			Particle1Glow p = new()
			{
				Position = target.Center + Main.rand.NextVector2Circular(12f, 12f),
				Velocity = vel,
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.4f, 0.9f),
				DrawColor = impactColor,
				Lifetime = Main.rand.Next(14, 26)
			};
			p.Spawn();
		}

		// Heavy gore dust burst
		for (int i = 0; i < 8; i++)
		{
			int d = Dust.NewDust(target.position, target.width, target.height, DustID.Blood, 0f, 0f, 80, default, 1.8f);
			Main.dust[d].noGravity = false;
			Main.dust[d].velocity = Main.rand.NextVector2Circular(5f, 5f);
		}

		// Debuffs — the weight of the axe
		target.AddBuff(BuffID.BrokenArmor, 300);
		target.AddBuff(BuffID.Slow, 180);
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		// Heavy red/orange dust — sparks flying off the axe
		if (Main.rand.NextBool(2))
		{
			int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Torch, 0f, 0f, 100, default, 1.3f);
			Main.dust[d].noGravity = true;
			Main.dust[d].velocity *= 0.4f;
		}

		// Ground-shatter sparks
		if (Main.rand.NextBool(3))
		{
			Color sparkColor = new(220, 80, 30);
			ChromaticStreak s = new()
			{
				Position = new Vector2(Main.rand.Next(hitbox.Left, hitbox.Right), Main.rand.Next(hitbox.Top, hitbox.Bottom)),
				Velocity = player.velocity * 0.15f + Main.rand.NextVector2Circular(2f, 2f),
				RotationSpeed = Main.rand.NextFloat(-0.06f, 0.06f),
				Scale = Vector2.One * Main.rand.NextFloat(0.2f, 0.35f),
				DrawColor = sparkColor,
				Lifetime = Main.rand.Next(8, 14),
				Stretch = Main.rand.NextFloat(2f, 3.8f)
			};
			s.Spawn();
		}
	}

	public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, "MaxHPDamage", "Deals bonus 8% of target's max health on hit") { OverrideColor = new Color(200, 60, 40) });
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.HellstoneBar, 15);
		recipe.AddIngredient(ItemID.Bone, 25);
		recipe.AddIngredient(ItemID.SoulofMight, 8);
		recipe.AddTile(TileID.MythrilAnvil);
		recipe.Register();
	}
}
