using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Pets
{
    public class AccursedSarcophagusBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Accursed Sarcophagus");
            Description.SetDefault("It'll keep its hands off");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<FargoSoulsPlayer>().AccursedSarcophagus = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.AccursedSarcophagus>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Pets.AccursedSarcophagus>(), 0, 0f, player.whoAmI);
            }
        }
    }
}