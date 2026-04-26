using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Projectiles;
using System.Collections.Generic;
using Luminance.Core.Graphics;
using Terraria.Audio;
using OSTARsSWORDS.Content.Globals;

namespace OSTARsSWORDS.Content.Items.Swords;

public class TheNightmareAmalgamation : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 110;
		Item.height = 110;
		Item.scale = 0.9f;
		Item.rare = ItemRarityID.Purple;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.damage = 150;
		Item.knockBack = 10f;
		Item.UseSound = SoundID.Item71;
		Item.shoot = ModContent.ProjectileType<Nightmare>();
		Item.shootSpeed = 15f;
		Item.value = Item.sellPrice(0, 30, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 1;
			Item.useAnimation = 1;
			Item.noUseGraphic = true;
			Item.noMelee = true;

			var modPlayer = player.GetModPlayer<GlobalPlayer>();
			if (modPlayer.nightmareAbilityCooldownTimer <= 0)
			{
				modPlayer.nightmareAbilityActive = true;
				modPlayer.nightmareAbilityDurationTimer = 600; // 10 seconds
				modPlayer.nightmareAbilityCooldownTimer = 1800; // 30 seconds
				Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("OSTARsSWORDS/Content/Items/Swords/NightmareUnleash") {Volume = 1.5f, Pitch = 0.25f}, player.position);

				ScreenShakeSystem.StartShake(10f, 1f, null, 1.5f);

				for (int i = 0; i < 40; i++)
				{
					Vector2 velocity = Main.rand.NextVector2Circular(10f, 10f);
					Dust.NewDust(player.position, player.width, player.height, DustID.Shadowflame, velocity.X, velocity.Y, 150, default(Color), 2f);
				}

				return true;
			}
			return false;
		}
		else
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.noUseGraphic = false;
			Item.noMelee = false;
		}
		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		var modPlayer = Main.LocalPlayer.GetModPlayer<GlobalPlayer>();
		
		if (modPlayer.nightmareAbilityCooldownTimer > 0)
		{
			int seconds = (modPlayer.nightmareAbilityCooldownTimer / 60) + 1;
			tooltips.Add(new TooltipLine(Mod, "Cooldown", $"Ability Cooldown: {seconds}s") { OverrideColor = Color.Red });
		}
		else
		{
			tooltips.Add(new TooltipLine(Mod, "Cooldown", "Ability Ready!") { OverrideColor = Color.LimeGreen });
        }
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2 || !player.GetModPlayer<GlobalPlayer>().nightmareAbilityActive)
			return false;

		for (int i = 0; i < 3; i++)
		{
			Vector2 perturbedSpeed = Utils.RotatedByRandom(new Vector2(velocity.X, velocity.Y), (double)MathHelper.ToRadians(12f));
			Projectile.NewProjectile(source, position.X + i*10, position.Y+i*10, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<Nightmare>(), damage, knockback, player.whoAmI, 0f, 0f, 0f);
		}
		return false;
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		if (Main.rand.Next(1) == 0)
		{
			int dust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurpleTorch, 0f, 0f, 100, default(Color), 2f);
			Main.dust[dust].noGravity = true;
			dust = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurpleTorch, 0f, 0f, 100, default(Color), 2f);
			Main.dust[dust].noGravity = true;
		}
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.ShadowFlame, 800, false);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(Mod, "CthulhuJudge", 1);
		recipe.AddIngredient(Mod, "StickyGlowstickSword", 1);
		recipe.AddIngredient(Mod, "TheEater", 1);
		recipe.AddIngredient(ItemID.BeeKeeper, 1);
		recipe.AddIngredient(Mod, "SwordOfPower", 1);
		recipe.AddIngredient(ItemID.BreakerBlade, 1);
		recipe.AddIngredient(Mod, "PrimeSword", 1);
		recipe.AddIngredient(Mod, "DestroyerSword", 1);
		recipe.AddIngredient(Mod, "TwinsSword", 1);
		recipe.AddIngredient(Mod, "Executioner", 1);
		recipe.AddIngredient(Mod, "Golem", 1);
		recipe.AddIngredient(Mod, "Doomsday", 1);
		recipe.AddIngredient(Mod, "Sharkron", 1);
		recipe.AddTile(TileID.LunarCraftingStation);
		recipe.Register();
		Recipe recipe2 = CreateRecipe();
		recipe2.AddIngredient(Mod, "CthulhuJudge", 1);
		recipe2.AddIngredient(Mod, "StickyGlowstickSword", 1);
		recipe2.AddIngredient(Mod, "TheBrain", 1);
		recipe2.AddIngredient(ItemID.BeeKeeper, 1);
		recipe2.AddIngredient(Mod, "SwordOfPower", 1);
		recipe2.AddIngredient(ItemID.BreakerBlade, 1);
		recipe2.AddIngredient(Mod, "PrimeSword", 1);
		recipe2.AddIngredient(Mod, "DestroyerSword", 1);
		recipe2.AddIngredient(Mod, "TwinsSword", 1);
		recipe2.AddIngredient(Mod, "Executioner", 1);
		recipe2.AddIngredient(Mod, "Golem", 1);
		recipe2.AddIngredient(Mod, "Doomsday", 1);
		recipe2.AddIngredient(Mod, "Sharkron", 1);
		recipe2.AddTile(TileID.LunarCraftingStation);
		recipe2.Register();
	}
}
