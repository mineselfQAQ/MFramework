using MFramework.Text;
using NUnit.Framework;

namespace MFramework.Tests.Features.MText
{
    public sealed class MTextInlineParserFeatureTests
    {
        [Test]
        public void FeatureMTextInlineParser_WithEffectTags_StripsTagsAndReturnsRanges()
        {
            MTextParseResult result = MTextInlineParser.Parse("你好 {wave}世界{/wave}!");

            Assert.AreEqual("你好 世界!", result.Text);
            Assert.AreEqual(1, result.Effects.Count);
            Assert.AreEqual(MTextInlineEffectType.Wave, result.Effects[0].Type);
            Assert.AreEqual(3, result.Effects[0].StartIndex);
            Assert.AreEqual(4, result.Effects[0].EndIndex);
        }

        [Test]
        public void FeatureMTextInlineParser_WithColorTag_ParsesColorRange()
        {
            MTextParseResult result = MTextInlineParser.Parse("{color=#ff0000}Alert{/color}");

            Assert.AreEqual("Alert", result.Text);
            Assert.AreEqual(1, result.Effects.Count);
            Assert.AreEqual(MTextInlineEffectType.Color, result.Effects[0].Type);
            Assert.AreEqual(0, result.Effects[0].StartIndex);
            Assert.AreEqual(4, result.Effects[0].EndIndex);
        }
    }
}
