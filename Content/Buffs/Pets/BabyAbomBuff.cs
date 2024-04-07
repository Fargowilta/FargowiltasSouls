using FargowiltasSouls.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Pets
{
    public class BabyAbomBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Baby Abom");
            // Description.SetDefault("Kickflipping on a scythe");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.FargoSouls().BabyAbom = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BabyAbom>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<BabyAbom>(), 0, 0f, player.whoAmI);
            }
        }
    }
}