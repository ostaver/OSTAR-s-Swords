using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OSTARsSWORDS.Content.Items.Potions.SkoomaPotion;

namespace OSTARsSWORDS.Content.Globals
{
    public class OstarsGlobalNPC : GlobalNPC
    {
        public bool eBlaze;

        public override bool InstancePerEntity => true;

        public override void ResetEffects(NPC npc)
        {
            eBlaze = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (eBlaze)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= 12000;

                if (damage < 40000)
                {
                    damage = 40000;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (!eBlaze)
                return;

            if (Main.rand.Next(8) < 6)
            {
                // Dust logic
                int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, Mod.Find<ModDust>("EmperorBlazeDust").Type, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);

                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity.Y -= 0.5f;

                if (Main.rand.Next(8) == 0)
                {
                    Main.dust[dust].noGravity = false;
                    Main.dust[dust].scale *= 0.7f;
                }
            }
            // Adds a blue-ish light around the NPC
            Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
        }
    }
}