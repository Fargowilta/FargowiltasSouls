using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasSouls.Items.Misc;
using FargowiltasSouls.Toggler;

namespace FargowiltasSouls.Items.Accessories.Forces
{
    public class CosmoForce : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Force of Cosmos");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "宇宙之力");
            
            string tooltip =
@"A meteor shower initiates every few seconds while attacking
Solar shield allows you to dash through enemies
Attacks may inflict the Solar Flare debuff
Double tap down to toggle stealth, reducing chance for enemies to target you but slowing movement
You also spawn a vortex to draw in and massively damage enemies when you enter stealth
Hurting enemies has a chance to spawn buff boosters
Double tap down to direct your empowered guardian
Press the Freeze Key to freeze time for 5 seconds
Stardust Guardian gains a strong attack if enabled while time is frozen
There is a 60 second cooldown for this effect, a sound effect plays when it's back
'Been around since the Big Bang'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"攻击时每过几秒便会释放一次流星雨
召唤日耀护盾，让你能冲撞敌人
攻击有几率造成太阳耀斑减益
双击“下”键切换至隐形模式，减少敌人以你为目标的几率，但大幅降低移动速度
进入隐形状态时生成一个会吸引并伤害敌人的旋涡
对敌人造成伤害时有几率生成强化焰
双击“下”键将你的守卫引至光标位置
按下“冻结”键后会冻结时间，持续5秒
星尘守卫在时间冻结期间获得一种全新而强大的攻击
此效果有60秒冷却时间，冷却结束时会播放音效
“自宇宙大爆炸以来就一直存在”";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);

        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.rare = ItemRarityID.Purple;
            item.value = 600000;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //meme speed, solar flare,
            modPlayer.CosmoForce = true;

            //meteor shower
            modPlayer.MeteorEffect();
            //solar shields
            modPlayer.SolarEffect();
            //flare debuff
            modPlayer.SolarEnchant = true;
            //stealth, voids, pet
            modPlayer.VortexEffect(hideVisual);
            //boosters and meme speed
            modPlayer.NebulaEffect();
            //guardian and time freeze
            modPlayer.StardustEffect();
            //modPlayer.AddPet(player.GetToggleValue("PetSuspEye"), hideVisual, BuffID.SuspiciousTentacle, ProjectileID.SuspiciousTentacle);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);

            recipe.AddIngredient(null, "MeteorEnchant");
            recipe.AddIngredient(null, "SolarEnchant");
            recipe.AddIngredient(null, "VortexEnchant");
            recipe.AddIngredient(null, "NebulaEnchant");
            recipe.AddIngredient(null, "StardustEnchant");
            recipe.AddIngredient(ModContent.ItemType<LunarCrystal>(), 5);
            //recipe.AddIngredient(ItemID.SuspiciousLookingTentacle);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
