using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Pajche
{
    public class PajcheWarAxe : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle SwingSound = new("OSTARsSWORDS/Sounds/Item/HellkiteSwing", 2);
        public static readonly SoundStyle SwingSoundBig = new("OSTARsSWORDS/Sounds/Item/HellkiteHeavySwing");
        public static readonly SoundStyle HitSoundSmall = new("OSTARsSWORDS/Sounds/Item/HellkiteSmallHit", 3);
        public static readonly SoundStyle HitSoundBig = new("OSTARsSWORDS/Sounds/Item/HellkiteBigHit", 2);
        public static readonly SoundStyle ChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteCharge");
        public static readonly SoundStyle FullChargeSound = new("OSTARsSWORDS/Sounds/Item/HellkiteFullCharge");
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 90;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.scale = 1.5f;
            Item.useAnimation = Item.useTime = 71;
            Item.useTurn = true;
            Item.knockBack = 13f;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.Lime;

            Item.channel = true;
            Item.shoot = Mod.Find<ModProjectile>("PajcheWarAxeHoldout").Type;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        // Inlined from CustomUseProjItem base class
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, 5);
            }
            else
                Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, type, damage, knockback, player.whoAmI, 0, 0, 0);
            return false;
        }
        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glowmaskTexture = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextSparkle").Value;
            Vector2 origin = new Vector2(glowmaskTexture.Width / 2f, glowmaskTexture.Height / 2f);
            Color color = Color.White;
            spriteBatch.Draw(glowmaskTexture, Item.Center - Main.screenPosition, null, color, rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
