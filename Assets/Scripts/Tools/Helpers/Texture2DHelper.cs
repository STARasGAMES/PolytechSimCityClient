using System;
using UnityEngine;

namespace Tools.Helpers
{
    public class ImageData
    {
        private Color[] _colors;
        private int _width;
        private int _height;

        public ImageData(Texture2D _texture)
        {
            _colors = _texture.GetPixels();
            _width = _texture.width;
            _height = _texture.height;
        }
#if !OCULUS
        public ImageData(WebCamTexture _texture)
        {
            _colors = _texture.GetPixels();
            _width = _texture.width;
            _height = _texture.height;
        }
#endif

        public void RotatePixels90()
        {
            var clone = _colors.Clone() as Color[];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _colors[y + (_width - 1 - x) * _height] = clone[x + y * _width];
                }
            }

            var temp = _width;
            _width = _height;
            _height = temp;
        }

        public void RotatePixels180()
        {
            Array.Reverse(_colors);
        }

        public void RotatePixels270()
        {
            var clone = _colors.Clone() as Color[];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _colors[_height - 1 - y + x * _height] = clone[x + y * _width];
                }
            }

            var temp = _width;
            _width = _height;
            _height = temp;
        }

        public void FlipPixelsHorizontally()
        {
            var clone = _colors.Clone() as Color[];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _colors[_width - x - 1 + y * _width] = clone[x + y * _width];
                }
            }
        }

        public void FlipPixelsVertically()
        {
            var clone = _colors.Clone() as Color[];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _colors[x + (_height - 1 - y) * _width] = clone[x + y * _width];
                }
            }
        }

        public Texture2D GetTexture(TextureFormat format)
        {
            var result = new Texture2D(_width, _height, format, false);
            result.SetPixels(_colors);
            result.Apply();
            return result;
        }
    }
}
