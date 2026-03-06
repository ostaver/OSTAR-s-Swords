using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Luminance.Core.Graphics;
using OSTARsSWORDS.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Fitz;

public class FitzProjectile : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 60;
		Projectile.height = 60;
		Projectile.scale = 1.1f;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.penetrate = 3;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
	}

	public override void AI()
	{
		// Constant spinning
		Projectile.rotation += 0.35f * Projectile.direction;
		if (Projectile.direction == 0)
			Projectile.rotation += 0.35f;

		// Slight gravity pull
		Projectile.velocity.Y += 0.04f;

		// Cycling hue for the glow effects — dark purple to green matching the sprite
		float hue = (Main.GlobalTimeWrappedHourly * 0.6f + Projectile.identity * 0.1f) % 1f;
		Color glowColor = Main.hslToRgb(hue, 0.85f, 0.55f);

		// Dynamic light
		Lighting.AddLight(Projectile.Center, glowColor.ToVector3() * 0.8f);

		// Trailing glow particles
		if (Main.rand.NextBool(2))
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(16f, 16f),
				Velocity = -Projectile.velocity * 0.15f + Main.rand.NextVector2Circular(2f, 2f),
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.3f, 0.7f),
				DrawColor = glowColor,
				Lifetime = Main.rand.Next(12, 24)
			};
			p.Spawn();
		}

		// Chromatic streaks for extra juice
		if (Main.rand.NextBool(3))
		{
			ChromaticStreak s = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f),
				Velocity = -Projectile.velocity * 0.25f + Main.rand.NextVector2Circular(1.5f, 1.5f),
				RotationSpeed = Main.rand.NextFloat(-0.06f, 0.06f),
				Scale = Vector2.One * Main.rand.NextFloat(0.2f, 0.4f),
				DrawColor = glowColor,
				Lifetime = Main.rand.Next(8, 14),
				Stretch = Main.rand.NextFloat(2.5f, 4.5f)
			};
			s.Spawn();
		}

		// Dust trail
		if (Main.rand.NextBool(2))
		{
			int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 100, default, 1.2f);
			Main.dust[d].noGravity = true;
			Main.dust[d].velocity = -Projectile.velocity * 0.1f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// Explosive hit — screen shake and particle burst
		ScreenShakeSystem.StartShake(4f, 0.6f, null, 0.6f);

		float hue = (Main.GlobalTimeWrappedHourly * 0.6f + Projectile.identity * 0.1f) % 1f;
		Color burstColor = Main.hslToRgb(hue, 0.9f, 0.6f);

		// Explosion particle burst
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(8f, 8f);
			Particle1Glow p = new()
			{
				Position = target.Center + Main.rand.NextVector2Circular(10f, 10f),
				Velocity = vel,
				RotationSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Scale = Vector2.One * Main.rand.NextFloat(0.4f, 1.0f),
				DrawColor = burstColor,
				Lifetime = Main.rand.Next(14, 28)
			};
			p.Spawn();
		}

		for (int i = 0; i < 6; i++)
		{
			ChromaticStreak s = new()
			{
				Position = target.Center,
				Velocity = Main.rand.NextVector2Circular(10f, 10f),
				RotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f),
				Scale = Vector2.One * Main.rand.NextFloat(0.3f, 0.5f),
				DrawColor = burstColor,
				Lifetime = Main.rand.Next(10, 16),
				Stretch = Main.rand.NextFloat(3f, 5.5f)
			};
			s.Spawn();
		}

		// On Fire debuff for explosive feel
		target.AddBuff(BuffID.OnFire3, 240);
		target.AddBuff(BuffID.ShadowFlame, 180);
	}

	public override void OnKill(int timeLeft)
	{
		// Explosion on death
		SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

		float hue = (Main.GlobalTimeWrappedHourly * 0.6f) % 1f;
		Color deathColor = Main.hslToRgb(hue, 0.85f, 0.6f);

		// Explosion dust
		for (int i = 0; i < 20; i++)
		{
			int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 80, default, 1.6f);
			Main.dust[d].noGravity = true;
			Main.dust[d].velocity = Main.rand.NextVector2Circular(6f, 6f);
		}

		// Final burst of Luminance particles
		for (int i = 0; i < 16; i++)
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center,
				Velocity = Main.rand.NextVector2Circular(10f, 10f),
				RotationSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Scale = Vector2.One * Main.rand.NextFloat(0.5f, 1.1f),
				DrawColor = deathColor,
				Lifetime = Main.rand.Next(16, 30)
			};
			p.Spawn();
		}

		// Small explosion damage area
		if (Projectile.owner == Main.myPlayer)
		{
			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ProjectileID.DD2ExplosiveTrapT3Explosion, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
		}

		ScreenShakeSystem.StartShake(5f, 0.8f, null, 0.5f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 origin = texture.Size() / 2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

		float hue = (Main.GlobalTimeWrappedHourly * 0.6f + Projectile.identity * 0.1f) % 1f;
		Color glowColor = Main.hslToRgb(hue, 0.85f, 0.55f);

		float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 10f) * 0.15f + 0.85f;

		// Bloom behind the projectile
		Color bloom = glowColor * 0.5f * Projectile.Opacity;
		Main.EntitySpriteDraw(texture, drawPos, null, bloom, Projectile.rotation, origin, Projectile.scale * (1.4f + pulse * 0.2f), SpriteEffects.None, 0);

		// White-hot core glow
		Color core = Color.White * 0.3f * Projectile.Opacity;
		Main.EntitySpriteDraw(texture, drawPos, null, core, Projectile.rotation, origin, Projectile.scale * (1.15f + pulse * 0.1f), SpriteEffects.None, 0);

		// Main sprite
		Main.EntitySpriteDraw(texture, drawPos, null, lightColor * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

		return false;
	}
}
