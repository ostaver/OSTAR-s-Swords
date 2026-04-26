using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OSTARsSWORDS.Content.Projectiles;

/// <summary>
/// A chaotic lightning bolt that strikes from the sky toward a target position.
/// Procedurally generates a jagged bolt with branches for a dramatic effect.
/// Uses multi-pass additive sprite rendering (no shader) for maximum visual quality.
/// </summary>
public class TeskiLightning : ModProjectile
{
    private List<Vector2> boltPoints;
    private List<List<Vector2>> branches;
    private float lifeProgress = 0f;
    private float maxLife = 20f;
    private bool initialized = false;
    private Color boltColor;
    private Vector2 impactPos;

    // ai[0] = target X, ai[1] = target Y
    public Vector2 TargetPos => new(Projectile.ai[0], Projectile.ai[1]);

    public override string Texture => "OSTARsSWORDS/Content/Projectiles/InvisibleProj";

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 22;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.damage = 0;
    }

    public override bool? CanDamage() => false;

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        impactPos = TargetPos;

        // Lightning origin: WAY up in the sky — 900 to 1400 pixels above, offset sideways
        float skyOffsetX = Main.rand.NextFloat(-350f, 350f);
        float skyOffsetY = Main.rand.NextFloat(-1400f, -900f);
        Vector2 start = impactPos + new Vector2(skyOffsetX, skyOffsetY);
        Projectile.Center = start;

        // Generate the main bolt with macro points (less subdivisions because shader handles jitter)
        boltPoints = GenerateBolt(start, impactPos, 2, 80f);

        // Generate 3-5 branches off the main bolt for chaos
        branches = new List<List<Vector2>>();
        int branchCount = Main.rand.Next(3, 6);
        for (int i = 0; i < branchCount; i++)
        {
            int branchStart = Main.rand.Next(1, boltPoints.Count - 1);
            Vector2 branchDir = new Vector2(
                Main.rand.NextFloat(-180f, 180f),
                Main.rand.NextFloat(50f, 220f));
            branches.Add(GenerateBolt(boltPoints[branchStart], boltPoints[branchStart] + branchDir, 1, 40f));
        }

        // Icy blue/white/cyan color palette
        int colorChoice = Main.rand.Next(4);
        boltColor = colorChoice switch
        {
            0 => Color.Lerp(Color.White, Color.CornflowerBlue, 0.3f),
            1 => Color.Lerp(Color.LightCyan, Color.DeepSkyBlue, 0.4f),
            2 => Color.Lerp(Color.White, Color.LightBlue, 0.2f),
            _ => Color.Lerp(Color.AliceBlue, Color.CornflowerBlue, 0.5f)
        };

        SpawnImpactEffects(impactPos);
    }

    private List<Vector2> GenerateBolt(Vector2 start, Vector2 end, int subdivisions, float jitter)
    {
        var points = new List<Vector2> { start, end };

        for (int sub = 0; sub < subdivisions; sub++)
        {
            var newPoints = new List<Vector2> { points[0] };
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 mid = (points[i] + points[i + 1]) / 2f;
                Vector2 perpendicular = points[i + 1] - points[i];
                perpendicular = new Vector2(-perpendicular.Y, perpendicular.X);
                if (perpendicular != Vector2.Zero)
                    perpendicular.Normalize();
                mid += perpendicular * Main.rand.NextFloat(-jitter, jitter);
                newPoints.Add(mid);
                newPoints.Add(points[i + 1]);
            }
            points = newPoints;
            jitter *= 0.52f;
        }

        return points;
    }

    private void SpawnImpactEffects(Vector2 pos)
    {
        // Big burst of electric sparks
        for (int i = 0; i < 18; i++)
        {
            Vector2 vel = new Vector2(Main.rand.NextFloat(3f, 12f), 0).RotatedByRandom(MathHelper.TwoPi);
            Dust spark = Dust.NewDustPerfect(pos, DustID.Electric, vel);
            spark.scale = Main.rand.NextFloat(0.7f, 1.5f);
            spark.noGravity = true;
        }

        // Icy glow particles
        for (int i = 0; i < 10; i++)
        {
            Vector2 vel = new Vector2(Main.rand.NextFloat(2f, 8f), 0).RotatedByRandom(MathHelper.TwoPi);
            Dust glow = Dust.NewDustPerfect(pos, DustID.RainbowMk2, vel);
            glow.scale = Main.rand.NextFloat(0.6f, 1.3f);
            glow.noGravity = true;
            glow.color = Main.rand.NextBool(3) ? Color.White : Color.DeepSkyBlue;
        }

        // Ground smoke
        for (int i = 0; i < 6; i++)
        {
            Dust smoke = Dust.NewDustPerfect(pos + new Vector2(Main.rand.NextFloat(-25, 25), 0),
            DustID.Smoke, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-4f, -1f)));
            smoke.scale = Main.rand.NextFloat(0.9f, 1.6f);
            smoke.color = Color.LightBlue * 0.5f;
        }
    }

    public override void AI()
    {
        Initialize();

        lifeProgress++;
        if (lifeProgress >= maxLife)
            Projectile.Kill();

        // Dynamic lighting along bolt segments
        if (boltPoints != null && boltPoints.Count > 2)
        {
            float lightIntensity = MathHelper.Clamp(1f - lifeProgress / maxLife, 0f, 1f);

            // Light every ~10th point along the bolt for strong illumination
            for (int i = 0; i < boltPoints.Count; i += 10)
            {
                Vector3 lightCol = new Vector3(0.5f, 0.75f, 1.2f) * lightIntensity * 2.5f;
                Lighting.AddLight(boltPoints[i], lightCol);
            }

            // Extra bright light at the impact point
            Lighting.AddLight(impactPos, new Vector3(0.7f, 1f, 1.5f) * lightIntensity * 3f);

            // Ambient sparks along the bolt
            if (Main.rand.NextBool(2))
            {
                int idx = Main.rand.Next(boltPoints.Count);
                Dust ambient = Dust.NewDustPerfect(boltPoints[idx], DustID.Electric,
                Main.rand.NextVector2Circular(2f, 2f));
                ambient.scale = Main.rand.NextFloat(0.3f, 0.8f);
                ambient.noGravity = true;
            }
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (boltPoints == null || boltPoints.Count < 2) return false;

        SpriteBatch sb = Main.spriteBatch;

        // Flash intensity: bright on spawn, fades out
        float alpha = 1f - (lifeProgress / maxLife);
        alpha = MathHelper.Clamp(alpha, 0f, 1f);

        // Initial flash is extra bright for the first few frames
        float flashMult = lifeProgress < 4 ? 1.6f : 1f;
        alpha *= flashMult;

        // --- Switch to immediate blending for Shader ---
        sb.End();
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        // Load and apply the lightning shader
        Effect lightningShader = ModContent.Request<Effect>("OSTARsSWORDS/Effects/TeskiLightning", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        lightningShader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

        // Draw bolt bloom glow orbs along the main bolt
        Texture2D glowTex = ModContent.Request<Texture2D>("OSTARsSWORDS/ExtraTextures/UI/CrystalTextGlow").Value;
        Vector2 glowOrigin = glowTex.Size() / 2f;

        // Apply shader before rendering bolts
        lightningShader.CurrentTechnique.Passes[0].Apply();

        DrawShaderBolt(sb, boltPoints, alpha, 1f, glowTex, glowOrigin);

        if (branches != null)
        {
            foreach (var branch in branches)
                DrawShaderBolt(sb, branch, alpha * 0.6f, 0.5f, glowTex, glowOrigin);
        }

        // --- Resume standard Additive blending for the impact point ---
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        // Massive multi-layered bloom orb at impact point
        float impactBloom = alpha * (lifeProgress < 5 ? 2.5f : 1.2f);
        Vector2 impactScreen = impactPos - Main.screenPosition;
        // Layer 1: huge soft colored halo
        sb.Draw(glowTex, impactScreen, null, boltColor * impactBloom * 0.5f, 0f,
            glowOrigin, 3.5f * impactBloom, SpriteEffects.None, 0f);
        // Layer 2: medium bright colored bloom
        sb.Draw(glowTex, impactScreen, null, boltColor * impactBloom * 0.8f, 0f,
            glowOrigin, 2f * impactBloom, SpriteEffects.None, 0f);
        // Layer 3: tight white-hot core
        sb.Draw(glowTex, impactScreen, null, Color.White * impactBloom * 0.6f, 0f,
            glowOrigin, 1.2f * impactBloom, SpriteEffects.None, 0f);
        // Layer 4: pulsing ring (rotated glow)
        float pulse = (float)Math.Sin(lifeProgress * 0.5f) * 0.3f + 1f;
        sb.Draw(glowTex, impactScreen, null, boltColor * impactBloom * 0.3f,
            lifeProgress * 0.2f, glowOrigin, 2.8f * impactBloom * pulse, SpriteEffects.None, 0f);

        // --- Switch back to normal blending ---
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        return false;
    }

    private void DrawShaderBolt(SpriteBatch sb, List<Vector2> points, float alpha, float widthScale,
        Texture2D glowTex, Vector2 glowOrigin)
    {
        Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 start = points[i] - Main.screenPosition;
            Vector2 end = points[i + 1] - Main.screenPosition;
            Vector2 diff = end - start;
            float length = diff.Length();
            float angle = (float)Math.Atan2(diff.Y, diff.X);

            // The shader expects a quad where it can draw the lightning thickness and jitter
            // A higher width covers more jitter area
            float shaderQuadWidth = 90f * widthScale; 
            
            // Draw a single stretched quad. The shader will color it based on UVs and noise
            sb.Draw(pixel,
                start - new Vector2(0, shaderQuadWidth / 2f).RotatedBy(angle),
                new Rectangle(0, 0, 1, 1), // This will be mapped to (0,0) to (1,1) UV space
                boltColor * alpha, // Pass alpha and base color to the shader
                angle,
                Vector2.Zero,
                new Vector2(length, shaderQuadWidth),
                SpriteEffects.None, 0f);

            // Draw bloom orbs at connection points for extra brightness where macro segments meet
            if (i > 0 || points.Count == 2)
            {
                float bloomScale = 1.2f * widthScale * alpha;
                sb.Draw(glowTex, start, null, boltColor * alpha * 0.7f, 0f,
                    glowOrigin, bloomScale, SpriteEffects.None, 0f);
                sb.Draw(glowTex, start, null, Color.White * alpha * 0.3f, 0f,
                    glowOrigin, bloomScale * 0.5f, SpriteEffects.None, 0f);
            }
        }

        // Bloom orb at the very start of the bolt
        if (points.Count > 0)
        {
            Vector2 topScreen = points[0] - Main.screenPosition;
            sb.Draw(glowTex, topScreen, null, boltColor * alpha * 0.6f, 0f,
                glowOrigin, 1.5f * widthScale * alpha, SpriteEffects.None, 0f);
            sb.Draw(glowTex, topScreen, null, Color.White * alpha * 0.3f, 0f,
                glowOrigin, 0.8f * widthScale * alpha, SpriteEffects.None, 0f);
        }
    }
}
