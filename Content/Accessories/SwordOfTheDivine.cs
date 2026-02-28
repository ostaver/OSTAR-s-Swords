using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.GlobalNPCs;

namespace OSTARsSWORDS.Content.Accessories;

public class SwordOfTheDivine : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 100;
		Item.height = 100;
		Item.value = Item.sellPrice(gold: 33);
		Item.rare = ItemRarityID.Purple;
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		//Increases crit damage based on your crit chance
		player.GetModPlayer<GlobalPlayer>().DivineCrit = true;
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
