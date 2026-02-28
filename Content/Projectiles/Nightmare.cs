using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Luminance.Core.Graphics;
using OSTARsSWORDS.Content.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
namespace OSTARsSWORDS.Content.Projectiles;

public class Nightmare : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 100;
		Projectile.scale = 0.5f;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Main.projFrames[Projectile.type] = 4;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 60;
		Projectile.Opacity = 1.0f;
	}

	public override void AI()
	{
		Dust obj = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 0, default, 1f);
		obj.noGravity = true;
		obj.scale = 1f;
		Dust obj2 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 0, default, 1f);
		obj2.noGravity = true;
		obj2.scale = 1.5f;
		Dust obj3 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PurpleTorch, 0f, 0f, 0, default, 1f);
		obj3.noGravity = true;
		obj3.scale = 2f;

		// Spawn Luminance particles for a continuous trail/bloom effect
		if (Main.rand.NextBool(3))
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width * 0.4f, Projectile.height * 0.4f),
				Velocity = Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(1f, 1f),
				RotationSpeed = Main.rand.NextFloat(-0.1f, 0.1f),
				Scale = Vector2.One * Main.rand.NextFloat(0.4f, 0.7f),
				DrawColor = Color.Purple,
				Lifetime = Main.rand.Next(25, 45)
			};
			p.Spawn();
		}
		if (Projectile.velocity.X < 0f)
		{
			Projectile.spriteDirection = -1;
		}
		if (Projectile.velocity.X > 0f)
		{
		Projectile.spriteDirection = 1;
		}

		// Target finding and exponential homing
		NPC closestTarget = null;
		float shortestDistance = 480f;
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
			// ai[0] counts how many frames we've been homing
			Projectile.ai[0]++;

			// Get current speed and apply exponential growth (4% per frame)
			float speed = Projectile.velocity.Length();
			if (speed < 12f) speed = 12f; 
			speed *= 1.04f;
			if (speed > 40f) speed = 40f;

			// Turning logic: Calculate the direction to the target
			Vector2 desiredDirection = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
			
			// Turn Strength: Starts very low (0.02) and grows over time (0.005 per frame)
			// This creates an "ease-in" for the turn itself, making it more of a curve than a snap.
			float turnStrength = MathHelper.Clamp(0.02f + (Projectile.ai[0] * 0.005f), 0.02f, 0.3f);

			// Gradually rotate the velocity towards the target
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredDirection * speed, turnStrength);
			
			// Re-normalize and apply speed to ensure we don't lose velocity during the Lerp
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
		}

		// Fade out effect based on timeLeft
		Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f);

		// Animation Logic
		Projectile.frameCounter++;
		if (Projectile.frameCounter >= 8)
		{
			Projectile.frame++;
			Projectile.frameCounter = 0;
			if (Projectile.frame >= Main.projFrames[Projectile.type])
			{
				Projectile.frame = 0;
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		int frameHeight = texture.Height / Main.projFrames[Projectile.type];
		Rectangle sourceRectangle = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
		Vector2 origin = sourceRectangle.Size() / 2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
		SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

		// 1. Draw the "Bloom" / Glow layer first
		// We use additive-like color blending with Opacity
		Color glowColor = Color.Purple * 0.5f * Projectile.Opacity;
		Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, glowColor, Projectile.rotation, origin, Projectile.scale * 1.3f, effects, 0);

		// 2. Draw the main sprite
		// We use Projectile.GetAlpha to ensure it respects the fade-out Opacity
		Color mainColor = Projectile.GetAlpha(lightColor);
		Main.EntitySpriteDraw(texture, drawPos, sourceRectangle, mainColor, Projectile.rotation, origin, Projectile.scale, effects, 0);

		return false; // Return false to prevent the default drawing, which fixes the "shadowy offset" issue
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 10; i++)
		{
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurpleTorch, Projectile.oldVelocity.X * 0.1f, Projectile.oldVelocity.Y * 0.1f, 0, default(Color), 1f);
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurpleTorch, Projectile.oldVelocity.X * 0.1f, Projectile.oldVelocity.Y * 0.1f, 0, default(Color), 1f);
		}

        // Luminance particle burst on kill
        for (int i = 0; i < 15; i++)
		{
			Vector2 velocity = Main.rand.NextVector2Circular(14f, 14f);
			Particle1Glow p = new()
			{
				Position = Projectile.Center,
				Velocity = velocity,
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.5f, 1f),
				DrawColor = Color.Violet,
				Lifetime = Main.rand.Next(30, 50)
			};
			p.Spawn();
		}

		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Player owner = Main.player[Projectile.owner];
		if (Main.rand.Next(2) == 0)
		{
			owner.statLife += 2;
			owner.HealEffect(2, true);
		}

		// Big shine bloom flash
		Particle1Glow bloomParticle = new()
		{
			Position = target.Center,
			Velocity = Vector2.Zero,
			RotationSpeed = Main.rand.NextFloat(-0.02f, 0.02f),
			Scale = Vector2.One * 3f, // Massive scale for shine/bloom!
			DrawColor = Color.Fuchsia,
			Lifetime = 25
		};
		bloomParticle.Spawn();

		for (int i = 0; i < 8; i++)
		{
			Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
			Particle1Glow p = new()
			{
				Position = target.Center,
				Velocity = velocity,
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.4f, 0.7f),
				DrawColor = Color.Magenta,
				Lifetime = Main.rand.Next(20, 40)
			};
			p.Spawn();
		}

		if (target.life <= 0)
		{
			ScreenShakeSystem.StartShake(6f, 4f, null, 0.5f);
		}
	}
}
