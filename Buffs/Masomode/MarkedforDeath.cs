using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasSouls.Buffs.Masomode
{
    public class MarkedforDeath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marked for Death");
            Description.SetDefault("Just don't get hit");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;

            //DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "死亡标记");
            //Description.AddTranslation((int)GameCulture.CultureName.Chinese, "别被打到");
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<FargoSoulsPlayer>().DeathMarked = true;

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Masomode.DeathSkull>()] < 1)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex),
                    player.Center - 200f * Vector2.UnitY, Vector2.Zero,
                    ModContent.ProjectileType<Projectiles.Masomode.DeathSkull>(), 0, 0f, player.whoAmI);
        }
    }
}
