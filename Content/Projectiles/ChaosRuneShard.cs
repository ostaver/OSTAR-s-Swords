using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OSTARsSWORDS.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Projectiles;

public class ChaosRuneShard : ModProjectile
{
	// A clean, readable vanilla texture that takes dyes/glow nicely.
	public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.NebulaBolt;

	private const int ModeSerpentine = 0;
	private const int ModeHunt = 1;
	private const int ModeRicochet = 2;
	private const int ModeOrbitLaunch = 3;
	private const int ModeBlink = 4;

	public override void SetDefaults()
	{
		Projectile.width = 22;
		Projectile.height = 22;
		Projectile.scale = 0.8f;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.penetrate = 2;
		Projectile.timeLeft = 220;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;

		Projectile.extraUpdates = 1;
	}

	public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
	{
		// ai[0] = mode, ai[1] = seed/extra (set by the item)
		Projectile.localAI[0] = Main.rand.Next(1_000_000);
	}

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		int mode = (int)Projectile.ai[0];
		int seed = (int)Projectile.localAI[0];
		float t = Projectile.timeLeft * 0.075f + seed * 0.0007f;

		float hue = (seed * 0.0000013f + Main.GlobalTimeWrappedHourly * 0.35f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		Lighting.AddLight(Projectile.Center, chroma.ToVector3() * 0.45f);
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		// Ambient Luminance particles: a "stuttering prism" trail.
		if (Main.rand.NextBool(3))
		{
			Particle1Glow p = new()
			{
				Position = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f),
				Velocity = Projectile.velocity * -0.1f + Main.rand.NextVector2Circular(1.5f, 1.5f),
				RotationSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Vector2.One * Main.rand.NextFloat(0.25f, 0.55f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(18, 32)
			};
			p.Spawn();
		}

		if (Main.rand.NextBool(5))
		{
			ChromaticStreak s = new()
			{
				Position = Projectile.Center,
				Velocity = Projectile.velocity * -0.15f,
				RotationSpeed = Main.rand.NextFloat(-0.08f, 0.08f),
				Scale = Vector2.One * Main.rand.NextFloat(0.2f, 0.35f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(10, 18),
				Stretch = Main.rand.NextFloat(2.2f, 3.6f)
			};
			s.Spawn();
		}

		// Mode behaviors.
		switch (mode)
		{
			case ModeSerpentine:
				SerpentineMotion(t);
				break;

			case ModeHunt:
				HomingMotion();
				break;

			case ModeRicochet:
				RicochetMotion();
				break;

			case ModeOrbitLaunch:
				OrbitThenLaunch(owner);
				break;

			case ModeBlink:
				BlinkStrike(owner);
				break;
		}
	}

	private void SerpentineMotion(float t)
	{
		float wiggle = (float)Math.Sin(t) * 0.06f + (float)Math.Sin(t * 0.41f) * 0.04f;
		Projectile.velocity = Projectile.velocity.RotatedBy(wiggle) * 1.01f;
		ClampSpeed(12f, 26f);
	}

	private void HomingMotion()
	{
		NPC target = FindTarget(560f);
		if (target is null)
		{
			ClampSpeed(10f, 28f);
			return;
		}

		float speed = MathHelper.Clamp(Projectile.velocity.Length() * 1.02f, 12f, 30f);
		Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * speed;
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.10f);
		Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * speed;
	}

	private void RicochetMotion()
	{
		Projectile.tileCollide = true;

		// Periodic "chaos kick" so it doesn't just bounce forever in a boring pattern.
		if (Projectile.timeLeft % 18 == 0)
			Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(1.02f, 1.10f);

		ClampSpeed(11f, 32f);
	}

