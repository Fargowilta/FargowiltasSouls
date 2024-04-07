using FargowiltasSouls.Content.Buffs;
using FargowiltasSouls.Content.Buffs.Masomode;
using FargowiltasSouls.Content.Buffs.Minions;
using FargowiltasSouls.Content.Projectiles.Deathrays;
using FargowiltasSouls.Content.Projectiles.Minions;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Masomode
{
    public class GroundStick : SoulsItem
    {
        public override bool Eternity => true;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Remote Control");
            /* Tooltip.SetDefault(@"Grants immunity to Lightning Rod
Electric and ray attacks supercharge you and do halved damage if not already supercharged
While supercharged, you have increased movement speed, attack speed, and inflict Electrified and Lightning Rod
Two friendly probes fight by your side and will supercharge with you
'A defeated foe's segment with an antenna glued on'"); */
            //             DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "遥控装置");
            //             Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, @"'被击败敌人的残片,上面粘着天线'
            // 免疫避雷针
            // 攻击小概率造成避雷针效果
            // 召唤2个友善的探测器为你而战");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(0, 4);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[ModContent.BuffType<LightningRodBuff>()] = true;
            player.AddEffect<GroundStickDR>(Item);
            player.AddEffect<ProbeMinionEffect>(Item);
        }
    }
    public class GroundStickDR : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<GroundStick>();
        private static readonly int[] ElectricAttacks = new int[]
{
            ProjectileID.DeathLaser,
            ProjectileID.EyeLaser,
            ProjectileID.PinkLaser,
            ProjectileID.EyeBeam,
            ProjectileID.MartianTurretBolt,
            ProjectileID.BrainScramblerBolt,
            ProjectileID.GigaZapperSpear,
            ProjectileID.RayGunnerLaser,
            ProjectileID.SaucerLaser,
            ProjectileID.NebulaLaser,
            ProjectileID.VortexVortexLightning,
            ProjectileID.DD2LightningBugZap
};
        public override float ProjectileDamageDR(Player player, Projectile projectile, ref Player.HurtModifiers modifiers)
        {
            float dr = 0;

            bool electricAttack = false;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            Projectile proj = projectile;

            if (proj.ModProjectile == null)
            {
                if (proj.aiStyle == ProjAIStyleID.MartianDeathRay
                    || proj.aiStyle == ProjAIStyleID.ThickLaser
                    || proj.aiStyle == ProjAIStyleID.LightningOrb
                    || ElectricAttacks.Contains(proj.type))
                {
                    electricAttack = true;
                }
            }
            else if (proj.ModProjectile is BaseDeathray)
            {
                electricAttack = true;
            }
            else
            {
                string name = proj.ModProjectile.Name.ToLower();
                if (name.Contains("lightning") || name.Contains("electr") || name.Contains("thunder") || name.Contains("laser"))
                    electricAttack = true;
            }

            if (electricAttack && player.whoAmI == Main.myPlayer && !player.HasBuff(ModContent.BuffType<SuperchargedBuff>()))
            {
                dr = 0.5f;

                player.AddBuff(ModContent.BuffType<SuperchargedBuff>(), 60 * 30);

                foreach (Projectile p in Main.projectile.Where(p => p.active && p.minion && p.owner == player.whoAmI
                    && (p.type == ModContent.ProjectileType<Probe1>() || p.type == ModContent.ProjectileType<Probe2>())))
                {
                    p.ai[1] = 180;
                    p.netUpdate = true;
                }

                SoundEngine.PlaySound(SoundID.NPCDeath6, player.Center);
                SoundEngine.PlaySound(SoundID.Item92, player.Center);
                SoundEngine.PlaySound(SoundID.Item14, player.Center);

                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Vortex, 0f, 0f, 100, default, 3f);
                    Main.dust[dust].velocity *= 1.4f;
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(player.position, player.width, player.height, DustID.Torch, 0f, 0f, 100, default, 3.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 7f;
                    dust = Dust.NewDust(player.position, player.width, player.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 3f;
                }

                for (int index1 = 0; index1 < 30; ++index1)
                {
                    int index2 = Dust.NewDust(player.position, player.width, player.height, DustID.Vortex, 0f, 0f, 100, new Color(), 2f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].velocity *= 21f;
                    Main.dust[index2].noLight = true;
                    int index3 = Dust.NewDust(player.position, player.width, player.height, DustID.Vortex, 0f, 0f, 100, new Color(), 1f);
                    Main.dust[index3].velocity *= 12f;
                    Main.dust[index3].noGravity = true;
                    Main.dust[index3].noLight = true;
                }

                for (int i = 0; i < 20; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, DustID.Vortex, 0f, 0f, 100, default, Main.rand.NextFloat(2f, 5f));
                    if (Main.rand.NextBool(3))
                        Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= Main.rand.NextFloat(12f, 18f);
                    Main.dust[d].position = player.Center;
                }
            }
            return dr;
        }
    }
    public class ProbeMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DubiousHeader>();
        public override int ToggleItemType => ModContent.ItemType<GroundStick>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (!player.HasBuff<SouloftheMasochistBuff>())
                player.AddBuff(ModContent.BuffType<ProbesBuff>(), 2);
        }
    }
}