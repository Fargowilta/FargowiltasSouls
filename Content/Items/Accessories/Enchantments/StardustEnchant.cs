
using FargowiltasSouls.Content.Buffs.Souls;
using FargowiltasSouls.Content.Projectiles;
using FargowiltasSouls.Core;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Enchantments
{
    public class StardustEnchant : BaseEnchant
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override Color nameColor => new(0, 174, 238);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.rare = ItemRarityID.Purple;
            Item.value = 400000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<StardustMinionEffect>(Item);
            player.AddEffect<StardustEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.StardustHelmet)
            .AddIngredient(ItemID.StardustBreastplate)
            .AddIngredient(ItemID.StardustLeggings)
            //stardust wings
            //.AddIngredient(ItemID.StardustPickaxe);
            .AddIngredient(ItemID.StardustCellStaff) //estee pet
            .AddIngredient(ItemID.StardustDragonStaff)
            .AddIngredient(ItemID.RainbowCrystalStaff)
            //MoonlordTurretStaff


            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
    public class StardustMinionEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<StardustEnchant>();
        public override bool MinionEffect => true;
        public override void PostUpdateEquips(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.StardustGuardian] < 1)
            {
                FargoSoulsUtil.NewSummonProjectile(GetSource_EffectItem(player), player.Center, Vector2.Zero, ProjectileID.StardustGuardian, 30, 10f, Main.myPlayer);
            }
        }
    }
    public class StardustEffect : AccessoryEffect
    {
        public override Header ToggleHeader => null;
        public const int TIMESTOP_DURATION = 540; //300
        public override void PostUpdateEquips(Player player)
        {
            player.setStardust = true;
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (modPlayer.FreezeTime && modPlayer.freezeLength > 0)
            {
                player.buffImmune[ModContent.BuffType<TimeFrozenBuff>()] = true;

                if (modPlayer.freezeLength > 0 && Main.netMode != NetmodeID.Server)
                {
                    ManagedScreenFilter filter = ShaderManager.GetFilter("FargowiltasSouls.Invert");
                    filter.SetFocusPosition(player.Center);
                    if (modPlayer.freezeLength > 60)
                    {
                        filter.Activate();
                    }

                    if (SoulConfig.Instance.ForcedFilters && Main.WaveQuality == 0)
                        Main.WaveQuality = 1;
                }

                /*for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (p.active && !p.dead && !p.ghost && !p.HasEffect<StardustEffect>())
                    {
                        p.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), modPlayer.freezeLength);
                    }
                }*/

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.HasBuff(ModContent.BuffType<TimeFrozenBuff>()))
                        npc.AddBuff(ModContent.BuffType<TimeFrozenBuff>(), modPlayer.freezeLength);
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && !(p.minion && !ProjectileID.Sets.MinionShot[p.type]) && !p.FargoSouls().TimeFreezeImmune && p.FargoSouls().TimeFrozen == 0)
                    {
                        p.FargoSouls().TimeFrozen = modPlayer.freezeLength;

                        /*if (p.owner == Player.whoAmI && p.friendly && !p.hostile)
                        {
                            //p.maxPenetrate = 1;
                            if (!p.usesLocalNPCImmunity && !p.usesIDStaticNPCImmunity)
                            {
                                p.usesLocalNPCImmunity = true;
                                p.localNPCHitCooldown = 1;
                            }
                        }*/
                    }
                }

                modPlayer.freezeLength--;

                FargowiltasSouls.ManageMusicTimestop(modPlayer.freezeLength < 5);

                if (modPlayer.freezeLength == 90)
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(new SoundStyle("FargowiltasSouls/Assets/Sounds/ZaWarudoResume"), player.Center);
                }

                if (modPlayer.freezeLength <= 0)
                {
                    modPlayer.FreezeTime = false;
                    modPlayer.freezeLength = TIMESTOP_DURATION;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.dontTakeDamage && npc.life == 1 && npc.lifeMax > 1)
                            npc.SimpleStrikeNPC(int.MaxValue, 0, false, 0, null, false, 0, true);
                    }
                }
            }
        }
    }
}
