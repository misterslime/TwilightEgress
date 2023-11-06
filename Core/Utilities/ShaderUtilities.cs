using static Cascade.Assets.Effects.ShaderManager;

namespace Cascade
{
    public static partial class Utilities
    {
        private static readonly NullReferenceException ShaderNotFoundException = new("The following shader was not found from the given key, " +
                "likely meaning the key is not registered in Cascade's shader dictionary. Please input a viable key.");

        /// <summary>
        /// Tries to obtain a shader from Cascade's shader dictionary using the given key.
        /// The given key MUST match the internal key which a shader is registered under in
        /// the aforementioned shader dictionary, which can be found in <see cref="Shaders"/>.
        /// </summary>
        /// <returns>The shader as <see cref="MiscShaderData"/>.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public static MiscShaderData TryGetShader(string shaderKey)
        {
            if (Shaders.ContainsKey(shaderKey))
                return Shaders[shaderKey];
            throw ShaderNotFoundException;
        }

        public static Filter TryGetScreenShader(string shaderKey)
        {
            if (ScreenShaders.ContainsKey(shaderKey))
                return ScreenShaders[shaderKey];
            throw ShaderNotFoundException;
        }

        // This is quite a crude way of doing things, but it's what works. Can't think of a better way to this at the moment, but if I do this'll definitely be changed.

        // Texture Shaders.
        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, float value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, float[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, int value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, int[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, bool value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, bool[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector2 value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector2[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector3 value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector3[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector4 value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this MiscShaderData shader, string parameterKey, Vector4[] value) => shader.Shader.Parameters[parameterKey].SetValue(value);

        // Screen Shaders.
        public static void TrySetParameterValue(this Filter shader, string parameterKey, float value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, float[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, int value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, int[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, bool value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, bool[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector2 value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector2[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector3 value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector3[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector4 value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);

        public static void TrySetParameterValue(this Filter shader, string parameterKey, Vector4[] value) => shader.GetShader().Shader.Parameters[parameterKey].SetValue(value);
    }
}
