using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Pets
{
    public class BabyLifelightBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Buffs/PlaceholderBuff";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petlight");
            Description.SetDefault("Behold, the light of an angel");
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
            Main.lightPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.GetModPlayer<FargoSoulsPlayer>().BabyLifelight = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Pets.BabyLifelight>()] <= 0 && player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.Pets.BabyLifelight>(), 0, 0f, player.whoAmI);
            }
        }
    }
}