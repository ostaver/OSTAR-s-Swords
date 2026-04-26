using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords
{
    public abstract class BaseHoldoutSword : ModProjectile
    {
        #region Base class fields
        public bool whenSpawned = true;
        public abstract int AssignedItemID { get; }
        
        public virtual float HitboxOutset => 100f * (Projectile.scale / 1.5f);
        public virtual Vector2 HitboxSize => new Vector2(185, 185) * Projectile.scale;
        public virtual float HitboxRotationOffset => MathHelper.ToRadians(-45);

        public Vector2 Offset = Vector2.Zero;
        public Player Owner => Main.player[Projectile.owner];
        public int NumberOfAnimations = 0;
        public float Animation = 0;
        public float AnimationProgress = 0;
        public bool FlipAsSword = false;
        public bool IgnoreActiveAnimation = false;
        public float RotationOffset = 0f;
        public float ArmRotationOffset = 0f;
        public float ArmRotationOffsetBack = 0f;
        public virtual int FrameCount => 1;
        public int Frame = 0;
        public virtual Vector2 SpriteOrigin => new(-3, 90);
        public float FinalRotation => Projectile.rotation + RotationOffset;
        public SpriteEffects spriteEffects = SpriteEffects.None;
        public bool CanHit_Field = false;
        public Vector2 AbsolutePosition = Vector2.Zero;
        public bool DrawUnconditionally = false;
        #endregion

        #region Animation fields
        public Vector2 mousePos;
        public Vector2 aimVel;
        public int useAnim;
        public int storedUseAnim;
        public int swingCount = 0;
        public bool playSwingSound = true;
        public int pierceReduction = 0;
        public float fadeIn = 0f;

        public bool doSwing = false;
        public bool postSwing = false;
        public bool chargedSwing = false;
        public int chargeTimer = 0;
        public int chargeTimerMax = 240;
        public float GFBMulti = 1;
        public int GFBCharge = 0;
        public bool GFBFlashWarning = false;

        public SlotId AudSlot;

        /// <summary>
        /// Checks if right-click is currently held. Main.mouseRight stays true while held,
        /// unlike altFunctionUse which resets after the first frame.
        /// </summary>
        public bool IsRightClickHeld => Projectile.owner == Main.myPlayer && Main.mouseRight;
        #endregion

        #region Utility
        public static float ExpInOutEasing(float amount, int degree) =>
            amount == 0f ? 0f : amount == 1f ? 1f : amount < 0.5f
                ? (float)Math.Pow(2, 20f * amount - 10f) / 2f
                : (2f - (float)Math.Pow(2, -20f * amount + 10f)) / 2f;
        #endregion

        public override void SetDefaults()
        {
            Projectile.width = (int)Math.Max(HitboxSize.X, 1);
            Projectile.height = (int)Math.Max(HitboxSize.Y, 1);
            Projectile.friendly = true;
            Projectile.scale = 1.5f;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.noEnchantmentVisuals = true;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = Owner.HeldItem.useAnimation + 1;
        }

        public virtual void WhenSpawned()
        {
            CanHit_Field = false;
            Projectile.knockBack = 0;
            bool oldNoMelee = Owner.HeldItem.noMelee;
            Owner.HeldItem.noMelee = false;
            Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
            Owner.HeldItem.noMelee = oldNoMelee;
            Projectile.ai[1] = -1;

            mousePos = Main.MouseWorld;
            aimVel = (Owner.Center - Main.MouseWorld).SafeNormalize(Vector2.UnitX) * 65;
            useAnim = Owner.itemAnimationMax;
            storedUseAnim = useAnim;

            chargeTimerMax = useAnim * 5;

            if (mousePos.X < Owner.Center.X) Owner.direction = -1;
            else Owner.direction = 1;

            FlipAsSword = Owner.direction == -1;
        }

        public abstract void UseStyle();

        #region AI
        public override void AI()
        {
            if (whenSpawned)
            {
                WhenSpawned();
                whenSpawned = false;
                Projectile.timeLeft = Owner.HeldItem.useAnimation + 1;
                Projectile.netUpdate = true;
            }

            bool ItemAnimationActive = Owner.ItemAnimationActive;

            if (Owner.HeldItem.type != AssignedItemID || Owner.dead)
                Projectile.Kill();

            bool oldNoMelee = Owner.HeldItem.noMelee;
            Owner.HeldItem.noMelee = false;
            Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem);
            Owner.HeldItem.noMelee = oldNoMelee;

            if (ItemAnimationActive || IgnoreActiveAnimation)
            {
                Animation++;
                UseStyle();
                Owner.heldProj = Projectile.whoAmI;
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffset);
                Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffsetBack);
            }
            else
            {
                Animation = 0;
                if (DrawUnconditionally)
                {
                    Owner.heldProj = Projectile.whoAmI;
                    Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffset);
                    Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + RotationOffset + ArmRotationOffsetBack);
                }
                NumberOfAnimations = 0;
            }

            AnimationProgress = Animation % Owner.itemAnimationMax;

            if (AbsolutePosition == Vector2.Zero)
                Projectile.position = Owner.position + (Owner.Size / 2) - (Projectile.Size / 2) + Offset;
            else
            {
                AbsolutePosition += Projectile.velocity;
                Projectile.position = AbsolutePosition - (Projectile.Size / 2) + Offset;
            }

            if (AnimationProgress == Owner.itemAnimationMax - 1)
                NumberOfAnimations++;

            if (Owner.itemAnimation == Owner.itemAnimationMax - 1)
                Projectile.timeLeft = Owner.HeldItem.useAnimation + 1;

            if (DrawUnconditionally) Projectile.timeLeft = Math.Max(Projectile.timeLeft, 2);
        }
        #endregion

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(AudSlot, out var snd))
                snd?.Stop();
        }

        #region CanHitNPC / CanDamage / ModifyDamageHitbox
        public override bool? CanHitNPC(NPC target)
        {
            return !target.friendly && !target.dontTakeDamage;
        }

        public override bool? CanDamage()
        {
            return CanHit_Field ? base.CanDamage() : false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 cen = Projectile.Center + new Vector2(HitboxOutset, 0).RotatedBy(FinalRotation + HitboxRotationOffset);
            hitbox = new Rectangle((int)cen.X - (int)(HitboxSize.X / 2), (int)cen.Y - (int)(HitboxSize.Y / 2), (int)HitboxSize.X, (int)HitboxSize.Y);
            base.ModifyDamageHitbox(ref hitbox);
        }
        #endregion

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Owner.direction;

            if (chargedSwing)
            {
                modifiers.SetCrit();
                modifiers.FlatBonusDamage += target.lifeMax * 0.08f;
            }
            else
            {
                modifiers.FlatBonusDamage += target.lifeMax * 0.04f;
            }
        }
    }
}
