using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasSouls.Items.Accessories.Souls
{
    [AutoloadEquip(EquipType.Shield)]
    public class TerrariaSoul : SoulsItem
    {
        public override void SetStaticDefaults()
        {
            /*DisplayName.SetDefault("Soul of Terraria");
            
            DisplayName.AddTranslation(GameCulture.Chinese, "泰拉之魂");
            
            string tooltip =
@"Summons fireballs, shadow orbs, icicles, leaf crystals, flameburst minion, hallowed sword and shield, and beetles
Right Click to Guard
Double tap down to spawn a sentry and portal, call a storm and arrow rain, toggle stealth, and direct your empowered guardian
Gold Key encases you in gold, Freeze Key freezes time for 5 seconds, minions spew scythes
Solar shield allows you to dash, Dash into any walls, to teleport through them
Throw a smoke bomb to teleport to it and gain the First Strike Buff
Attacks may spawn lightning, a storm cloud, flower petals, spectre orbs, a Dungeon Guardian, snowballs, spears, or buff boosters
Attacks cause increased life regen, shadow dodge, Flameburst shots, meteor showers, and reduced enemy immune frames
Critical chance is set to 25%, Crit to increase it by 5%, At 100% every 10th attack gains 4% life steal
Getting hit drops your crit back down, trigger a blood geyser, and reflects damage
Projectiles may split or shatter and spawn stars, item and projectile size increased, attract items from further away
Nearby enemies are ignited, You leave behind a trail of fire, jump to create a spore explosion
Grants Crimson regen, immunity to fire, fall damage, and lava, and doubled herb collection
Grants 50% chance for Mega Bees, 15% chance for minion crits, 20% chance for bonus loot
Critters have increased defense and their souls will aid you, You may summon temporary minions
All grappling hooks are more effective and fire homing shots, Greatly enhances all DD2 sentries
Your attacks inflict Midas, Enemies explode into needles
You violently explode to damage nearby enemies when hurt and revive with 200 HP when killed
Effects of Flower Boots and Greedy Ring
'A true master of Terraria'";
            Tooltip.SetDefault(tooltip);

            string tooltip_ch =
@"召唤火球、暗影珠、冰锥、叶状水晶、爆炸烈焰哨兵、神圣剑、神圣盾和甲虫
右键进行盾牌格挡
双击'下'键后会召唤一个哨兵、传送门、风暴、箭雨和你的的守卫至光标位置并切换至隐形状态
按下'金身'键后会将你包裹在一个黄金壳中并冻结5秒时间，召唤物会释放镰刀
允许你使用日耀护盾进行冲刺，冲进墙壁时会直接穿过去
扔出烟雾弹后会将你传送至其落点的位置并使你获得先发制人增益
攻击有几率生成闪电、风暴云、花瓣、幽魂珠、地牢守卫、雪球、长矛或强化增益
攻击额外发射爆炸烈焰火球且会使流星雨从天而降，攻击敌人会降低其防御、增加你的生命恢复速度并使你获得暗影闪避增益
将你的基础暴击率设为25%，每次暴击时都会增加5%暴击率，暴击率达到100%后每攻击10次会汲取4%生命值
被击中后会降低暴击率、喷出血液并反弹伤害
弹幕有几率分裂或爆裂成碎片并生成星星，增加物品和弹幕的尺寸，扩大你的物品拾取范围
引燃你附近的敌人，你的身后会留下一条火痕，跳跃会释放孢子爆炸
给予你猩红魔石的恢复效果，使你免疫岩浆、火和摔落伤害并使药草收获翻倍
普通蜜蜂有50%几率变成大蜜蜂，你的仆从和哨兵现在可以造成暴击且有15%基础暴击率，敌人死亡时有20%几率掉落更多战利品
增加小动物的防御，小动物死亡后会释放它们的灵魂来帮助你，在召唤栏用光后你仍可以召唤临时的哨兵和仆从
强化钩爪的效果，钩爪会发射追踪弹幕，大幅强化旧日军团的塔防哨兵
你的攻击会造成迈达斯减益，敌人在死亡时会爆裂出针刺
你受到伤害时会剧烈爆炸并伤害附近的敌人，你在重生时以200点生命值重生
拥有花靴和贪婪戒指效果
'泰拉之主，天地共证'";
            Tooltip.AddTranslation(GameCulture.Chinese, tooltip_ch);*/

            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 24));
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.mod == "Terraria" && line.Name == "ItemName")
            {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(ItemID.LivingRainbowDye), item, null); //use living rainbow dye shader
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1); //draw the tooltip manually
                Main.spriteBatch.End(); //then end and begin again to make remaining tooltip lines draw in the default way
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            ItemID.Sets.ItemNoGravity[item.type] = true;
            item.value = 5000000;

            item.rare = -12;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            FargoPlayer modPlayer = player.GetModPlayer<FargoPlayer>();
            //includes revive, both spectres, adamantite, and star heal
            modPlayer.TerrariaSoul = true;

            //WOOD
            mod.GetItem("TimberForce").UpdateAccessory(player, hideVisual);
            //TERRA
            mod.GetItem("TerraForce").UpdateAccessory(player, hideVisual);
            //EARTH
            mod.GetItem("EarthForce").UpdateAccessory(player, hideVisual);
            //NATURE
            mod.GetItem("NatureForce").UpdateAccessory(player, hideVisual);
            //LIFE
            mod.GetItem("LifeForce").UpdateAccessory(player, hideVisual);
            //SPIRIT
            mod.GetItem("SpiritForce").UpdateAccessory(player, hideVisual);
            //SHADOW
            mod.GetItem("ShadowForce").UpdateAccessory(player, hideVisual);
            //WILL
            mod.GetItem("WillForce").UpdateAccessory(player, hideVisual);
            //COSMOS
            mod.GetItem("CosmoForce").UpdateAccessory(player, hideVisual);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "TimberForce");
            recipe.AddIngredient(null, "TerraForce");
            recipe.AddIngredient(null, "EarthForce");
            recipe.AddIngredient(null, "NatureForce");
            recipe.AddIngredient(null, "LifeForce");
            recipe.AddIngredient(null, "SpiritForce");
            recipe.AddIngredient(null, "ShadowForce");
            recipe.AddIngredient(null, "WillForce");
            recipe.AddIngredient(null, "CosmoForce");
            recipe.AddIngredient(null, "AbomEnergy", 10);

            recipe.AddTile(ModLoader.GetMod("Fargowiltas").TileType("CrucibleCosmosSheet"));

            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
