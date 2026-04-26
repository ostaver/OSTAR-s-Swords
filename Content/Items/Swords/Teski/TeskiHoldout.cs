using System;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OSTARsSWORDS.Content.Buffs.WoltazhaBuff;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Teski
{
    public class TeskiHoldout : BaseHoldoutSword, ILocalizedModType
    {
        #region Sounds
        public static readonly SoundStyle SwingSound = new("OSTARsSWORDS/Sounds/Item/HellkiteSwing", 2);
        public static readonly SoundStyle SwingSoundBig = new("OSTARsSWORDS/Sounds/ClickyHit");
        public static readonly SoundStyle HitSoundSmall = new("OSTARsSWORDS/Sounds/Item/HellkiteSmallHit", 3);
        public static readonly SoundStyle HitSoundBig = new("OSTARsSWORDS/Sounds/Item/HellkiteBigHit", 2);
        public static readonly SoundStyle ChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteCharge");
        public static readonly SoundStyle FullChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteFullCharge");
        #endregion

        public override int AssignedItemID => Mod.Find<ModItem>("Teski").Type;
        public override string Texture => "OSTARsSWORDS/Content/Items/Swords/Teski/Teski";

        private bool boomerangThrown = false;

        #region UseStyle
        public override void UseStyle()
        {
            AnimationProgress = Animation % (chargedSwing ? (int)(storedUseAnim * 0.7f) : storedUseAnim);

            // Continuously detect right-click (same as PajcheWarAxeHoldout)
            if (IsRightClickHeld)
                Projectile.ai[2] = 5;

            DrawUnconditionally = false;
            bool cantUse = Owner == null || !Owner.active || Owner.dead ||
                (Projectile.ai[2] == 0 && !Owner.channel) ||
                (Projectile.ai[2] == 5 && !IsRightClickHeld) ||
                Owner.CCed || Owner.noItems;

            if (CanHit_Field || postSwing)
                mousePos = Owner.Center - aimVel;
            else
                mousePos = Main.MouseWorld;

            if (CanHit_Field)
                fadeIn = MathHelper.Lerp(fadeIn, 1, (chargedSwing ? 1.5f : 1) * 0.23f * Owner.GetAttackSpeed<MeleeDamageClass>());
            else
                fadeIn = MathHelper.Lerp(fadeIn, 0, 0.3f);
            if (chargeTimer > 0)
                fadeIn = Utils.Remap(chargeTimer, 0, chargeTimerMax, 0, 1f);

            if (cantUse)
            {
                chargeTimer = 0;
                Projectile.ai[2] = 0;
            }

            if (Projectile.ai[2] == 5)
                BoomerangThrow();
            else
                LeftClickAttack();

            ArmRotationOffset = MathHelper.ToRadians(-140f);
            ArmRotationOffsetBack = MathHelper.ToRadians(-140f);
        }

        /// <summary>
        /// Alt use — throw the blade as a boomerang, then kill the holdout.
        /// </summary>
        private void BoomerangThrow()
        {
            if (!boomerangThrown)
            {
                boomerangThrown = true;
                Vector2 aim = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX) * 20f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Owner.Center, aim,
                    ModContent.ProjectileType<Content.Projectiles.TeskiBoomerang>(),
                    Projectile.damage, Projectile.knockBack,
                    Projectile.owner);
            }
            // Kill the holdout immediately so it doesn't revert to a normal swing if right-click is released
            Projectile.Kill();
        }

        /// <summary>
        /// Left-click attack — same pattern as PajcheWarAxeHoldout.
        /// Windup → swing arc with ExpInOut easing → recovery, with charge-swing support.
        /// </summary>
        private void LeftClickAttack()
        {
            if (!doSwing)
            {
                playSwingSound = true;
                mousePos = Main.MouseWorld;
                aimVel = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX) * 65;
                CanHit_Field = false;
                if (mousePos.X < Owner.Center.X) Owner.direction = -1;
                else Owner.direction = 1;
                FlipAsSword = Owner.direction == -1;

                Vector2 bladePos = new Vector2(60, 0);
                Vector2 particlePos = Owner.Center + bladePos.RotatedBy(FinalRotation + MathHelper.ToRadians(-45));

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

                    RotationOffset = MathHelper.Lerp(RotationOffset,
                        MathHelper.ToRadians(120f * Projectile.ai[1] * Owner.direction *
                        (1 + (Utils.GetLerpValue(useAnim * 0.35f, useAnim * 0.6f, Animation, true)) * 0.5f)), 0.2f);
                    FlipAsSword = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX).X > 0;
                }
                else
                {
                    float time = AnimationProgress - (useAnim / 3);
                    float timeMax = useAnim - (useAnim / 3);

                    if (time >= (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && playSwingSound)
                    {
                        if (!chargedSwing)
                            SoundEngine.PlaySound(SwingSound with { Volume = 0.8f, PitchVariance = 0.25f }, Projectile.Center);
                        else
                            SoundEngine.PlaySound(SwingSound with { Volume = 0.9f, Pitch = 0.2f }, Projectile.Center);
                        swingCount++;
                        playSwingSound = false;
                    }

                    if (time > (int)(timeMax * (chargedSwing ? 0.2f : 0.4f)) && time < (int)(timeMax * (chargedSwing ? 0.9f : 0.7f)))
                    {
                        CanHit_Field = true;

                        Vector2 particleVel = new Vector2(0, 10 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));
                        Vector2 particlePos = Owner.Center + new Vector2(Main.rand.Next(30, 170), 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45));

                        // Icy swing trail particles
                        Dust spark = Dust.NewDustPerfect(particlePos, DustID.FireworksRGB, -particleVel.RotatedByRandom(0.2f));
                        spark.scale = Main.rand.NextFloat(0.3f, 0.7f);
                        spark.noGravity = true;
                        spark.color = Main.rand.NextBool(3) ? Color.LightBlue : Color.CornflowerBlue;

                        Dust smoke = Dust.NewDustPerfect(particlePos, DustID.Smoke, -particleVel.RotatedByRandom(0.2f) * 2);
                        smoke.scale = Main.rand.NextFloat(0.5f, 1f);
                        smoke.noGravity = true;
                        smoke.color = Main.rand.NextBool(4) ? Color.LightBlue : Color.DeepSkyBlue;
                    }
                    else
                    {
                        CanHit_Field = false;
                    }

                    RotationOffset = MathHelper.Lerp(RotationOffset,
                        MathHelper.ToRadians(MathHelper.Lerp(
                            150f * Projectile.ai[1] * Owner.direction,
                            120f * -Projectile.ai[1] * Owner.direction,
                            ExpInOutEasing(time / timeMax, 1))),
                        0.2f);

                    if (time < (int)(timeMax * 0.9f))
                        postSwing = true;

                    if (CanHit_Field)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            float randRot = Main.rand.NextFloat(-30, -60);
                            Vector2 dustVel = new Vector2(0, 15 * -Projectile.ai[1] * Owner.direction).RotatedBy(FinalRotation + MathHelper.ToRadians(randRot));
                            Dust dust2 = Dust.NewDustPerfect(
                                Owner.Center + new Vector2(170, 0).RotatedBy(FinalRotation + MathHelper.ToRadians(-45)).RotatedByRandom(0.3f),
                                DustID.FireworksRGB, dustVel * Main.rand.NextFloat(0.1f, 0.5f));
                            dust2.scale = Main.rand.NextFloat(0.55f, 0.85f);
                            dust2.noGravity = true;
                            dust2.color = Main.rand.NextBool(3) ? Color.LightBlue : Color.CornflowerBlue;
                        }
                    }
                }
            }
        }
        #endregion

        #region Hit logic
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int lightningType = ModContent.ProjectileType<Content.Projectiles.TeskiLightning>();

            // Helper: spawn N lightning bolts targeting around the enemy
            void SpawnLightningBarrage(Vector2 center, int count, float spread)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 strikePos = center + Main.rand.NextVector2Circular(spread, spread * 0.5f);
                    Projectile.NewProjectile(
                        Projectile.GetSource_OnHit(target),
                        strikePos, Vector2.Zero,
                        lightningType, 0, 0f,
                        Projectile.owner,
                        strikePos.X, strikePos.Y);
                }
            }

            // Teski crit effects — massive lightning storm (always strikes)
            if (hit.Crit)
            {
                SpawnLightningBarrage(target.Center, Main.rand.Next(4, 7), 80f);
                for (int i = 0; i < 10; i++)
                {
                    Dust spark = Dust.NewDustPerfect(target.Center, DustID.Electric,
                        Main.rand.NextVector2Circular(6f, 6f));
                    spark.scale = Main.rand.NextFloat(0.8f, 1.4f);
                    spark.noGravity = true;
                }
                target.AddBuff(BuffID.Slow, 1800);
                Owner.AddBuff(ModContent.BuffType<WoltazhaBuff>(), 1800);
            }

            if (chargedSwing)
            {
                // Charged swing hit: ~70% chance for heavy lightning
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(HitSoundBig with { Volume = 1f }, Projectile.Center);
                    ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 8f);
                }
                if (Main.rand.NextFloat() < 0.7f)
                    SpawnLightningBarrage(target.Center, Main.rand.Next(3, 6), 70f);
                for (int i = 0; i < 8; i++)
                {
                    Dust spark = Dust.NewDustPerfect(target.Center + new Vector2(Main.rand.NextFloat(-30, 30), 0),
                        DustID.Electric, new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-5f, -1f)));
                    spark.scale = Main.rand.NextFloat(0.6f, 1.2f);
                    spark.noGravity = true;
                }
            }
            else
            {
                // Normal slash hit: ~30% chance for a lightning strike
                if (Projectile.numHits == 0)
                {
                    SoundEngine.PlaySound(HitSoundSmall with { Volume = 0.85f, PitchVariance = 0.25f }, Projectile.Center);
                    ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 4f);
                }
                if (Main.rand.NextFloat() < 0.3f)
                    SpawnLightningBarrage(target.Center, 1, 40f);
                for (int i = 0; i < 4; i++)
                {
                    Dust spark = Dust.NewDustPerfect(target.Center, DustID.Electric,
                        Main.rand.NextVector2Circular(4f, 4f));
                    spark.scale = Main.rand.NextFloat(0.4f, 0.9f);
                    spark.noGravity = true;
                }
            }

            // Teski extra hit — 2% of target current life
            damageDone += (int)(target.life * 0.02f);
            NPC.HitInfo extraHit = new()
            {
                Damage = damageDone + 220,
                HitDirection = Owner.direction,
                Crit = false
            };
            target.StrikeNPC(extraHit);
            Owner.addDPS(damageDone);
        }

        #endregion

        #region PreDraw — PajcheWarAxe-style aura + swoosh + main sword
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] == 5)
                return false;

            if ((useAnim > 0 || DrawUnconditionally) && Owner.ItemAnimationActive)
            {
                Asset<Texture2D> tex = ModContent.Request<Texture2D>(Texture);
                Asset<Texture2D> swoosh = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextGlow");

                float r = FlipAsSword ? MathHelper.ToRadians(90) : 0f;
                Vector2 generalDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);
                SpriteEffects sEffects = spriteEffects != SpriteEffects.None ? spriteEffects : (FlipAsSword ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

                // Glow aura (icy blue instead of PajcheWarAxe's orange)
                for (int i = 0; i < 25; i++)
                {
                    Color auraColor = Color.CornflowerBlue with { A = 0 } * 0.15f * fadeIn;
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 25f).ToRotationVector2() * (chargeTimer > 0 ? 4 : 7) * fadeIn;
                    Main.EntitySpriteDraw(tex.Value, generalDrawPos + drawOffset,
                        tex.Frame(1, FrameCount, 0, Frame), auraColor,
                        Projectile.rotation + RotationOffset + r,
                        FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin,
                        Projectile.scale, sEffects);
                }

                // Swoosh trail on swings
                if (swingCount > 0 && !playSwingSound)
                {
                    Color swooshColor = Color.Lerp(Color.MidnightBlue, Color.CornflowerBlue, 0.4f) with { A = 0 } * fadeIn * 0.9f;
                    Main.EntitySpriteDraw(swoosh.Value, generalDrawPos, null,
                        swooshColor,
                        (FinalRotation + MathHelper.ToRadians(45)) + MathHelper.ToRadians(swingCount % 2 == 0 ? -80 : 80) * -Owner.direction,
                        swoosh.Size() * 0.5f, Projectile.scale * 2.35f / 4,
                        swingCount % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
                }

                // Main sword
                Main.EntitySpriteDraw(tex.Value, generalDrawPos, tex.Frame(1, FrameCount, 0, Frame),
                    lightColor, Projectile.rotation + RotationOffset + r,
                    FlipAsSword ? new Vector2(tex.Width() - SpriteOrigin.X, SpriteOrigin.Y) : SpriteOrigin,
                    Projectile.scale, sEffects);
            }
            return false;
        }
        #endregion
    }
}
