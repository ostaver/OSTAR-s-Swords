using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Items.Swords;

namespace OSTARsSWORDS.Content;

public class RecipeGroups : ModSystem
{
	public static int SwordsOfTheUniverseGroup { get; private set; }

	public override void AddRecipeGroups()
	{
		RecipeGroup group = new RecipeGroup(
			() => "Any Sword Of The Universe",
			ModContent.ItemType<SwordOfTheUniverse>(),
			ModContent.ItemType<SwordOfTheUniverseV2>(),
			ModContent.ItemType<SwordOfTheUniverseV3>(),
			ModContent.ItemType<SwordOfTheUniverseV4>(),
			ModContent.ItemType<SwordOfTheUniverseV5>(),
			ModContent.ItemType<SwordOfTheUniverseV6>(),
			ModContent.ItemType<SwordOfTheUniverseV7>(),
			ModContent.ItemType<SwordOfTheUniverseV8>(),
			ModContent.ItemType<SwordOfTheUniverseV9>()
		);

		SwordsOfTheUniverseGroup = RecipeGroup.RegisterGroup("Any Sword Of The Universe", group);
	}
}
