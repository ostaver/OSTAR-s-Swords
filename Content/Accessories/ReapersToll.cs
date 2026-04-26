using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Globals;

namespace OSTARsSWORDS.Content.Accessories;

public class ReapersToll : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 100;
		Item.height = 100;
		Item.value = Item.sellPrice(gold: 27);
		Item.rare = ItemRarityID.LightRed;
		Item.accessory = true;
		Item.expert = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// Increases total attack speed by 15%
		player.GetAttackSpeed(DamageClass.Melee) *= 1.15f;
		player.GetModPlayer<GlobalPlayer>().slowReaper = true;
    }

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(Mod, "UpgradeMatter", 4);
        recipe.AddIngredient(Mod, "SwordShard", 1);
        recipe.AddIngredient(ItemID.SoulofNight, 10);
        recipe.AddIngredient(ItemID.SoulofLight, 10);
        recipe.AddIngredient(ItemID.Ectoplasm, 18);
        recipe.AddIngredient(ItemID.Seedler, 1);
        recipe.AddIngredient(ItemID.SporeSac, 1);
        recipe.AddTile(TileID.MythrilAnvil);
		recipe.Register();
	}
}
