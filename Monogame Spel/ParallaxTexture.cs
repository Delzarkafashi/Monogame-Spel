using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monogame_Spel
{
    internal class ParallaxTexture
    {
        private Texture2D _texture;
        private Vector2 _position;

        public ParallaxTexture(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }

        public void Update(float speed)
        {
            _position.X -= speed; // Justera bakgrundens position baserat på spelarens rörelse
        }
    }

}
