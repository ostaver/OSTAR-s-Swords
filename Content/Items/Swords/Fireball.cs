using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Projectiles;

namespace OSTARsSWORDS.Content.Items.Swords;

public class Fireball : ModItem
{
	private static readonly SoundStyle FR_SWING = new SoundStyle("OSTARsSWORDS/Sounds/FR_SWING")
	{
		Volume = 0.5f,
		Pitch = 0.3f
	};

	public override void SetDefaults()
	{
		Item.width = 54;
		Item.height = 54;
		Item.scale = 1.2f;
		Item.rare = ItemRarityID.LightRed;

		// Throw style — no melee swing
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 28;
		Item.useAnimation = 28;
		Item.noMelee = true;       // The item itself does not deal melee damage
		Item.noUseGraphic = true;  // Don't draw the item sprite when used

		Item.damage = 31;
		Item.knockBack = 5f;
		Item.DamageType = DamageClass.Melee;

		Item.shoot = ModContent.ProjectileType<FireballProjectile>();
		Item.shootSpeed = 28f;     // Fast in a straight line
		Item.UseSound = FR_SWING;

		Item.value = Item.sellPrice(0, 3, 0, 0);
		Item.autoReuse = true;
		Item.ResearchUnlockCount = 1;
	}
}
