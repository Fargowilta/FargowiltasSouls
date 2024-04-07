using FargowiltasSouls.Content.Projectiles.Pets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Buffs.Pets
{
    public class SeekerOfTreasuresBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Seeker of Treasures");
            // Description.SetDefault("Give it a few thousand years to grow up");
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.FargoSouls().SeekerOfAncientTreasures = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SeekerOfTreasures>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<SeekerOfTreasures>(), 0, 0f, player.whoAmI);
            }
        }
    }
}