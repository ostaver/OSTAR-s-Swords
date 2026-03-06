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

namespace OSTARsSWORDS.Content.Projectiles;

public class EntropyNova : ModProjectile
{
	public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.InfernoFriendlyBolt;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.scale = 0.1f;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.penetrate = -1;
		Projectile.timeLeft = 16;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 9999; // effectively once per NPC
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		if (!owner.active || owner.dead)
		{
			Projectile.Kill();
			return;
		}

		Projectile.Center = owner.MountedCenter;

		// ai[0] holds consumed entropy (0..100) from the item.
		float entropy = MathHelper.Clamp(Projectile.ai[0], 0f, 100f);
		float strength = 0.35f + entropy / 140f; // 0.35..1.06

		// Expand rapidly; hitbox follows scale via ModifyDamageHitbox.
		// Make the nova feel huge at high entropy.
		Projectile.scale = MathHelper.Lerp(Projectile.scale, 1.2f + strength * 2.6f, 0.38f);
		Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 16f, 0f, 1f);

		float hue = (Main.GlobalTimeWrappedHourly * 0.45f + entropy * 0.006f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		Lighting.AddLight(Projectile.Center, chroma.ToVector3() * 1.1f * Projectile.Opacity);

		// Particle storm: dense early, then taper, with entropy chaos baked into the count.
		int count = (int)MathHelper.Lerp(52f + entropy * 0.2f, 12f, 1f - Projectile.Opacity);
		for (int i = 0; i < count; i++)
		{
			// Chaotic radial velocity: wobble the radius to avoid a perfect circle.
			Vector2 baseVel = Main.rand.NextVector2Circular(18f, 18f) * strength;
			float wobble = (float)Math.Sin((Projectile.timeLeft * 0.6f) + i * 0.45f) * 0.35f;
			Vector2 vel = baseVel.RotatedBy(wobble);
			Particle1Glow p = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
				Velocity = vel,
				RotationSpeed = Main.rand.NextFloat(-0.25f, 0.25f),
				Scale = Vector2.One * Main.rand.NextFloat(0.35f, 1.1f) * strength,
				DrawColor = chroma,
				Lifetime = Main.rand.Next(18, 36)
			};
			p.Spawn();

			if (Main.rand.NextBool(2))
			{
				ChromaticStreak s = new()
				{
				Position = Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
				Velocity = vel * Main.rand.NextFloat(0.45f, 0.8f),
					RotationSpeed = Main.rand.NextFloat(-0.06f, 0.06f),
					Scale = Vector2.One * Main.rand.NextFloat(0.25f, 0.45f) * strength,
					DrawColor = chroma,
					Lifetime = Main.rand.Next(10, 16),
					Stretch = Main.rand.NextFloat(2.8f, 5.0f) * strength
				};
				s.Spawn();
			}
		}

		// One-time punch on spawn.
		if (Projectile.localAI[0] == 0f)
		{
			Projectile.localAI[0] = 1f;
			SoundEngine.PlaySound(SoundID.Item84 with { PitchVariance = 0.25f, Volume = 0.85f }, Projectile.Center);
			ScreenShakeSystem.StartShake(6f + strength * 10f, 2f, null, 0.7f);
		}

		// Secondary micro-shakes and "breathing" feel while the ring expands.
		if (Projectile.timeLeft % 4 == 0)
		{
			float micro = 0.5f + strength * 1.5f;
			ScreenShakeSystem.StartShake(micro, 0.3f, null, 0.25f);
		}
	}

	public override void ModifyDamageHitbox(ref Rectangle hitbox)
	{
		// Significantly larger base radius, then scaled by the nova growth.
		int radius = (int)(220f * Projectile.scale);
		hitbox = new Rectangle((int)Projectile.Center.X - radius, (int)Projectile.Center.Y - radius, radius * 2, radius * 2);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		// Extra chaos damage: spread 10% of max HP across 10 randomized chunks.
		if (Projectile.owner == Main.myPlayer)
		{
			int totalBonus = (int)(target.lifeMax * 0.10f);
			if (totalBonus > 0)
			{
				// Generate random weights that sum to 1, then distribute totalBonus.
				float[] weights = new float[10];
				float sum = 0f;
				for (int i = 0; i < 10; i++)
				{
					weights[i] = Main.rand.NextFloat(0.2f, 1.2f);
					sum += weights[i];
				}

				int accumulated = 0;
				for (int i = 0; i < 10; i++)
				{
					int chunk = (int)(totalBonus * (weights[i] / sum));
					// Ensure we always reach exactly totalBonus (avoid rounding loss).
					if (i == 9)
						chunk = totalBonus - accumulated;

					if (chunk <= 0)
						continue;

					accumulated += chunk;

					int dir = Main.rand.NextBool() ? -1 : 1;
					NPC.HitInfo extra = new()
					{
						Damage = chunk,
						HitDirection = dir,
						Knockback = hit.Knockback * Main.rand.NextFloat(0.6f, 1.4f),
						Crit = false
					};
					target.StrikeNPC(extra);
					Main.player[Projectile.owner].addDPS(chunk);
				}
			}
		}

		// Debuff roulette — "use chaos".
		switch (Main.rand.Next(6))
		{
			case 0:
				target.AddBuff(BuffID.Confused, 120);
				break;
			case 1:
				target.AddBuff(BuffID.ShadowFlame, 240);
				break;
			case 2:
				target.AddBuff(BuffID.CursedInferno, 180);
				break;
			case 3:
				target.AddBuff(BuffID.Ichor, 240);
				break;
			case 4:
				target.AddBuff(BuffID.OnFire3, 180);
				break;
			default:
				target.AddBuff(BuffID.Weak, 180);
				break;
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 origin = texture.Size() / 2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

		float entropy = MathHelper.Clamp(Projectile.ai[0], 0f, 100f);
		float hue = (Main.GlobalTimeWrappedHourly * 0.45f + entropy * 0.006f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 14f) * 0.15f + 0.85f;

		// Soft bloom ring.
		Color bloom = chroma * 0.45f * Projectile.Opacity;
		Main.EntitySpriteDraw(texture, drawPos, null, bloom, 0f, origin, Projectile.scale * (2.0f + pulse * 0.35f), SpriteEffects.None, 0);

		// White-hot core.
		Color core = Color.White * 0.65f * Projectile.Opacity;
		Main.EntitySpriteDraw(texture, drawPos, null, core, 0f, origin, Projectile.scale * (1.2f + pulse * 0.15f), SpriteEffects.None, 0);

		return false;
	}
}

