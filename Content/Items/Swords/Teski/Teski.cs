using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OSTARsSWORDS.Content.Buffs.WoltazhaBuff;
using OSTARsSWORDS.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords.Teski
{
    public class Teski : ModItem, ILocalizedModType
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
            Item.damage = 1;
            Item.DamageType = DamageClass.Melee;
            Item.scale = 1.5f;
            Item.width = 128;
            Item.height = 128;
            Item.useTime = 90;
            Item.useAnimation = 90;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(gold: 999);
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.autoReuse = true;
            Item.ResearchUnlockCount = 1;
            //Item.crit = 99;

            Item.channel = true;
            Item.shoot = Mod.Find<ModProjectile>("TeskiHoldout").Type;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }

        // Inlined from CustomUseProjItem base class
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1 && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.TeskiBoomerang>()] < 1;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.TeskiBoomerang>()] < 1 && base.CanUseItem(player);
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

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod, "ElBastardo", 1);
            recipe.AddIngredient(ItemID.Burger, 15);
            recipe.AddIngredient(ItemID.HallowedBar, 25);
            recipe.AddIngredient(Mod, "Grease", 99);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(ItemID.SoulofFright, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
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
