using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.NPCs;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Sadism : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Eternity");
            Description.SetDefault("The power of Eternity Mode is with you");
            Main.buffNoSave[Type] = false;
            DisplayName.AddTranslation(GameCulture.Chinese, "永恒");
            Description.AddTranslation(GameCulture.Chinese, "永恒模式的力量与你同在");
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "FargowiltasSouls/Buffs/PlaceholderBuff";
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffImmune[ModContent.BuffType<Antisocial>()] = true;
            player.buffImmune[ModContent.BuffType<Atrophied>()] = true;
            player.buffImmune[ModContent.BuffType<Berserked>()] = true;
            player.buffImmune[ModContent.BuffType<Bloodthirsty>()] = true;
            player.buffImmune[ModContent.BuffType<ClippedWings>()] = true;
            player.buffImmune[ModContent.BuffType<Crippled>()] = true;
            player.buffImmune[ModContent.BuffType<CurseoftheMoon>()] = true;
            player.buffImmune[ModContent.BuffType<Defenseless>()] = true;
            player.buffImmune[ModContent.BuffType<FlamesoftheUniverse>()] = true;
            player.buffImmune[ModContent.BuffType<Flipped>()] = true;
            player.buffImmune[ModContent.BuffType<FlippedHallow>()] = true;
            player.buffImmune[ModContent.BuffType<Fused>()] = true;
            player.buffImmune[ModContent.BuffType<GodEater>()] = true;
            player.buffImmune[ModContent.BuffType<Guilty>()] = true;
            player.buffImmune[ModContent.BuffType<Hexed>()] = true;
            player.buffImmune[ModContent.BuffType<Hypothermia>()] = true;
            player.buffImmune[ModContent.BuffType<Infested>()] = true;
            player.buffImmune[ModContent.BuffType<IvyVenom>()] = true;
            player.buffImmune[ModContent.BuffType<Jammed>()] = true;
            player.buffImmune[ModContent.BuffType<Lethargic>()] = true;
            player.buffImmune[ModContent.BuffType<LihzahrdCurse>()] = true;
            player.buffImmune[ModContent.BuffType<LightningRod>()] = true;
            player.buffImmune[ModContent.BuffType<LivingWasteland>()] = true;
            player.buffImmune[ModContent.BuffType<Lovestruck>()] = true;
            player.buffImmune[ModContent.BuffType<LowGround>()] = true;
            player.buffImmune[ModContent.BuffType<MarkedforDeath>()] = true;
            player.buffImmune[ModContent.BuffType<Midas>()] = true;
            player.buffImmune[ModContent.BuffType<MutantNibble>()] = true;
            player.buffImmune[ModContent.BuffType<NanoInjection>()] = true;
            player.buffImmune[ModContent.BuffType<NullificationCurse>()] = true;
            player.buffImmune[ModContent.BuffType<Oiled>()] = true;
            player.buffImmune[ModContent.BuffType<OceanicMaul>()] = true;
            player.buffImmune[ModContent.BuffType<Purified>()] = true;
            player.buffImmune[ModContent.BuffType<ReverseManaFlow>()] = true;
            player.buffImmune[ModContent.BuffType<Rotting>()] = true;
            player.buffImmune[ModContent.BuffType<Shadowflame>()] = true;
            player.buffImmune[ModContent.BuffType<SqueakyToy>()] = true;
            player.buffImmune[ModContent.BuffType<Swarming>()] = true;
            player.buffImmune[ModContent.BuffType<Stunned>()] = true;
            player.buffImmune[ModContent.BuffType<Unstable>()] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            FargoSoulsGlobalNPC fargoNPC = npc.GetGlobalNPC<FargoSoulsGlobalNPC>();
            npc.poisoned = true;
            npc.venom = true;
            npc.ichor = true;
            npc.onFire2 = true;
            npc.betsysCurse = true;
            npc.midas = true;
            fargoNPC.Electrified = true;
            fargoNPC.OceanicMaul = true;
            fargoNPC.CurseoftheMoon = true;
            fargoNPC.Infested = true;
            fargoNPC.Rotting = true;
            fargoNPC.MutantNibble = true;
            fargoNPC.Sadism = true;
        }
    }
}
