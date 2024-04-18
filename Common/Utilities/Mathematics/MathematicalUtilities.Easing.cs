using static Luminance.Common.Easings.EasingCurves;

namespace Cascade
{
    public static partial class CascadeUtilities
    {
        public static float LinearEase(float interpolant) => interpolant;

        public static float SineEaseIn(float interpolant) => Sine.Evaluate(EasingType.In, interpolant);

        public static float SineEaseOut(float interpolant) => Sine.Evaluate(EasingType.Out,interpolant);

        public static float SineEaseInOut(float interpolant) => Sine.Evaluate(EasingType.InOut, interpolant);

        public static float CubicEaseIn(float interpolant) => Cubic.Evaluate(EasingType.In, interpolant);

        public static float CubicEaseOut(float interpolant) => Cubic.Evaluate(EasingType.Out, interpolant);

        public static float CubicEaseInOut(float interpolant) => Cubic.Evaluate(EasingType.InOut, interpolant);

        public static float QuintEaseIn(float interpolant) => Quintic.Evaluate(EasingType.In, interpolant);

        public static float QuintEaseOut(float interpolant) => Quintic.Evaluate(EasingType.Out, interpolant);

        public static float QuintEaseInOut(float interpolant) => Quintic.Evaluate(EasingType.InOut, interpolant);

        public static float CircEaseIn(float interpolant) => Circ.Evaluate(EasingType.In, interpolant);

        public static float CircEaseOut(float interpolant) => Circ.Evaluate(EasingType.Out, interpolant);

        public static float CircEaseInOut(float interpolant) => Circ.Evaluate(EasingType.InOut, interpolant);

        public static float QuadEaseIn(float interpolant) => Quadratic.Evaluate(EasingType.In, interpolant);

        public static float QuadEaseOut(float interpolant) => Quadratic.Evaluate(EasingType.Out, interpolant);

        public static float QuadEaseInOut(float interpolant) => Quadratic.Evaluate(EasingType.InOut, interpolant);

        public static float QuartEaseIn(float interpolant) => Quartic.Evaluate(EasingType.In, interpolant);

        public static float QuartEaseOut(float interpolant) => Quartic.Evaluate(EasingType.Out, interpolant);

        public static float QuartEaseInOut(float interpolant) => Quartic.Evaluate(EasingType.InOut, interpolant);

        public static float ExpoEaseIn(float interpolant) => EasingCurves.Exp.Evaluate(EasingType.In, interpolant);

        public static float ExpoEaseOut(float interpolant) => EasingCurves.Exp.Evaluate(EasingType.Out, interpolant);

        public static float ExpoEaseInOut(float interpolant) => EasingCurves.Exp.Evaluate(EasingType.InOut, interpolant);
    }
}
