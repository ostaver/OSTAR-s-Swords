using Terraria;
using Terraria.ModLoader;
using OSTARsSWORDS.Rarities;
using OSTARsSWORDS.Content.Globals;

namespace OSTARsSWORDS.Content.Accessories;

public class SwordOfTheDivine : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 100;
		Item.height = 100;
		Item.value = Item.sellPrice(gold: 33);
		Item.rare = ModContent.RarityType<CalamityRed>();
		Item.accessory = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		//Increases crit damage based on your crit chance
		player.GetModPlayer<GlobalPlayer>().DivineCrit = true;
    }
}
