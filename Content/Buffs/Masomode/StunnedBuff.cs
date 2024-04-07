using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Masomode
{
    public class StunnedBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Stunned");
            // Description.SetDefault("You're too dizzy to move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "昏迷");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "你头晕目眩,动弹不得");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlHook = false;
            player.releaseHook = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
            player.FargoSouls().Stunned = true;
            player.FargoSouls().NoUsingItems = 2;

            if (player.whoAmI == Main.myPlayer && player.buffTime[buffIndex] % 60 == 55)
                SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/DizzyBird"));
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss)
            {
                npc.velocity.X *= 0;
                npc.velocity.Y *= 0;
                npc.frameCounter = 0;
            }
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            return time > 3;
        }
    }
}