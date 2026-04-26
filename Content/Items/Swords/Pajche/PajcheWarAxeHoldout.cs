using System;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Pajche
{
    public class PajcheWarAxeHoldout : BaseHoldoutSword, ILocalizedModType
    {
        #region Sound Styles (inlined from Hellkite item)
        public static readonly SoundStyle SwingSound = new("OSTARsSWORDS/Sounds/Item/HellkiteSwing", 2);
        public static readonly SoundStyle SwingSoundBig = new("OSTARsSWORDS/Sounds/Item/HellkiteHeavySwing");
        public static readonly SoundStyle HitSoundSmall = new("OSTARsSWORDS/Sounds/Item/HellkiteSmallHit", 3);
        public static readonly SoundStyle HitSoundBig = new("OSTARsSWORDS/Sounds/Item/HellkiteBigHit", 2);
        public static readonly SoundStyle ChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteCharge");
        public static readonly SoundStyle FullChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteFullCharge");
        public static readonly SoundStyle Pluck = new("OSTARsSWORDS/Sounds/Item/PluckHigh");
        #endregion

        public override int AssignedItemID => Mod.Find<ModItem>("PajcheWarAxe").Type;

        public override string Texture => "OSTARsSWORDS/Content/Items/Swords/Pajche/PajcheWarAxe";

        public override void UseStyle()
        {
            AnimationProgress = Animation % (chargedSwing ? (int)(storedUseAnim * 0.7f) : storedUseAnim);

            // Vanilla right-click detection
            if (IsRightClickHeld)
                Projectile.ai[2] = 5;

            DrawUnconditionally = false;
            bool cantUse = (Owner == null || !Owner.active || Owner.dead || (Projectile.ai[2] == 0 && !Owner.channel) || (Projectile.ai[2] == 5 && !IsRightClickHeld) || Owner.CCed || Owner.noItems);

            if (CanHit_Field || postSwing)
                mousePos = Owner.Center - aimVel;
            else
            {
                mousePos = Main.MouseWorld;
            }

            if (CanHit_Field)
                fadeIn = MathHelper.Lerp(fadeIn, 1, (chargedSwing ? 1.5f : 1) * 0.23f * Owner.GetAttackSpeed<MeleeDamageClass>());
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.3f);
            if (chargeTimer > 0)
                fadeIn = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0, 1f);

            // If you are no longer holding the charge, then stop charge counter so you can swing
            if (cantUse)
            {
                chargeTimer = 0;
                if (Projectile.ai[2] == 5)
                {
                    Owner.itemAnimation = Owner.itemAnimationMax;
                    Projectile.timeLeft = Owner.itemAnimation;
                }
                Projectile.ai[2] = 0;
            }

            if (!doSwing)
            {
                playSwingSound = true;
                mousePos = Main.MouseWorld;
                aimVel = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit_Field = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1 ? true : false;

                Vector2 bladePos = new Vector2(60, 0);
                Vector2 particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));

                if (Projectile.ai[2] == 5)
                {
                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction), 0.05f);

                    float rotationValue = 45f + (25 * Utils.GetLerpValue(0, chargeTimerMax, chargeTimer, true)) * (FlipAsSword ? 1 : -1) * -Projectile.ai[1];
                    Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(rotationValue), 0.3f);
                    Animation = 0;
                    Owner.itemAnimation++;
                    Projectile.timeLeft++;

                    if (chargeTimer < chargeTimerMax && !chargedSwing)
                        chargeTimer++;

                    Vector2 particleVel = (Owner.Center - particlePos).SafeNormalize(Vector2.UnitX) * -15;
                    particlePos += Main.rand.NextVector2Circular(20, 20);

                    // Vanilla dust replacement for GlowOrbParticle + dust
                    Dust dust2 = Dust.NewDustPerfect(particlePos, DustID.RainbowMk2, particleVel * Main.rand.NextFloat(0.2f, 1));
                    dust2.scale = Main.rand.NextFloat(0.65f, 1.15f) * fadeIn * GFBMulti;
                    dust2.noGravity = true;
                    dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;

                    Dust glowDust = Dust.NewDustPerfect(particlePos, DustID.FireworksRGB, particleVel * Main.rand.NextFloat(0.2f, 1.5f));
                    glowDust.scale = Main.rand.NextFloat(0.2f, 0.4f) * fadeIn * GFBMulti;
                    glowDust.noGravity = true;
                    glowDust.color = Main.rand.NextBool(3) ? Color.Red : Color.OrangeRed;

                    if (SoundEngine.TryGetActiveSound(AudSlot, out var chargeSnd) && chargeSnd.IsPlaying)
                    {
                        chargeSnd.Position = Projectile.Center;
                        chargeSnd.Pitch = Utils.Remap(chargeTimer, 0, chargeTimerMax, -0.4f, 0f);
                        chargeSnd.Volume = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0f, 0.5f) * 100;
                    }
                    else if (!chargedSwing)
                    {
                        AudSlot = SoundEngine.PlaySound(ChargeSound with { Volume = 0.01f, Pitch = 0, IsLooped = true }, Projectile.Center);
                    }
                }
                if (chargeTimer == chargeTimerMax || GFBCharge >= (MathHelper.Clamp(chargeTimerMax - (GFBMulti * GFBMulti * 2), 12, 80)) && GFBCharge > 0)
                {
                    particlePos = Owner.Center + (bladePos).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                    SoundEngine.PlaySound(FullChargeSound with { Volume = 0.9f, PitchVariance = 0.2f }, Projectile.Center);
                    chargedSwing = true;
                    useAnim = storedUseAnim / 3;
                    chargeTimer++;

                    // Vanilla dust replacement for LineParticle burst
                    for (int i = 0; i < 20; i++)
                    {
                        Dust sparkDust = Dust.NewDustPerfect(particlePos, DustID.FireworksRGB, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f));
                        sparkDust.scale = Main.rand.NextFloat(0.5f, 1f);
                        sparkDust.noGravity = true;
                        sparkDust.color = Main.rand.NextBool(3) ? Color.Red : Color.OrangeRed;

                        Dust dust3 = Dust.NewDustPerfect(particlePos, DustID.RainbowMk2, new Vector2(8, 8).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f));
                        dust3.scale = Main.rand.NextFloat(0.65f, 1.15f) * fadeIn;
                        dust3.noGravity = true;
                        dust3.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                    }
                }

                if (chargeTimer == 0)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                        Projectile.localNPCImmunity[i] = 0;

                    Projectile.numHits = 0;
                    pierceReduction = 0;
                    doSwing = true;
                }
            }
            else if (chargeTimer == 0)
            {
                if (SoundEngine.TryGetActiveSound(AudSlot, out var chargeSnd))
                    chargeSnd?.Stop();

                if (!CanHit_Field && !postSwing)
                {
                    if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }
                else
                {
                    if ((Owner.Center - aimVel).X < Owner.Center.X) Owner.direction = -1;
                    else Owner.direction = 1;
                }

                Projectile.rotation = Projectile.rotation.AngleLerp(Owner.AngleTo(mousePos) + MathHelper.ToRadians(45f), 0.1f);

                if (AnimationProgress < (useAnim / 1.5f))
                {
                    if (Projectile.ai[2] == 5 && !chargedSwing)
                        doSwing = false;

                    aimVel = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                    CanHit_Field = false;
                    postSwing = false;
                    if (AnimationProgress == 0)
                    {
                        GFBMulti = 1;
                        GFBCharge = 0;
                        Animation = 0;
                        doSwing = false;
                        chargeTimer = 0;
                        chargedSwing = false;
                        useAnim = storedUseAnim;
                        Projectile.ai[1] = -Projectile.ai[1];
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction * (1 + (Utils.GetLerpValue(useAnim * 0.35f, useAnim * 0.6f, Animation, true)) * 0.5f)), 0.2f);
                    FlipAsSword = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX).X > 0 ? true : false;
                }
                else
                {
                    float time = (AnimationProgress) - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && playSwingSound)
                    {
                        if (!chargedSwing)
                        {
                            SoundEngine.PlaySound(SwingSound with { Volume = 0.8f, PitchVariance = 0.25f }, Projectile.Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(SwingSound with { Volume = 0.9f, Pitch = 0.2f }, Projectile.Center);
                        }
                        swingCount++;
                        playSwingSound = false;
                    }
                    if (time > (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && time < (int)(timeMax * (chargedSwing ? 0.9f : 0.7f)))
                    {
                        CanHit_Field = true;

                        Vector2 particleVel = new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                        if (chargedSwing)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                particleVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction) * Main.rand.NextFloat(0.3f, 1f)).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                                particlePos = Owner.Center + (new Vector2(Main.rand.Next(30, 170), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)));
                                // Vanilla dust replacement for AltSparkParticle + HeavySmokeParticle
                                Dust spark = Dust.NewDustPerfect(particlePos, DustID.FireworksRGB, -particleVel.RotatedByRandom(0.4f));
                                spark.scale = Main.rand.NextFloat(0.3f, 0.7f);
                                spark.noGravity = true;
                                spark.color = Main.rand.NextBool(3) ? Color.OrangeRed : Color.DarkRed;

                                Dust smoke = Dust.NewDustPerfect(particlePos, DustID.Smoke, -particleVel.RotatedByRandom(0.4f));
                                smoke.scale = Main.rand.NextFloat(0.5f, 1f);
                                smoke.noGravity = true;
                                smoke.color = Main.rand.NextBool(4) ? Color.Red : Color.DarkRed;

                                Dust smoke2 = Dust.NewDustPerfect(particlePos, DustID.Smoke, -particleVel.RotatedByRandom(0.4f) * 2);
                                smoke2.scale = Main.rand.NextFloat(0.5f, 1f);
                                smoke2.noGravity = true;
                                smoke2.color = Main.rand.NextBool(4) ? Color.Crimson : Color.Red;
                            }
                        }
                        else
                        {
                            // Vanilla dust replacement for AltSparkParticle + HeavySmokeParticle
                            Dust spark = Dust.NewDustPerfect(particlePos, DustID.FireworksRGB, -particleVel.RotatedByRandom(0.2f));
                            spark.scale = Main.rand.NextFloat(0.3f, 0.7f);
                            spark.noGravity = true;
                            spark.color = Main.rand.NextBool(3) ? Color.OrangeRed : Color.DarkRed;

                            Dust smoke = Dust.NewDustPerfect(particlePos, DustID.Smoke, -particleVel.RotatedByRandom(0.2f) * 2);
                            smoke.scale = Main.rand.NextFloat(0.5f, 1f);
                            smoke.noGravity = true;
                            smoke.color = Main.rand.NextBool(4) ? Color.Red : Color.DarkRed;
                        }
                    }
                    else
                    {
                        CanHit_Field = false;
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset, MathHelper.ToRadians(MathHelper.Lerp(150f * Projectile.ai[1] * Owner.direction, 120f * -Projectile.ai[1] * Owner.direction, ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time < (int)(timeMax * 0.9f))
                    {
                        postSwing = true;
                    }

                    if (CanHit_Field)
                    {
                        if (chargedSwing)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                float randRot = Main.rand.NextFloat(-10, -45);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                // Vanilla dust replacement for PointParticle
                                Dust point = Dust.NewDustPerfect(Owner.Center + (new Vector2(170, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot)).RotatedByRandom(0.4f)), DustID.FireworksRGB, -dustVel * Main.rand.NextFloat(0.4f, 0.7f));
                                point.scale = Main.rand.NextFloat(0.7f, 1.2f);
                                point.noGravity = true;
                                point.color = (Main.rand.NextBool(4) ? Color.Orange : Color.OrangeRed) * 0.8f;
                            }
                            for (int i = 0; i < 4; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.3f, 0.9f));
                                dust2.scale = Main.rand.NextFloat(0.65f, 0.95f);
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float randRot = Main.rand.NextFloat(-30, -60);
                                Vector2 dustVel = (new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction)).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                                Dust dust2 = Dust.NewDustPerfect(Owner.Center + (new Vector2(170, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f)), DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                                dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                                dust2.noGravity = true;
                                dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                            }
                        }
                    }
                }
            }

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // If you are hitting an armored target or kill a target, don't reduce damage based on enemy hits
            if ((damageDone <= 2 || (target.life <= 0 && target.realLife == -1)) && pierceReduction > 0)
            {
                pierceReduction -= 1;
            }

            if (!chargedSwing)
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(HitSoundSmall with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                    ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 4.5f);
                }
                for (int i = 0; i < MathHelper.Clamp(8 - Projectile.numHits * 2, 2, 8); i++)
                {
                    // Vanilla dust replacement for LineParticle
                    Dust spark = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, ((Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitY) * -20).RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f));
                    spark.scale = Main.rand.NextFloat(0.3f, 1f);
                    spark.noGravity = true;
                    spark.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;

                    if (Main.rand.NextBool())
                    {
                        Dust spark2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, ((Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitY) * -20).RotatedByRandom(0.7) * Main.rand.NextFloat(0.2f, 1f));
                        spark2.scale = Main.rand.NextFloat(0.3f, 1f);
                        spark2.noGravity = true;
                        spark2.color = Color.DarkRed;
                    }
                }
            }
            else
            {
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(HitSoundBig with { Volume = 1f }, Projectile.Center);
                    SoundEngine.PlaySound(SwingSoundBig with { Volume = 0.9f, Pitch = -.25f }, Projectile.Center);
                    ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 8.5f * GFBMulti);

                    // Vanilla dust replacement for CustomPulse blast rings
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            Dust ring = Dust.NewDustPerfect(target.Center, DustID.RainbowMk2, new Vector2(Main.rand.NextFloat(3f, 8f) * (i + 1) * GFBMulti, 0).RotatedByRandom(MathHelper.TwoPi));
                            ring.scale = Main.rand.NextFloat(0.5f, 1f);
                            ring.noGravity = true;
                            ring.color = Color.OrangeRed;
                        }
                    }

                    // Vanilla dust replacement for GlowSparkParticle
                    for (int i = 0; i < 2; i++)
                    {
                        Dust sparkGlow = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitY) * -25 * (i == 0 ? -1 : 1));
                        sparkGlow.scale = 0.08f * GFBMulti;
                        sparkGlow.noGravity = true;
                        sparkGlow.color = Color.OrangeRed;
                    }

                    for (int i = 0; i < 15; i++)
                    {
                        // Vanilla dust replacement for SparkParticle + AltSparkParticle
                        Dust sparkDust = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, ((Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitY) * -40).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                        sparkDust.scale = Main.rand.NextFloat(0.5f, 1.2f) * GFBMulti;
                        sparkDust.noGravity = true;
                        sparkDust.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;

                        if (Main.rand.NextBool())
                        {
                            Dust altSpark = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, ((Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitY) * -40).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f));
                            altSpark.scale = Main.rand.NextFloat(0.5f, 1.2f) * GFBMulti;
                            altSpark.noGravity = true;
                            altSpark.color = Color.DarkRed;
                        }

                        Dust dust2 = Dust.NewDustPerfect(target.Center, DustID.FireworksRGB, new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1));
                        dust2.scale = Main.rand.NextFloat(0.55f, 0.85f) * GFBMulti;
                        dust2.noGravity = true;
                        dust2.color = Main.rand.NextBool(3) ? Color.Orange : Color.OrangeRed;
                    }
                }
            }

            ////Inlined: MoveNPC
            //Vector2 launchVel = Utils.DirectionTo(Owner.Center, Main.MouseWorld);
            //MoveNPC(target, launchVel, chargedSwing ? 24 : 19, true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Only draw if the projectile's owner is using the item
            if ((useAnim > 0 || DrawUnconditionally) && (Owner.ItemAnimationActive || IsRightClickHeld))
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                //Asset<Texture2D> glowTex = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextSparkle");
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextGlow");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                SpriteEffects sEffects = spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                for (int i = 0; i < 25; i++)
                {
                    Texture2D centerTexture = ModContent.Request<Texture2D>("OSTARsSWORDS/Content/Items/Swords/Pajche/PajcheWarAxe").Value;
                    Color auraColor = Color.OrangeRed with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * (chargeTimer > 0 ? 4 : 7) * fadeIn;
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + drawOffset + new Vector2(0, Owner.gfxOffY), centerTexture.Frame(1, FrameCount, 0, Frame), auraColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None));
                }

                if (swingCount > 0 && Projectile.ai[2] != 5 && !playSwingSound)
                    Main.EntitySpriteDraw(swoosh.Value, Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY), null, Color.Lerp(Color.DarkRed, Color.OrangeRed, 0.25f) with { A = 0 } * fadeIn * 0.9f, (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -80 : 80) * -Owner.direction, swoosh.Size() * 0.5f, Projectile.scale * 2.35f / 4, swingCount % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                Main.EntitySpriteDraw(tex.Value, generalDrawPos, tex.Frame(1, FrameCount, 0, Frame), lightColor, Projectile.rotation + RotationOffset + r, FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin, Projectile.scale, sEffects);
            }
            return false;
        }
    }
}
