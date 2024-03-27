using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Monogame_Spel
{
    public class Block
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Size { get; set; } // Lägg till en egenskap för storlek


        public Block(Vector2 position, Texture2D texture, Vector2 size)
        {
            Position = position;
            Texture = texture;
            Size = size;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
        }

    }

}
