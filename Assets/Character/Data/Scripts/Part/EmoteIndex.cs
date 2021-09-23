using Asgla.Data.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterCreator2D {
    [Serializable]
    public class EmoteIndex {
        public string name;
        public Part eyebrowPart;
        public Part eyesPart;
        public Part nosePart;
        public Part mouthPart;
        public Part earPart;

        public Part getPart(Equipment slot) {
            switch (slot) {
                case (Equipment.Eyebrow): return this.eyebrowPart;
                case (Equipment.Eye): return this.eyesPart;
                case (Equipment.Nose): return this.nosePart;
                case (Equipment.Mouth): return this.mouthPart;
                case (Equipment.Ear): return this.earPart;
                default: return null;
            }
        }
        public void setPart(Equipment slot, Part part) {
            switch (slot) {
                case (Equipment.Eyebrow): this.eyebrowPart = part == null ? null : part; break;
                case (Equipment.Eye): this.eyesPart = part == null ? null : part; break;
                case (Equipment.Nose): this.nosePart = part == null ? null : part; break;
                case (Equipment.Mouth): this.mouthPart = part == null ? null : part; break;
                case (Equipment.Ear): this.earPart = part == null ? null : part; break;
            }
        }
    }

    [Serializable]
    public class EmotionList {
        public EmoteIndex blink;
        public EmoteIndex attack;
        public EmoteIndex hurt;
        public EmoteIndex talk;

        public EmoteIndex customEmote0;
        public EmoteIndex customEmote1;
        public EmoteIndex customEmote2;
        public EmoteIndex customEmote3;
        public EmoteIndex customEmote4;
        public EmoteIndex customEmote5;
        public EmoteIndex customEmote6;
        public EmoteIndex customEmote7;
        public EmoteIndex customEmote8;
        public EmoteIndex customEmote9;

        public void resetPresetName() {
            this.blink.name = "Blink";
            this.attack.name = "Attack";
            this.hurt.name = "Hurt";
            this.talk.name = "Talk";
        }

        public EmoteIndex getIndex(EmotionType emotion) {
            switch (emotion) {
                case (EmotionType.Blink): return this.blink;
                case (EmotionType.Attack): return this.attack;
                case (EmotionType.Hurt): return this.hurt;
                case (EmotionType.Talk): return this.talk;

                case (EmotionType.Emote_0): return this.customEmote0;
                case (EmotionType.Emote_1): return this.customEmote1;
                case (EmotionType.Emote_2): return this.customEmote2;
                case (EmotionType.Emote_3): return this.customEmote3;
                case (EmotionType.Emote_4): return this.customEmote4;
                case (EmotionType.Emote_5): return this.customEmote5;
                case (EmotionType.Emote_6): return this.customEmote6;
                case (EmotionType.Emote_7): return this.customEmote7;
                case (EmotionType.Emote_8): return this.customEmote8;
                case (EmotionType.Emote_9): return this.customEmote9;
                default: return null;
            }
        }
    }

}