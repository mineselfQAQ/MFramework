using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MFramework.Text
{
    public enum MTextInlineEffectType
    {
        Wave,
        Shake,
        Pulse,
        Color,
    }

    public readonly struct MTextInlineEffectRange
    {
        public MTextInlineEffectRange(MTextInlineEffectType type, int startIndex, int endIndex, Color color)
        {
            Type = type;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Color = color;
        }

        public MTextInlineEffectType Type { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }
        public Color Color { get; }
    }

    public readonly struct MTextParseResult
    {
        public MTextParseResult(string text, IReadOnlyList<MTextInlineEffectRange> effects)
        {
            Text = text;
            Effects = effects;
        }

        public string Text { get; }
        public IReadOnlyList<MTextInlineEffectRange> Effects { get; }
    }

    public static class MTextInlineParser
    {
        private readonly struct OpenTag
        {
            public OpenTag(MTextInlineEffectType type, int startIndex, Color color)
            {
                Type = type;
                StartIndex = startIndex;
                Color = color;
            }

            public MTextInlineEffectType Type { get; }
            public int StartIndex { get; }
            public Color Color { get; }
        }

        public static MTextParseResult Parse(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return new MTextParseResult(string.Empty, Array.Empty<MTextInlineEffectRange>());
            }

            List<MTextInlineEffectRange> effects = new List<MTextInlineEffectRange>();
            Stack<OpenTag> stack = new Stack<OpenTag>();
            System.Text.StringBuilder builder = new System.Text.StringBuilder(source.Length);

            for (int i = 0; i < source.Length;)
            {
                if (TryReadTag(source, i, out string tag, out int length))
                {
                    if (TryOpenTag(tag, builder.Length, out OpenTag openTag))
                    {
                        stack.Push(openTag);
                        i += length;
                        continue;
                    }

                    if (IsCloseTag(tag) && stack.Count > 0)
                    {
                        OpenTag open = stack.Pop();
                        int endIndex = builder.Length - 1;
                        if (endIndex >= open.StartIndex)
                        {
                            effects.Add(new MTextInlineEffectRange(open.Type, open.StartIndex, endIndex, open.Color));
                        }
                        i += length;
                        continue;
                    }
                }

                builder.Append(source[i]);
                i++;
            }

            return new MTextParseResult(builder.ToString(), effects);
        }

        private static bool TryReadTag(string source, int start, out string tag, out int length)
        {
            tag = null;
            length = 0;
            if (source[start] != '{') return false;

            int end = source.IndexOf('}', start + 1);
            if (end < 0) return false;

            tag = source.Substring(start + 1, end - start - 1).Trim();
            length = end - start + 1;
            return tag.Length > 0;
        }

        private static bool TryOpenTag(string tag, int startIndex, out OpenTag openTag)
        {
            openTag = default;

            if (EqualsTag(tag, "wave"))
            {
                openTag = new OpenTag(MTextInlineEffectType.Wave, startIndex, Color.white);
                return true;
            }

            if (EqualsTag(tag, "shake"))
            {
                openTag = new OpenTag(MTextInlineEffectType.Shake, startIndex, Color.white);
                return true;
            }

            if (EqualsTag(tag, "pulse"))
            {
                openTag = new OpenTag(MTextInlineEffectType.Pulse, startIndex, Color.white);
                return true;
            }

            const string colorPrefix = "color=";
            if (tag.StartsWith(colorPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string value = tag.Substring(colorPrefix.Length).Trim();
                if (TryParseColor(value, out Color color))
                {
                    openTag = new OpenTag(MTextInlineEffectType.Color, startIndex, color);
                    return true;
                }
            }

            return false;
        }

        private static bool IsCloseTag(string tag)
        {
            return EqualsTag(tag, "/wave") ||
                   EqualsTag(tag, "/shake") ||
                   EqualsTag(tag, "/pulse") ||
                   EqualsTag(tag, "/color");
        }

        private static bool EqualsTag(string lhs, string rhs)
        {
            return string.Equals(lhs, rhs, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryParseColor(string value, out Color color)
        {
            if (ColorUtility.TryParseHtmlString(value, out color))
            {
                return true;
            }

            if (uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint rgb))
            {
                color = new Color32((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb, 255);
                return true;
            }

            color = Color.white;
            return false;
        }
    }
}
