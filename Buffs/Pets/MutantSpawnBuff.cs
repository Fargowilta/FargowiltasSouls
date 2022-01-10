using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Pets
{
    public class MutantSpawnBuff : ModBuff
    {
        public override void SetDefaults()
        {
            //DisplayName.SetDefault("Mutant Spawn");
            //Description.SetDefault("Mutant Spawn");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<FargoPlayer>().MutantSpawn = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.MutantSpawn>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Pets.MutantSpawn>(), 0, 0f, player.whoAmI);
            }
        }
    }
}
