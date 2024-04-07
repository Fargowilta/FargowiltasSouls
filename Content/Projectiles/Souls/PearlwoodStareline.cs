using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasSouls.Content.Projectiles.Souls
{
    public class PearlwoodStareline : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.FairyQueenMagicItemShot;
        public override bool InstancePerEntity => true;
        public bool Pearlwood = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (player == null || !player.active || player.dead || !(modPlayer.PearlwoodStar && player.HasEffect<PearlwoodEffect>()))
                return;
            int[] pearlwoodItems = new int[] { ModContent.ItemType<PearlwoodEnchant>(), ModContent.ItemType<TimberForce>(), ModContent.ItemType<TerrariaSoul>() };
            if (source is EntitySource_ItemUse itemSource && pearlwoodItems.Contains(itemSource.Item.type))
            {
                SoundEngine.PlaySound(SoundID.Item84, projectile.Center);
                Pearlwood = true;

                projectile.hostile = true;
                projectile.friendly = false;
                projectile.penetrate = 1;
                projectile.timeLeft = 22;
                //projectile.aiStyle = -1;
                projectile.tileCollide = false;

            }

        }
        /*
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 22;
        }
       
        public override string Texture => "Terraria/Images/Projectile_723"; 
        */
        public override void AI(Projectile projectile)
        {
            if (!Pearlwood)
            {
                base.AI(projectile);
                return;
            }
            base.AI(projectile);

            Player player = Main.player[projectile.owner];
            if (player == null || !player.active || player.dead)
            {
                projectile.Kill();
                return;
            }
            FargoSoulsPlayer modPlayer = player.FargoSouls();
            if (!modPlayer.PearlwoodStar || !player.HasEffect<PearlwoodEffect>())
            {
                projectile.Kill();
                return;
            } //kill projkcetoiele when unequip or toggled off or player dies or leaves or commits a war crime idfk

            //damage enemies if force
            bool force = modPlayer.ForceEffect<PearlwoodEnchant>();
            projectile.friendly = force;

            //refresh lifetime
            projectile.timeLeft = 22;

            //damage to make sure it never can change
            projectile.damage = 1000;

            //spin
            //projectile.rotation += MathHelper.ToRadians(6);

            //sparkle on queen
            //Dust.NewDust(modPlayer.PStarelinePos, projectile.width, projectile.height, DustID.TintableDustLighted, default, default, 200, Color.Fuchsia); //idk how to make dust look good


            //follow the player
            projectile.velocity = modPlayer.PStarelinePos - projectile.Center;
        }
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (Pearlwood && target.type == NPCID.TargetDummy)
                return false;
            return base.CanHitNPC(projectile, target);
        }
        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (!Pearlwood)
            {
                base.OnHitPlayer(projectile, target, info);
                return;
            }
            SoundEngine.PlaySound(SoundID.Item25, target.Center);
            projectile.Kill();
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!Pearlwood)
            {
                base.ModifyHitPlayer(projectile, target, ref modifiers);
                return;
            }
            modifiers.SetMaxDamage((int)(target.statLifeMax2 * 0.2)); //hits player for 1/5th max hp damage
            modifiers.ScalingArmorPenetration += 1;
        }


        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (!Pearlwood)
            {
                base.OnKill(projectile, timeLeft);
                return;
            }
            Player player = Main.player[projectile.owner];
            FargoSoulsPlayer modPlayer = player.FargoSouls();

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(modPlayer.PStarelinePos, 22, 22, DustID.GoldFlame, 0f, 0f, 175, default, 1.75f);
            }

            //modPlayer.PStarelineActive = false;
        }

    }
}
