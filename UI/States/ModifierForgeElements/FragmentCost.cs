﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace PathOfModifiers.UI.States.ModifierForgeElements
{
	public class FragmentCost : UIText
	{
		public FragmentCost(string text, float textScale = 1f, bool large = false) : base(text, textScale, large) { }
        public FragmentCost(LocalizedText text, float textScale = 1f, bool large = false) : base(text, textScale, large) { }

        public override void Recalculate()
        {
            base.Recalculate();
            MarginLeft = GetInnerDimensions().Height * 1.2f + 3f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dims = GetOuterDimensions();
            var dest = new Rectangle((int)dims.X, (int)dims.Y, (int)(dims.Height * 1.2f), (int)(dims.Height * 1.2f));
            var texture = ModContent.Request<Texture2D>("PathOfModifiers/Items/ModifierFragment", AssetRequestMode.ImmediateLoad).Value;
            spriteBatch.Draw(texture, dest, null, Color.White);

            base.DrawSelf(spriteBatch);
        }
    }
}
