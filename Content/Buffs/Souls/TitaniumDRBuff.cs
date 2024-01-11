using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;

namespace FargowiltasSouls.Content.Buffs.Souls
{
    public class TitaniumDRBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Titanium Shield");
            // Description.SetDefault("You have extra damage resistance in close range and resist most debuffs");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            player.GetEffectFields<TitaniumFields>().TitaniumDRBuff = true;

            //kill all shards before running out
            if (player.buffTime[buffIndex] == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];

                    if (proj.active && proj.type == ProjectileID.TitaniumStormShard && proj.owner == player.whoAmI)
                    {
                        proj.Kill();
                    }
                }
            }
        }
    }
}
