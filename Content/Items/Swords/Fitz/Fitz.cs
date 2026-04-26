using Microsoft.Xna.Framework;
using OSTARsSWORDS.Rarities;
using OSTARsSWORDS.Content.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Fitz;

public class Fitz : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 70;
		Item.height = 90;
		Item.scale = 1.2f;
		Item.rare = ModContent.RarityType<AbyssalBlue>();
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 22;
		Item.useAnimation = 22;
		Item.damage = 165;
		Item.knockBack = 7.5f;
		Item.UseSound = SoundID.Item71;
		Item.shoot = ModContent.ProjectileType<FitzProjectile>();
		Item.shootSpeed = 14f;
		Item.value = Item.sellPrice(0, 35, 0, 0);
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}

	public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
	{
		// Explosive glow burst on melee impact
		float hue = (Main.GlobalTimeWrappedHourly * 0.5f) % 1f;
		Color chroma = Main.hslToRgb(hue, 0.9f, 0.6f);

		for (int i = 0; i < 10; i++)
		{
			Vector2 v = Main.rand.NextVector2Circular(8f, 8f);
			Particle1Glow p = new()
			{
				Position = target.Center + Main.rand.NextVector2Circular(10f, 10f),
				Velocity = v,
				RotationSpeed = Main.rand.NextFloat(-0.25f, 0.25f),
				Scale = Vector2.One * Main.rand.NextFloat(0.35f, 0.85f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(14, 28)
			};
			p.Spawn();
		}

		// Debuffs for the explosive / fiery feel
		target.AddBuff(BuffID.OnFire3, 300);
		target.AddBuff(BuffID.ShadowFlame, 240);
		if (!target.boss)
		{ 
			target.StrikeInstantKill();
		}	
	}

	public override void MeleeEffects(Player player, Rectangle hitbox)
	{
		float hue = (Main.GlobalTimeWrappedHourly * 0.5f) % 1f;
		Color chroma = Main.hslToRgb(hue, 0.9f, 0.6f);

		// Dark fire dust trail
		if (Main.rand.NextBool(2))
		{
			int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.PurpleTorch, 0f, 0f, 100, chroma, 1.4f);
			Main.dust[d].noGravity = true;
			Main.dust[d].velocity *= 0.3f;
		}

		// Luminance chromatic streaks during swing
		if (Main.rand.NextBool(2))
		{
			ChromaticStreak s = new()
			{
				Position = new Vector2(Main.rand.Next(hitbox.Left, hitbox.Right), Main.rand.Next(hitbox.Top, hitbox.Bottom)),
				Velocity = player.velocity * 0.2f + Main.rand.NextVector2Circular(2.5f, 2.5f),
				RotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f),
				Scale = Vector2.One * Main.rand.NextFloat(0.22f, 0.38f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(10, 18),
				Stretch = Main.rand.NextFloat(2.4f, 4.4f)
			};
			s.Spawn();
		}
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		// Offset spawn to the front of the swing
		position += Vector2.Normalize(velocity) * 40f;

		// Shoot the rotating Fitz projectile
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

		return false;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.LunarBar, 10);
		recipe.AddIngredient(ItemID.FragmentSolar, 8);
		recipe.AddIngredient(ItemID.FragmentNebula, 8);
		recipe.AddIngredient(ItemID.Ectoplasm, 6);
		recipe.AddTile(TileID.LunarCraftingStation);
		recipe.Register();
	}
}
