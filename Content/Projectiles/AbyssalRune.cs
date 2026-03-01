using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Luminance.Core.Graphics;
using OSTARsSWORDS.Content.Particles;
using System;

namespace OSTARsSWORDS.Content.Projectiles;

public class AbyssalRune : ModProjectile
{
	public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.DemonScythe;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.scale = 0.8f;
		Projectile.friendly = true;
		Projectile.penetrate = 3;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 300;
		Projectile.alpha = 255; // start invisible to fade in
	}

	public override void AI()
	{
		// Fade in
		if (Projectile.alpha > 0)
			Projectile.alpha -= 15;
		if (Projectile.alpha < 0)
			Projectile.alpha = 0;

		Projectile.rotation += 0.2f * (Projectile.velocity.X > 0 ? 1f : -1f);

		// Emit dust
		if (Main.rand.NextBool(2))
		{
			Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, 0, 0, 100, default, 1.2f);
			dust.noGravity = true;
			dust.velocity *= 0.5f;
		}

		// AI state 0: pause and gather energy
		if (Projectile.ai[0] < 30)
		{
			Projectile.velocity *= 0.95f; // Slow down quickly
			Projectile.ai[0]++;
		}
		else
		{
			// AI state 1: Homing
			// Increase speed exponentially
			float speed = Projectile.velocity.Length();
			if (speed < 4f) speed = 4f;
			speed *= 1.05f;
			if (speed > 25f) speed = 25f;

			NPC closestTarget = null;
			float shortestDistance = 800f; // Long homing range
			
			foreach (var target in Main.ActiveNPCs)
			{
				if (target.CanBeChasedBy(Projectile))
				{
					float dist = Vector2.Distance(target.Center, Projectile.Center);
					if (dist < shortestDistance)
					{
						shortestDistance = dist;
						closestTarget = target;
					}
				}
			}

			if (closestTarget != null)
			{
				Vector2 desiredDirection = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
				// Turn Strength: ease in
				float turnStrength = 0.08f;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredDirection * speed, turnStrength);
				Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
			}
			else
			{
				Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed; // maintain direction
			}
		}

		// Luminance bloom effect randomly
		if (Main.rand.NextBool(5))
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(10f, 10f),
				Velocity = Projectile.velocity * -0.2f,
				RotationSpeed = Main.rand.NextFloat(-0.1f, 0.1f),
				Scale = Vector2.One * Main.rand.NextFloat(0.3f, 0.6f),
				DrawColor = Color.DarkViolet,
				Lifetime = Main.rand.Next(20, 40)
			};
			p.Spawn();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 origin = texture.Size() / 2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

		// Draw bloom layer
		Color glowColor = Color.Purple * 0.6f * ((255 - Projectile.alpha) / 255f);
		Main.EntitySpriteDraw(texture, drawPos, null, glowColor, Projectile.rotation, origin, Projectile.scale * 1.5f, SpriteEffects.None, 0);

		// Draw main sprite
		Color mainColor = Color.White * ((255 - Projectile.alpha) / 255f);
		Main.EntitySpriteDraw(texture, drawPos, null, mainColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 15; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 0, default, 1.5f);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, Projectile.velocity.X * -0.2f, Projectile.velocity.Y * -0.2f, 0, default, 1.5f);
		}
		
		for (int i = 0; i < 5; i++)
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center,
				Velocity = Main.rand.NextVector2Circular(8f, 8f),
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.5f, 0.9f),
				DrawColor = Color.MediumPurple,
				Lifetime = Main.rand.Next(15, 30)
			};
			p.Spawn();
		}

		SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.ShadowFlame, 300);
	}
}
