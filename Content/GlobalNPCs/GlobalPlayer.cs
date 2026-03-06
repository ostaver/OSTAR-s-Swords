using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.GlobalNPCs
{
    public class GlobalPlayer : ModPlayer
    {
        public bool eBlaze;
        public bool slowReaper;
        public bool DivineCrit;
        public bool nightmareAbilityActive;
        public int nightmareAbilityCooldownTimer;
        public int nightmareAbilityDurationTimer;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (slowReaper && modifiers.DamageType == DamageClass.Melee)
            {
                modifiers.FlatBonusDamage += (int)(target.lifeMax / 1000);
            }

            if (DivineCrit)
            {
                modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) => {
                    if (hitInfo.Crit)
                    {
                        hitInfo.Damage += (int)(hitInfo.Damage * Main.rand.NextFloat(0.5f, 2.0f));
                    }
                };
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType == DamageClass.Melee && slowReaper) // Reaper's Toll effect
            {
                target.AddBuff(BuffID.Slow, 600);
                
                if (target.lifeMax > 10) 
                    target.lifeMax -= (int)target.lifeMax / 1000;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (nightmareAbilityDurationTimer > 0)
            {
                nightmareAbilityDurationTimer--;
                if (nightmareAbilityDurationTimer == 0)
                {
                    nightmareAbilityActive = false;
                }
            }

            if (nightmareAbilityCooldownTimer > 0)
            {
                nightmareAbilityCooldownTimer--;
                if (nightmareAbilityCooldownTimer == 0)
                {
                    Terraria.Audio.SoundEngine.PlaySound(new Terraria.Audio.SoundStyle("OSTARsSWORDS/Content/Items/Swords/NightmareReady") {Volume = 0.5f, Pitch = 0.5f}, Player.position);
                }
            }
        }

        // Triggers will now be handled inside the Item file instead of a keybind.

        public override void ResetEffects()
        {
            eBlaze = false;
            slowReaper = false;
            DivineCrit = false;
        }

        public override void UpdateBadLifeRegen()
        {
            if (eBlaze)
            {
                // In tModLoader 1.4+, lifeRegen is handled slightly differently.
                // If lifeRegen is positive, reset it to 0 before applying the burn.
                if (Player.lifeRegen > 0)
                {
                    Player.lifeRegen = 0;
                }

                Player.lifeRegenTime = 0;
                // lifeRegen -= 40000 results in 20,000 damage per second.
                Player.lifeRegen -= 40000;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (eBlaze)
            {
                // Only spawn dust if the player isn't a "shadow" (afterimage)
                if (Main.rand.NextBool(8) && drawInfo.shadow == 0f)
                {
                    int dustType = Mod.Find<ModDust>("EmperorBlazeDust").Type;
                    int dust = Dust.NewDust(drawInfo.Position - new Vector2(2f, 2f), Player.width + 4, Player.height + 4, dustType, Player.velocity.X * 0.4f, Player.velocity.Y * 0.4f, 100, default, 3f);

                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.dust[dust].noGravity = false;
                }

                // Shifting player color tint towards orange/red
                r *= 1f;
                g *= 0.5f;
                b *= 0f;
                fullBright = true;
            }
        }
    }
}