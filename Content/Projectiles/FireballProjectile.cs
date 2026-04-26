using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Projectiles;

public class FireballProjectile : ModProjectile
{
	private const int TrailLength = 12;
	private static readonly SoundStyle FR_HIT = new SoundStyle("OSTARsSWORDS/Sounds/FR_HIT")
	{
		Volume = 0.5f,
		Pitch = 0.3f
	};

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLength;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
	}

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.scale = 0.85f;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 3;
		Projectile.timeLeft = 180;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.extraUpdates = 1;
		Projectile.light = 0.6f;
	}

	public override void AI()
	{
		// Point the sword along its travel direction
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

		// Pulsing scale for a living, breathing feel
		float pulse = 1f + (float)Math.Sin(Projectile.ai[0] * 0.15f) * 0.04f;
		Projectile.scale = 0.85f * pulse;
		Projectile.ai[0]++;

		// Dense fire dust trail
		for (int d = 0; d < 2; d++)
		{
			Dust dust = Dust.NewDustDirect(
				Projectile.position + Projectile.velocity * Main.rand.NextFloat(-0.5f, 0f),
				Projectile.width, Projectile.height,
				DustID.Torch, 0f, 0f, 100, default, 2.2f);
			dust.noGravity = true;
			dust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.1f, 0.3f);
			dust.fadeIn = 1.2f;
		}

		// Bright red shine particles
		if (Main.rand.NextBool(2))
		{
			Dust glow = Dust.NewDustDirect(
				Projectile.Center - new Vector2(6), 12, 12,
				DustID.RedTorch, 0f, 0f, 0, default, 1.8f);
			glow.noGravity = true;
			glow.velocity = Projectile.velocity * 0.05f + Main.rand.NextVector2Circular(0.5f, 0.5f);
		}

		// Falling embers with gravity
		if (Main.rand.NextBool(3))
		{
			Dust ember = Dust.NewDustDirect(
				Projectile.position, Projectile.width, Projectile.height,
				DustID.Torch,
				-Projectile.velocity.X * 0.15f + Main.rand.NextFloat(-1f, 1f),
				-Projectile.velocity.Y * 0.15f + Main.rand.NextFloat(-0.5f, 1f),
				180, default, 1.4f);
			ember.noGravity = false;
		}

		// Occasional sparkle burst
		if (Main.rand.NextBool(8))
		{
			for (int i = 0; i < 3; i++)
			{
				Dust sparkle = Dust.NewDustDirect(
					Projectile.Center - new Vector2(4), 8, 8,
					DustID.OrangeTorch, 0f, 0f, 0, default, 1f);
				sparkle.noGravity = true;
				sparkle.velocity = Main.rand.NextVector2Circular(2f, 2f);
				sparkle.fadeIn = 0.8f;
			}
		}

		// Flickering warm light
		float flicker = 0.9f + Main.rand.NextFloat(0.2f);
		Lighting.AddLight(Projectile.Center, 1.1f * flicker, 0.35f * flicker, 0.08f * flicker);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 origin = texture.Size() / 2f;
		Vector2 mainPos = Projectile.Center - Main.screenPosition;

		// --- Motion blur trail (back to front) ---
		for (int i = TrailLength - 1; i >= 0; i--)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
				continue;

			float progress = (float)i / TrailLength;
			float trailAlpha = (1f - progress);
			trailAlpha *= trailAlpha; // quadratic falloff for smoother fade
			float trailScale = Projectile.scale * (1f - progress * 0.4f);

			// Color shifts from bright orange-yellow (near) to deep red (far)
			Color trailColor = Color.Lerp(
				new Color(255, 140, 40),
				new Color(180, 20, 5),
				progress
			) * (trailAlpha * 0.5f);

			Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;

			Main.EntitySpriteDraw(
				texture,
				drawPos,
				null,
				trailColor,
				Projectile.oldRot[i],
				origin,
				trailScale,
				SpriteEffects.None,
				0
			);
		}

		// --- Subtle red glow ---
		Color bloomColor = new Color(255, 60, 20) * 0.15f;
		float bloomScale = Projectile.scale * 1.15f;
		for (int b = 0; b < 2; b++)
		{
			Vector2 offset = (MathHelper.TwoPi * b / 2f).ToRotationVector2() * 1.5f;
			Main.EntitySpriteDraw(
				texture,
				mainPos + offset,
				null,
				bloomColor,
				Projectile.rotation,
				origin,
				bloomScale,
				SpriteEffects.None,
				0
			);
		}

		// --- Main sprite (full brightness) ---
		Main.EntitySpriteDraw(
			texture,
			mainPos,
			null,
			Color.White,
			Projectile.rotation,
			origin,
			Projectile.scale,
			SpriteEffects.None,
			0
		);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		// Screen shake
		if (Main.LocalPlayer.Distance(Projectile.Center) < 600f)
		{
			Main.LocalPlayer.GetModPlayer<FireballScreenShake>().Shake(6, 8);
		}

		// Large fire explosion
		for (int i = 0; i < 25; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(5f, 5f);
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height,
				DustID.Torch, vel.X, vel.Y, 80, default, 2.5f);
			dust.noGravity = true;
			dust.fadeIn = 1.5f;
		}

		// Red glow burst
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
			Dust glow = Dust.NewDustDirect(Projectile.Center - new Vector2(8), 16, 16,
				DustID.RedTorch, vel.X, vel.Y, 0, default, 2.2f);
			glow.noGravity = true;
		}

		// Orange sparks flying outward
		for (int i = 0; i < 8; i++)
		{
			Vector2 vel = Main.rand.NextVector2CircularEdge(4f, 4f);
			Dust spark = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 8, 8,
				DustID.OrangeTorch, vel.X, vel.Y, 0, default, 1.5f);
			spark.noGravity = true;
		}

		SoundEngine.PlaySound(FR_HIT with { Volume = 0.5f, Pitch = 0.3f }, Projectile.position);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.OnFire, 360, false);

		// Hit flash — burst of particles on the enemy
		for (int i = 0; i < 6; i++)
		{
			Dust flash = Dust.NewDustDirect(target.position, target.width, target.height,
				DustID.RedTorch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, default, 1.6f);
			flash.noGravity = true;
		}
	}
}

// Small ModPlayer to handle screen shake
public class FireballScreenShake : ModPlayer
{
	private int shakeTimer;
	private int shakeIntensity;

	public void Shake(int intensity, int duration)
	{
		shakeIntensity = intensity;
		shakeTimer = duration;
	}

	public override void ModifyScreenPosition()
	{
		if (shakeTimer > 0)
		{
			float strength = shakeIntensity * ((float)shakeTimer / 8f);
			Main.screenPosition += Main.rand.NextVector2Circular(strength, strength);
			shakeTimer--;
		}
	}
}
