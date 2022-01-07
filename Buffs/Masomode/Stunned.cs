using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class Stunned : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Stunned");
            //Description.SetDefault("You're too dizzy to move");
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
            Main.debuff[Type] = true;
            //DisplayName.AddTranslation(GameCulture.Chinese, "昏迷");
            //Description.AddTranslation(GameCulture.Chinese, "你头晕目眩,动弹不得");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlLeft = false;
            player.controlRight = false;
            player.controlJump = false;
            player.controlDown = false;
            player.controlUseItem = false;
            player.controlHook = false;
            player.releaseHook = true;
            if (player.mount.Active)
                player.mount.Dismount(player);
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
            return time > 2;
        }
    }
}