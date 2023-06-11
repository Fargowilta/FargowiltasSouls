using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Boss
{
    public class BaronsBurden : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderDebuff";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baron's Burden");
            Description.SetDefault("Gravity heavily increased outside water\nBreathe water instead of air\nGet back in!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Terraria.ID.BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gills = true;
            if (!player.wet)
            {
                player.velocity.Y += 0.4f;
                player.velocity.X *= 0.9f;
            }
        }
    }
}