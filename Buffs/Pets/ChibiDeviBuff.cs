using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Pets
{
    public class ChibiDeviBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Chibi Devi");
            Description.SetDefault("She's interested in 'you'");
            DisplayName.AddTranslation(GameCulture.Chinese, "小戴维安帽");
            Description.AddTranslation(GameCulture.Chinese, "她对'你'很感兴趣");
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<FargoPlayer>().ChibiDevi = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.ChibiDevi>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Pets.ChibiDevi>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
