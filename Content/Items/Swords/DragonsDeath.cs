using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Items.Swords;

public class DragonsDeath : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 128;
		Item.height = 128;
		Item.scale = 1f;
		Item.rare = ItemRarityID.Lime;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.damage = 190;
		Item.knockBack = 13f;
		Item.UseSound = SoundID.Item1;
		Item.value = 490500;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.ResearchUnlockCount = 1;
	}
}
