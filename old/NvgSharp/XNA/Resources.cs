using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NvgSharp
{
    internal static class Resources
    {
        private static byte[] _effectSource = null, _effectWithAASource = null;

#if MONOGAME
        private static bool? _isOpenGL;
#endif

        public static byte[] GetNvgEffectSource(bool edgeAntiAlias)
        {
            if (_effectSource != null && !edgeAntiAlias)
            {
                return _effectSource;
            }

            if (_effectWithAASource != null && edgeAntiAlias)
            {
                return _effectWithAASource;
            }

            Assembly assembly = typeof(Resources).Assembly;

            string name = "Effect";
            if (edgeAntiAlias)
            {
                name += "_AA";
            }

#if MONOGAME
            string path = IsOpenGL ? "NvgSharp.MonoGame.Resources." + name + ".ogl.mgfxo" : "NvgSharp.MonoGame.Resources." + name + ".dx11.mgfxo";
#elif FNA
			var path = "NvgSharp.Resources." + name + ".fxb";
#endif

            byte[] result;

            MemoryStream ms = new MemoryStream();
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                stream.CopyTo(ms);
                result = ms.ToArray();
            }

            if (edgeAntiAlias)
            {
                _effectWithAASource = result;
            }
            else
            {
                _effectSource = result;
            }

            return result;
        }

#if MONOGAME
        public static bool IsOpenGL
        {
            get
            {
                if (_isOpenGL == null)
                {
                    _isOpenGL = (from f in typeof(GraphicsDevice).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance)
                                 where f.Name == "glFramebuffer"
                                 select f).FirstOrDefault() != null;
                }

                return _isOpenGL.Value;
            }
        }
#endif
    }
}
