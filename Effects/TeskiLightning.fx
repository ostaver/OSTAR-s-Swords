sampler uImage0 : register(s0);

float uTime;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float time = uTime * 7.0;

    // --- Jagged displacement (big stepped sine for sharp zig-zags) ---
    float seg = floor(coords.x * 10.0 + time * 0.3);
    float offset = sin(seg * 83.7) * 0.32;

    // Fade at bolt endpoints
    float edgeMask = saturate(coords.x * 12.0) * saturate((1.0 - coords.x) * 12.0);
    offset *= edgeMask;

    // --- Main bolt ---
    float cy = coords.y - 0.5;
    float dist = abs(cy - offset);
    float core = 0.004 / (dist * dist + 0.00015);
    float glow = 0.02 / (dist + 0.008);

    // --- Secondary tendril (runs alongside main bolt) ---
    float tOff = offset + sin(seg * 217.5) * 0.09 + 0.06;
    float tDist = abs(cy - tOff);
    float tendril = 0.005 / (tDist + 0.008);

    // --- Compose ---
    float3 col = core * float3(1.0, 1.0, 1.0);
    col += (glow + tendril * 0.4) * sampleColor.rgb * 2.0;
    col *= edgeMask;

    return float4(col, 1.0) * sampleColor.a;
}

technique Technique1
{
    pass LightningPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