	private void OrbitThenLaunch(Player owner)
	{
		Projectile.tileCollide = false;
		if (!owner.active || owner.dead)
		{
			Projectile.ai[0] = ModeSerpentine;
			return;
		}

		// ai[1] used as a tiny timer for this mode.
		Projectile.ai[1]++;

		if (Projectile.ai[1] < 22f)
		{
			float angle = (Projectile.localAI[0] * 0.0002f + Projectile.ai[1] * 0.55f) * owner.direction;
			Vector2 orbitPos = owner.MountedCenter + angle.ToRotationVector2() * 54f;
			Vector2 toPos = orbitPos - Projectile.Center;
			Projectile.velocity = toPos * 0.28f;
			return;
		}

		Vector2 launchDir = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.UnitX);
		Projectile.velocity = launchDir.RotatedBy(Main.rand.NextFloat(-0.45f, 0.45f)) * Main.rand.NextFloat(18f, 28f);
		Projectile.ai[0] = Main.rand.NextBool(2) ? ModeHunt : ModeSerpentine;
	}

	private void BlinkStrike(Player owner)
	{
		Projectile.tileCollide = false;

		// ai[1] is chain count for blink mode.
		if (Projectile.localAI[1] == 0f)
		{
			Projectile.localAI[1] = 1f;
			NPC target = FindTarget(720f);
			if (target != null)
			{
				Vector2 offset = Main.rand.NextVector2CircularEdge(64f, 64f);
				Projectile.Center = target.Center + offset;
				Projectile.velocity = (-offset).SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(18f, 32f);

				SoundEngine.PlaySound(SoundID.Item72 with { PitchVariance = 0.25f, Volume = 0.5f }, Projectile.Center);
			}
			else
			{
				Projectile.ai[0] = ModeHunt;
			}
		}

		// After the blink, behave like a fast serpent with occasional micro-homing.
		NPC maybe = FindTarget(420f);
		if (maybe != null && Main.rand.NextBool(4))
		{
			Vector2 desired = (maybe.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * MathHelper.Clamp(Projectile.velocity.Length() * 1.03f, 18f, 34f);
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.18f);
		}
		else
		{
			float chaos = (float)Math.Sin((Projectile.timeLeft + Projectile.localAI[0]) * 0.11f) * 0.085f;
			Projectile.velocity = Projectile.velocity.RotatedBy(chaos);
		}

		ClampSpeed(16f, 36f);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if ((int)Projectile.ai[0] != ModeRicochet)
			return true;

		Projectile.penetrate--;
		if (Projectile.penetrate <= 0)
			return true;

		if (Projectile.velocity.X != oldVelocity.X)
			Projectile.velocity.X = -oldVelocity.X;
		if (Projectile.velocity.Y != oldVelocity.Y)
			Projectile.velocity.Y = -oldVelocity.Y;

		Projectile.velocity *= 0.9f;

		BurstParticles(10);
		SoundEngine.PlaySound(SoundID.Item10 with { PitchVariance = 0.2f, Volume = 0.45f }, Projectile.Center);

		// On the last bounce, split into two serpents.
		if (Projectile.penetrate == 1 && Projectile.owner == Main.myPlayer)
		{
			for (int i = 0; i < 2; i++)
			{
				Vector2 v = oldVelocity.RotatedBy(i == 0 ? -0.45f : 0.45f) * 0.85f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, v, Type, (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner, ModeSerpentine, 0f);
			}
		}

		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		BurstParticles(16);

		// Rarely chain a blink shard to another target.
		if ((int)Projectile.ai[0] == ModeBlink && Projectile.owner == Main.myPlayer && Main.rand.NextBool(5) && Projectile.ai[1] < 2f)
		{
			Projectile.ai[1] += 1f;
			NPC next = FindTarget(720f, target.whoAmI);
			if (next != null)
			{
				Vector2 dir = (next.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, dir * 24f, Type, (int)(Projectile.damage * 0.9f), Projectile.knockBack, Projectile.owner, ModeBlink, Projectile.ai[1]);
			}
		}
	}

	private void BurstParticles(int count)
	{
		int seed = (int)Projectile.localAI[0];
		float hue = (seed * 0.0000013f + Main.GlobalTimeWrappedHourly * 0.35f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		for (int i = 0; i < count; i++)
		{
			Vector2 v = Main.rand.NextVector2Circular(10f, 10f);
			Particle1Glow p = new()
			{
				Position = Projectile.Center,
				Velocity = v,
				RotationSpeed = Main.rand.NextFloat(-0.25f, 0.25f),
				Scale = Vector2.One * Main.rand.NextFloat(0.35f, 0.9f),
				DrawColor = chroma,
				Lifetime = Main.rand.Next(14, 28)
			};
			p.Spawn();
		}
	}

	private NPC FindTarget(float maxDistance, int ignoreWhoAmI = -1)
	{
		NPC closest = null;
		float best = maxDistance;

		foreach (NPC npc in Main.ActiveNPCs)
		{
			if (npc.whoAmI == ignoreWhoAmI)
				continue;
			if (!npc.CanBeChasedBy(Projectile))
				continue;

			float d = Vector2.Distance(npc.Center, Projectile.Center);
			if (d < best)
			{
				best = d;
				closest = npc;
			}
		}
		return closest;
	}

	private void ClampSpeed(float min, float max)
	{
		float s = Projectile.velocity.Length();
		if (s < min)
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * min;
		else if (s > max)
			Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * max;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
		Vector2 origin = texture.Size() / 2f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

		int seed = (int)Projectile.localAI[0];
		float hue = (seed * 0.0000013f + Main.GlobalTimeWrappedHourly * 0.35f) % 1f;
		Color chroma = Main.hslToRgb(hue, 1f, 0.65f);

		Color glow = chroma * 0.65f;
		Main.EntitySpriteDraw(texture, drawPos, null, glow, Projectile.rotation, origin, Projectile.scale * 1.35f, SpriteEffects.None, 0);

		Color core = Color.White;
		Main.EntitySpriteDraw(texture, drawPos, null, core, Projectile.rotation, origin, Projectile.scale * 0.95f, SpriteEffects.None, 0);

		return false;
	}
}

