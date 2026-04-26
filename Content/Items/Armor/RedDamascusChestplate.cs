using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class RedDamascusChestplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 20;
            Item.ResearchUnlockCount = 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<RedDamascusHelmet>() &&
                   legs.type == ModContent.ItemType<RedDamascusLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Grants Titan potion effect, 1.2x melee damage, 10% increased melee critical chance, increases maximum life by 20";
            
            player.AddBuff(BuffID.Titan, 2);
            player.GetDamage<MeleeDamageClass>() *= 1.2f;
            player.GetCritChance(DamageClass.Generic) += 10f;
            player.statLifeMax2 += 20;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.1f;
            player.GetCritChance(DamageClass.Generic) += 10f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(Mod, "DamascusBar", 15);
            recipe.AddIngredient(Mod, "DamascusBreastplate", 1);
            recipe.AddIngredient(ItemID.SoulofMight, 15);
            recipe.AddIngredient(ItemID.SoulofSight, 15);
            recipe.AddIngredient(ItemID.SoulofFright, 15);
            recipe.AddIngredient(ItemID.WrathPotion, 15);
            recipe.AddIngredient(ItemID.HallowedPlateMail, 1);
            recipe.AddIngredient(ItemID.HallowedBar, 16);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
