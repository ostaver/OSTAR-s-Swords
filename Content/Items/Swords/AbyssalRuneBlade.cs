using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Projectiles;
using Luminance.Core.Graphics;
using System.Collections.Generic;
using System;

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
		Item.useTime = 22;
		Item.useAnimation = 22;
		Item.damage = 190;
		Item.knockBack = 8.5f;
		Item.UseSound = SoundID.Item71;
		Item.shoot = ModContent.ProjectileType<AbyssalRune>();
		Item.shootSpeed = 22f;
		Item.value = Item.sellPrice(0, 45, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.ShadowFlame, 600);
		ScreenShakeSystem.StartShake(4f, 1f, null, 0.7f);
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		if (Main.rand.NextBool(3))
		{
			int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Shadowflame, 0f, 0f, 100, default, 1.5f);
			Main.dust[d].noGravity = true;
			int d2 = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.5f);
			Main.dust[d2].noGravity = true;
		}
	}
	
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		int numberProjectiles = 3;
		float rotation = MathHelper.ToRadians(15);
		position += Vector2.Normalize(velocity) * 45f;
		for (int i = 0; i < numberProjectiles; i++)
		{
			Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1f))) * .2f; // Make them start slow
			Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false; // we manually spawned the projectiles
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
