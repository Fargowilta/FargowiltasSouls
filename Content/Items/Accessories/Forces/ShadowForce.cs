using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Dyes;
using FargowiltasSouls.Content.Projectiles.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Core.Toggler.Content;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Items.Accessories.Forces
{
    public class ShadowForce : BaseForce
    {
        public override void SetStaticDefaults()
        {
            Enchants[Type] =
            [
                ModContent.ItemType<NinjaEnchant>(),
                ModContent.ItemType<AncientShadowEnchant>(),
                ModContent.ItemType<CrystalAssassinEnchant>(),
                ModContent.ItemType<SpookyEnchant>(),
                ModContent.ItemType<ShinobiEnchant>(),
                ModContent.ItemType<DarkArtistEnchant>(),
                ModContent.ItemType<NecroEnchant>()
            ];
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SetActive(player);
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            //player.AddEffect<NinjaEffect>(Item);
            //modPlayer.ApprenticeEnchantActive = true;
            //modPlayer.DarkArtistEnchantActive = true;
            //player.AddEffect<ApprenticeSupport>(Item);
            //player.AddEffect<DarkArtistMinion>(Item);
            //player.AddEffect<NecroEffect>(Item);
            //shadow orbs
            //modPlayer.AncientShadowEnchantActive = true;
            //player.AddEffect<ShadowBalls>(Item);
            //darkness debuff
            //player.AddEffect<AncientShadowDarkness>(Item);
            //shinobi and monk effects
            //ShinobiEnchant.AddEffects(player, Item);
            //smoke bomb nonsense
            //CrystalAssassinEnchant.AddEffects(player, Item);
            //scythe doom
            //player.AddEffect<SpookyEffect>(Item);
            player.AddEffect<ShadowForceDashEffect>(Item);
            player.AddEffect<ShadowForceDamageEffect>(Item);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            foreach (int ench in Enchants[Type])
                recipe.AddIngredient(ench);
            recipe.AddTile(ModContent.Find<ModTile>("Fargowiltas", "CrucibleCosmosSheet"));
            recipe.Register();
        }
    }
    public class ShadowForceDamageEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShadowForce>();
        public override void PostUpdateEquips(Player player)
        {
            player.GetDamage(DamageClass.Generic) += MathHelper.Lerp(0, 0.4f, 1 - MathHelper.Clamp(player.velocity.Length() / 7f, 0, 1));

            
            if (Main.rand.NextBool((int)MathHelper.Lerp(7, 20, MathHelper.Clamp(player.velocity.Length() / 7f, 0, 1))) && player.velocity.Length() <= 7){
                Particle dot = new AlphaBloomParticle(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), new Vector2(0, Main.rand.NextFloat(-0.5f, -0.1f)), Color.Black, Vector2.One * 0.4f, Vector2.One * 0.1f, 30);
                dot.Spawn();
            }
        }
    }
    public class ShadowForceDashEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<ShadowHeader>();
        public override int ToggleItemType => ModContent.ItemType<ShadowForce>();
        
        public override void PostUpdateEquips(Player player)
        {
            FargoSoulsPlayer farg = player.FargoSouls();
            if (farg.IFrameDashTimer > 0)
            {
                farg.IFrameDashTimer--;
            }
            if (farg.IFrameDashTimer == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Pitch = -0.5f });
                Particle boom = new AlphaBloomParticle(player.Center, Vector2.Zero, Color.Black, Vector2.One * 8, Vector2.One * 1, 40, false);
                boom.Spawn();
                for (int i = 0; i < 10; i++)
                {
                    Vector2 pos = player.Center + new Vector2(0, Main.rand.NextFloat(20, 100)).RotatedByRandom(MathHelper.TwoPi);
                    Particle boom2 = new AlphaBloomParticle(pos, (player.Center - pos).SafeNormalize(Vector2.Zero)*2, Color.Black, Vector2.One * 1, Vector2.One * 0.1f, 50, false);
                    boom2.Spawn();
                }
            }
            if (farg.IsDashingTimer <= 0)
            {
                farg.IFrameDash = false;
            }
            if (farg.ShadowDashTimer > 0)
            {
                Color color = Color.Purple;
                if (farg.IFrameDash)
                {
                    player.immuneNoBlink = true;
                    player.immuneTime = 5;
                    player.immune = true;
                    player.AddImmuneTime(ImmunityCooldownID.Bosses, 1);
                    color = Color.Black;
                    Particle part = new AlphaSparkParticle(player.oldPosition + player.Size * 0.5f, player.velocity * 0.4f, Color.Black, 1, 10, false);
                    part.Spawn();
                    player.Yoraiz0rEye();
                }
                farg.ShadowDashTimer--;
                
                Particle bub = new AlphaSparkParticle(player.position + new Vector2(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height)), -(player.velocity * 0.1f) , color, 0.5f, 10, false);
                bub.Spawn();

                

            }
            
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            FargoSoulsPlayer farg = player.FargoSouls();
            if (farg.IFrameDash)
            {
                float jerp = farg.ShadowDashTimer / 10f;
                jerp = MathHelper.Clamp(jerp, 0f, 1f);
                r = MathHelper.Lerp(r, 0, jerp);
                g = MathHelper.Lerp(g, 0, jerp);
                b = MathHelper.Lerp(b, 0, jerp);
            }

            Asset<Texture2D> texture = ModContent.Request<Texture2D>("FargowiltasSouls/Content/Projectiles/GlowRing", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            //Main.EntitySpriteDraw(texture.Value, player.Center - Main.screenPosition, null, Color.Black * (1 - MathHelper.Clamp(player.velocity.Length() / 7f, 0.5f, 1f))*0.5f, 1, texture.Size() / 2, 1, SpriteEffects.None);
        }
        public static void AddDash(Player player)
        {
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            modPlayer.HasDash = true;
            modPlayer.FargoDash = DashManager.DashType.Shadow;
        }
        public static void ShadowDash(Player player, int dir)
        {
            
            
            
            
            player.FargoSouls().ShadowDashTimer = 30;
            

            FargoSoulsPlayer modPlayer = player.FargoSouls();
            float dashSpeed = 25f;
            player.velocity.X = dashSpeed * dir;
            if (modPlayer.IsDashingTimer < 30)
                modPlayer.IsDashingTimer = 30;
            player.dashDelay = 60;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);

            if (player.FargoSouls().IFrameDashTimer <= 0)
            {
                player.FargoSouls().IFrameDash = true;
                player.FargoSouls().IFrameDashTimer = 250;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Pitch = -0.5f });

                Particle boom = new AlphaBloomParticle(player.Center, Vector2.Zero, Color.Black, Vector2.One * 1, Vector2.One * 10, 20, false);
                boom.Spawn();
                for (int i = 0; i < 10; i++)
                {
                    Particle boom2 = new AlphaBloomParticle(player.Center, -player.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-30, 30))) * Main.rand.NextFloat(0.1f, 0.4f), Color.Black, Vector2.One * 1, Vector2.One * 0.1f, 100, false);
                    boom2.Spawn();
                }

                int proj = Projectile.NewProjectile(player.GetSource_EffectItem<ShadowForceDashEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<ShadowDash>(), 5000, 3, player.whoAmI);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncProjectile, number: proj);
            }
        }
    }
}
