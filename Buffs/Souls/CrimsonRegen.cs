using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Souls
{
    public class CrimsonRegen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crimson Regen");
            Description.SetDefault("You are regenning your last wound");
            Main.buffNoSave[Type] = true;
            //Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //player.buffTime[buffIndex] = 2;
            //player.GetModPlayer<FargoSoulsPlayer>().CrimsonRegen = true;
            player.lifeRegenTime++;
            player.lifeRegen += player.GetModPlayer<FargoSoulsPlayer>().CrimsonRegenAmount;

            for (int i = 0; i < 10; i++)
            {
                int num6 = Dust.NewDust(player.position, player.width, player.height, 5, 0f, 0f, 175, default(Color), 1.75f);
                Main.dust[num6].noGravity = true;
                Main.dust[num6].velocity *= 0.75f;
                int num7 = Main.rand.Next(-40, 41);
                int num8 = Main.rand.Next(-40, 41);
                Dust dust = Main.dust[num6];
                dust.position.X = dust.position.X + (float)num7;
                Dust dust2 = Main.dust[num6];
                dust2.position.Y = dust2.position.Y + (float)num8;
                Main.dust[num6].velocity.X = (float)(-(float)num7) * 0.075f;
                Main.dust[num6].velocity.Y = (float)(-(float)num8) * 0.075f;
            }

        }
    }
}