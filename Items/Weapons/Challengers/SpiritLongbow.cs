//JAVYZ TODO: SPIRIT LONGBOW
/*

using FargowiltasSouls.Projectiles.ChallengerItems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace FargowiltasSouls.Items.Weapons.Challengers
{
    public class SpiritLongbow : SoulsItem
    {
        private int delay = 0;
        private bool lastLMouse = false;
        private int Frame = 0;
        private int FrameCount = 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Longbow");
            Tooltip.SetDefault("Converts arrows to Spirit Arrows that release spirit energy behind them\nHold button to charge shots for more damage and higher speed");

            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 66;
            Item.height = 30;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0.5f;
            Item.value = Item.sellPrice(0, 1, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = null;
            Item.channel = true;
            Item.shoot = 1;
            Item.autoReuse = true;

            Item.useAmmo = AmmoID.Arrow;
            Item.noMelee = true;
        }
        private float Charge = 1;
        int SoundTimer = 0;

        //can't figure out how drawing an item animation works, but this aint it
        /*public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Item[Item.type].Value.Height / FrameCount; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Item.GetAlpha(drawColor), 0, origin2, scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Item[Item.type].Value.Height / FrameCount; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.EntitySpriteDraw(texture2D13, Item.position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Item.GetAlpha(lightColor), rotation, origin2, Item.scale, SpriteEffects.None, 0);
            return false;
        }*/
/*
        public override void HoldItem(Player player)
        {
            Frame = (player.channel ? (int)Charge : 0);
            const int ChargeMax = 4;
            if (delay > 0)
                delay--;
            if (player.channel && delay == 0)
            {
                if (SoundTimer <= 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath6 with { Pitch = 0.1f + ((Charge - 1) / 3) }, player.Center);
                    SoundTimer = 10;
                }
                SoundTimer--;

                if (Charge <= ChargeMax) //charge up
                {
                    Charge += 1f/30f;
                }
                
            }
            else
            {
                SoundTimer = 0;
            }
            if (lastLMouse && !Main.mouseLeft && !player.channel && delay == 0) //first frame after you release channel, shoot projectile and make sound
            {
                delay = (int)(30 * (ChargeMax - Charge));
                SoundEngine.PlaySound(SoundID.Item102 with { Pitch = 0.1f + ((Charge-1)/3)}, player.Center);
                Vector2 direction = Vector2.Normalize(Main.MouseWorld - player.Center);
                float shootoffset = 0; //arrow start difference from player center
                float shootSpeed = 10;
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center + (direction * shootoffset), direction * shootSpeed * Charge, ModContent.ProjectileType<SpiritArrow>(), Damage: (int)(Item.damage * Charge), KnockBack: 3f, Owner: player.whoAmI);
                Charge = 1;
            }
            
            lastLMouse = Main.mouseLeft;
            base.HoldItem(player);
        }
        public override bool CanUseItem(Player player)
        {
            return (delay > 0) ? false : base.CanUseItem(player);

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false; //manual shooting in HoldItem
        }
        public override Vector2? HoldoutOffset()
        {
            Vector2 Offset = new(0, 0);
            if (Charge > 3.8)
            {
                float shake = 6;
                Offset += new Vector2(Main.rand.NextFloat(-shake, shake), Main.rand.NextFloat(-shake, shake));
                
            }

            return Offset;
        }
    }
}

*/