using OSTARsSWORDS.Content.Accessories;
using OSTARsSWORDS.Content.Items;
using OSTARsSWORDS.Content.Items.Materials;
using OSTARsSWORDS.Content.Items.Potions.SkoomaPotion;
using OSTARsSWORDS.Content.Items.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.GlobalNPCs;

public class Loot : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        //Materials
        if (!npc.friendly) //Sword Matter
        {
            if (Main.expertMode) //Expert
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordMatter>(), chanceDenominator: 3, minimumDropped: 2, maximumDropped: 10));
            }
            else if (Main.masterMode)  //Master
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordMatter>(), chanceDenominator: 3, minimumDropped: 2, maximumDropped: 20));
            }
            else //Normal
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordMatter>(), chanceDenominator: 3, minimumDropped: 1, maximumDropped: 5));
            }
        }

        if (npc.type == NPCID.MartianSaucerCore) //Martian Saucer Core
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MartianSaucerCore>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        //Swords
        if (npc.type == NPCID.Paladin) //Paladin Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PaladinSword>(), chanceDenominator: 10, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GiantBat) //Bat Slayer
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BatSlayer>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.EyeofCthulhu) //Cthulhu Judge
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CthulhuJudge>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.KingSlime) //Sticky Glowstick Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StickyGlowstickSword>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.EaterofWorldsTail) //The Eater
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheEater>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.BrainofCthulhu) //The Brain
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheBrain>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.SkeletronHead) //Sword of Power
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordOfPower>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.SkeletronPrime) //Prime Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimeSword>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Spazmatism) //Twins Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TwinsSword>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.TheDestroyer) //Destroyer Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DestroyerSword>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Plantera) //Executioner
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Executioner>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Golem) //Golem
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Golem>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.DukeFishron) //Sharkron
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Sharkron>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.CultistBoss) //Doomsday
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Doomsday>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Vampire || npc.type == NPCID.VampireBat) //Dracula Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DraculaSword>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Frankenstein) //Finger of Doom
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FingerOfDoom>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Unicorn) //Giant Unicorn Horn
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GiantUnicornHorn>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if(npc.type == NPCID.GreekSkeleton) //Caesar's Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CaesarSword>(), chanceDenominator: 15, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.PirateShip) //Dutchman Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DutchmanSword>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.RedDevil) //Devil Blade, Scarlet Flare Core
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DevilBlade>(), chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScarletFlareCore>(), chanceDenominator: 15, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Demon || npc.type == NPCID.Corruptor) //Death Sword, Daedric Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeathSword>(), chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DaedricSword>(), chanceDenominator: 60, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GoblinWarrior) //Sting
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Sting>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GoblinPeon) //Goblin Knife
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoblinKnife>(), chanceDenominator: 9, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.FireImp) //Fireball
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Fireball>(), chanceDenominator: 15, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Piranha) //Biter
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Biter>(), chanceDenominator: 15, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.DungeonSlime) //Slime Killer
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SlimeKiller>(), chanceDenominator: 6, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.TheGroom) //Useless Weapon
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UselessWeapon>(), chanceDenominator: 2, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Werewolf) //Wolf Destroyer
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WolfDestroyer>(), chanceDenominator: 30, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Wraith) //Wraith Blade
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WraithBlade>(), chanceDenominator: 50, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Zombie || npc.type == NPCID.ArmedZombie || npc.type == NPCID.BaldZombie || npc.type == NPCID.PincushionZombie || npc.type == NPCID.SlimedZombie || npc.type == NPCID.SwampZombie || npc.type == NPCID.TwiggyZombie || npc.type == NPCID.FemaleZombie) //Zombie Knife
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZombieKnife>(), chanceDenominator: 50, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Mimic) //El Bastardo
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ElBastardo>(), chanceDenominator: 4, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GiantCursedSkull) //Weird Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WeirdSword>(), chanceDenominator: 30, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.DarkCaster) //Water Bolt Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WaterBoltSword>(), chanceDenominator: 40, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Harpy) //Feather Duster
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FeatherDuster>(), chanceDenominator: 30, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.WyvernHead) //Sky Power
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkyPower>(), chanceDenominator: 10, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.RustyArmoredBonesAxe || npc.type == NPCID.RustyArmoredBonesFlail || npc.type == NPCID.RustyArmoredBonesSword || npc.type == NPCID.RustyArmoredBonesSwordNoArmor) //Rusty Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RustySword>(), chanceDenominator: 150, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.BlueArmoredBones || npc.type == NPCID.BlueArmoredBonesMace || npc.type == NPCID.BlueArmoredBonesNoPants || npc.type == NPCID.BlueArmoredBonesSword) //Magnet Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MagnetSword>(), chanceDenominator: 150, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.HellArmoredBones || npc.type == NPCID.HellArmoredBonesMace || npc.type == NPCID.HellArmoredBonesSword || npc.type == NPCID.HellArmoredBonesSpikeShield) //Sword of Flames
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordOfFlames>(), chanceDenominator: 150, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.FlyingSnake) //Dragon's Death
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DragonsDeath>(), chanceDenominator: 500, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Crab) //Ocean Roar
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OceanRoar>(), chanceDenominator: 50, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall) //Poison Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PoisonSword>(), chanceDenominator: 70, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GoblinSummoner) //Goblin Summoner Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PhantomScimitar>(), chanceDenominator: 15, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.GraniteGolem) //Granite Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WitherBane>(), chanceDenominator: 30, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.DrManFly) //Heisenberg's Flask
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HeisenbergsFlask>(), chanceDenominator: 30, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.Stylist) //Extase
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Extase>(), chanceDenominator: 4, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.PossessedArmor) //Possessed Sword
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PossessedSword>(), chanceDenominator: 40, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.MoonLordCore) //Star Maelstorm
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StarMaelstorm>(), chanceDenominator: 100, minimumDropped: 1, maximumDropped: 1));
        }

        if (npc.type == NPCID.DungeonGuardian) //Halo of Horrors
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HaloOfHorrors>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
        }

        if(npc.type == NPCID.HallowBoss) //Sword of the Divine, Empress of Light
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SwordOfTheDivine>(), chanceDenominator: 1, minimumDropped: 1, maximumDropped: 1));
            npc.DeathSound = new SoundStyle("OSTARsSWORDS/Sounds/EmpressFinish") { Volume = 1.2f };
        }

        //Biome Mimic Drops
        if (npc.type == NPCID.BigMimicCorruption) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ClingerSword>(), chanceDenominator: 3, minimumDropped: 1, maximumDropped: 1));
        if (npc.type == NPCID.BigMimicCrimson) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DartSword>(), chanceDenominator: 3, minimumDropped: 1, maximumDropped: 1));
        if (npc.type == NPCID.BigMimicHallow) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalVineSword>(), chanceDenominator: 3, minimumDropped: 1, maximumDropped: 1));
        if (npc.type == NPCID.BigMimicJungle) npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RottenSword>(), chanceDenominator: 3, minimumDropped: 1, maximumDropped: 1));

        //Lunar Pillar Drops
        int[] pillars = { NPCID.LunarTowerNebula, NPCID.LunarTowerSolar, NPCID.LunarTowerStardust, NPCID.LunarTowerVortex };
        if(npc.type == pillars[0] || npc.type == pillars[1] || npc.type == pillars[2] || npc.type == pillars[3]) 
        { 
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<InnosWrath>(), chanceDenominator: 5, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BeliarClaw>(), chanceDenominator: 5, minimumDropped: 1, maximumDropped: 1));
        }
    }
    public override void SetupTravelShop(int[] shop, ref int nextSlot)
    {
        shop[nextSlot] = ModContent.ItemType<SkoomaPotion>();
        nextSlot++;
    }
}