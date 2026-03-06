using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Luminance.Core.Graphics;

namespace OSTARsSWORDS.Content.Particles;

public class ChromaticStreak : Particle
{
	public override string AtlasTextureName => string.Empty;
	public override BlendState BlendState => BlendState.Additive;

	public float Stretch = 3.2f;

	public override void Update()
	{
		Position += Velocity;
		Velocity *= 0.93f;

		// Face along motion with a bit of chaos.
		Rotation = Velocity.LengthSquared() <= 0.01f ? Rotation + RotationSpeed : Velocity.ToRotation() + RotationSpeed;

		Opacity = 1f - LifetimeRatio;
		Scale *= 0.985f;
		Stretch *= 0.985f;
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Texture2D glowTexture = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextGlow").Value;
		Vector2 origin = glowTexture.Size() / 2f;

		Vector2 drawPos = Position - Main.screenPosition;
		Color c = DrawColor * Opacity;

		// Wide glow bar + tighter core, gives a "blade tear" look.
		spriteBatch.Draw(glowTexture, drawPos, null, c * 0.55f, Rotation, origin, new Vector2(Scale.X * Stretch, Scale.Y * 0.55f), SpriteEffects.None, 0f);
		spriteBatch.Draw(glowTexture, drawPos, null, c * 0.9f, Rotation, origin, new Vector2(Scale.X * (Stretch * 0.65f), Scale.Y * 0.22f), SpriteEffects.None, 0f);
	}
}
