﻿using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;

namespace PathOfModifiers.Rarities
{
    public class Rarity
    {
        public Mod mod;

        public virtual float weight => 1f;
        public virtual byte maxAffixes => 0;
        public virtual byte maxPrefixes => 0;
        public virtual byte maxSuffixes => 0;
        public virtual Color color => new Color(1, 1, 1, 1);
        public virtual int vanillaRarity => 0;
        public virtual string name => string.Empty;
        public virtual float chanceToRollAffix => 0;
        
        public virtual Rarity Clone()
        {
            Rarity newRarity = (Rarity)Activator.CreateInstance(GetType());
            newRarity.mod = mod;
            return newRarity;
        }
    }
}