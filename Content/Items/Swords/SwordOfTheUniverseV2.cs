using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Projectiles;
using Terraria.ID;

namespace OSTARsSWORDS.Content.Items.Swords;

public class SwordOfTheUniverseV2 : ModItem
{
	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.width = 140;
		Item.height = 140;
		Item.scale = 2f;
		Item.rare = ItemRarityID.Purple;
		Item.crit = 16;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 15;
		Item.useAnimation = 15;
		Item.damage = 1337;
		Item.knockBack = 20f;
		Item.UseSound = new SoundStyle("UniverseOfSwordsModOld/Sounds/Item/GiantExplosion", (SoundType)0);
		Item.shoot = ModContent.ProjectileType<SOTUProjectile2>();
		Item.shootSpeed = 30f;
		Item.expert = true;
		Item.value = Item.sellPrice(10, 0, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}

	public override void AddRecipes()
	{
		Recipe recipe2 = CreateRecipe();
		recipe2.AddRecipeGroup("Any Sword Of The Universe", 1);
		recipe2.Register();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 2f, velocity.Y + 2f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 2f, velocity.Y - 2f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 4f, velocity.Y + 4f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 4f, velocity.Y - 4f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 6f, velocity.Y + 6f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 6f, velocity.Y - 6f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 8f, velocity.Y + 8f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 8f, velocity.Y - 8f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 10f, velocity.Y + 10f, ProjectileID.VortexBeaterRocket, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 1f, velocity.Y + 1f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 1f, velocity.Y - 1f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 2f, velocity.Y + 2f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 2f, velocity.Y - 2f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 3f, velocity.Y + 3f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 3f, velocity.Y - 3f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 4f, velocity.Y + 4f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X - 4f, velocity.Y - 4f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		Projectile.NewProjectile(source, position.X, position.Y, velocity.X + 5f, velocity.Y + 5f, ProjectileID.InfluxWaver, damage, knockback, player.whoAmI, 0f, 0f, 0f);
		return true;
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.Midas, 360, false);
		target.AddBuff(BuffID.Ichor, 360, false);
		target.AddBuff(BuffID.Frostburn, 360, false);
		target.AddBuff(BuffID.OnFire, 360, false);
		target.AddBuff(BuffID.Poisoned, 360, false);
		target.AddBuff(BuffID.CursedInferno, 360, false);
	}
}
