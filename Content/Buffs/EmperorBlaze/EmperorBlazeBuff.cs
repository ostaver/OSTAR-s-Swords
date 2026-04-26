using OSTARsSWORDS.Content.Globals;
using Terraria;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Buffs.EmperorBlaze;

public class EmperorBlazeBuff : ModBuff
{
    public override void SetStaticDefaults()
	{
		Main.debuff[Type] = true;
		Main.pvpBuff[Type] = true;
		Main.buffNoSave[Type] = true;
	}

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<GlobalPlayer>().eBlaze = true;
    }

    public override void Update(NPC npc, ref int buffIndex)
	{
        npc.GetGlobalNPC<OstarsGlobalNPC>().eBlaze = true;

        Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
    }
}
