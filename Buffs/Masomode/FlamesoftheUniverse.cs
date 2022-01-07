using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.NPCs;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class FlamesoftheUniverse : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Flames of the Universe");
            //Description.SetDefault("The heavens themselves have judged you");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "宇宙之火");
            //Description.AddTranslation(GameCulture.Chinese, "来自诸天的亲自审判");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //activates various vanilla debuffs
            player.GetModPlayer<FargoPlayer>().FlamesoftheUniverse = true;
            player.ichor = true;

            /*player.GetModPlayer<FargoPlayer>().Shadowflame = true;
            player.onFire = true;
            player.onFire2 = true;
            player.onFrostBurn = true;
            player.burned = true;
            player.ichor = true;*/
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<FargoSoulsGlobalNPC>().FlamesoftheUniverse = true;
            npc.ichor = true;

            /*bool beImmune = npc.buffTime[buffIndex] > 2;
            npc.buffImmune[BuffID.OnFire] = beImmune;
            npc.buffImmune[BuffID.CursedInferno] = beImmune;
            npc.buffImmune[BuffID.ShadowFlame] = beImmune;
            npc.buffImmune[BuffID.Frostburn] = beImmune;
            npc.buffImmune[BuffID.Ichor] = beImmune;
            npc.onFire = true;
            npc.onFire2 = true;
            npc.shadowFlame = true;
            npc.onFrostBurn = true;
            npc.ichor = true;*/
        }
    }
}