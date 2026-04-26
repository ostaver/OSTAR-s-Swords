using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OSTARsSWORDS.Content.Buffs.WoltazhaBuff;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Projectiles
{
    public class TeskiBoomerang : ModProjectile
    {
        public override string Texture => "OSTARsSWORDS/Content/Items/Swords/Teski/Teski";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // Records rotation too
        }

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false; // Bypasses blocks for more chaos
            Projectile.extraUpdates = 1; // Moves faster and smoother update rate
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0; // Ignore immunity cooldown entirely
            Projectile.damage = 270; // Base damage, can be modified by the weapon that shoots it
        }

        public override void AI()
        {
            // Set rotation
            Projectile.rotation += 0.35f * Projectile.direction;

            int flightTime = 40; // time before returning
            Projectile.ai[0]++;

            if (Projectile.ai[0] > flightTime)
            {
                Projectile.ai[1] = 1f; // Returning
                Projectile.tileCollide = false;
            }

            if (Projectile.ai[1] == 1f) // Returning logic
            {
                Player player = Main.player[Projectile.owner];
                if (!player.active || player.dead)
                {
                    Projectile.Kill();
                    return;
                }

                Vector2 diff = player.Center - Projectile.Center;
                float dist = diff.Length();
                if (dist < 50f)
                {
                    Projectile.Kill();
                    return;
                }

                float maxSpeed = 30f; // return speed
                diff.Normalize();
                diff *= maxSpeed;
                
                // Steer towards player smoothly
                Projectile.velocity = (Projectile.velocity * 25f + diff) / 26f;
            }

            // Visuals
            Lighting.AddLight(Projectile.Center, new Vector3(0.5f, 0.75f, 1.2f));

            if (Main.rand.NextBool(2))
            {
                Dust ice = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(50, 50), DustID.FireworksRGB, Projectile.velocity * -0.2f);
                ice.scale = Main.rand.NextFloat(0.4f, 0.9f);
                ice.noGravity = true;
                ice.color = Main.rand.NextBool(3) ? Color.LightBlue : Color.CornflowerBlue;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int lightningType = ModContent.ProjectileType<TeskiLightning>();
            
            // Spawn 1-2 lightning strikes occasionally on hit
            if (Main.rand.NextFloat() < 0.4f || hit.Crit)
            {
                int count = hit.Crit ? Main.rand.Next(3, 6) : Main.rand.Next(1, 4);
                
                // Play a loud thunder sound if crit
                if (hit.Crit)
                {
                    SoundEngine.PlaySound(SoundID.Thunder with { Volume = 0.5f, Pitch = 0.2f }, target.Center);
                    target.StrikeNPC(new NPC.HitInfo()
                    {
                        Damage = damageDone * (int)Main.rand.NextFloat(100f, 500f),
                        Crit = true
                    });
                }

                for (int i = 0; i < count; i++)
                {
                    Vector2 strikePos = target.Center + Main.rand.NextVector2Circular(60, 60);
                    Projectile.NewProjectile(
                        Projectile.GetSource_OnHit(target),
                        strikePos, Vector2.Zero,
                        lightningType, 0, 0f,
                        Projectile.owner,
                        strikePos.X, strikePos.Y);
                }
            }

            if (hit.Crit)
            {
                target.AddBuff(BuffID.Slow, 1800);
                target.AddBuff(BuffID.Ichor, 1800);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("OSTARsSWORDS/Content/Items/Swords/Teski/Teski").Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextGlow").Value;
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f);

            // Motion blur/afterimage
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float oldAlpha = 1f - (i / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, null, lightColor * 0.4f * oldAlpha, Projectile.oldRot[i], origin, Projectile.scale, SpriteEffects.None, 0);
            }

            // Glow back
            Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            // Main sword
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            // Core glow
            Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, null, Color.White * 0.3f, Projectile.rotation, glowTex.Size() / 2f, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            return false;
        }
    }
}
