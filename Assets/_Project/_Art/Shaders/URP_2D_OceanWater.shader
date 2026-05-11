Shader "SolarPhobia/URP_2D_OceanWater_BatcherSafe"
{
    Properties
    {
        // Thêm [NoScaleOffset] để chặn Unity tự tạo biến _ST và _TexelSize
        [HideInInspector] [NoScaleOffset] _MainTex ("Sprite Texture", 2D) = "white" {}
        _WaterColor ("Water Base Color", Color) = (0.1, 0.3, 0.4, 1)
        _FoamColor ("Foam Color", Color) = (0.8, 0.9, 1, 1)
        
        [Header(Wave Settings)]
        [NoScaleOffset] _NoiseTex ("Water Noise (R)", 2D) = "gray" {}
        
        // Biến tự chế để chỉnh Tiling thay thế cho _NoiseTex_ST mặc định
        _NoiseTiling ("Noise Tiling (XY) Offset (ZW)", Vector) = (1, 1, 0, 0) 
        
        _WaveSpeed ("Wave Speed", Range(0, 5)) = 1.0
        _WaveIntensity ("Wave Intensity (Calm to Storm)", Range(0, 0.5)) = 0.05
        _WaveFrequency ("Wave Frequency", Range(1, 50)) = 20.0
        
        [Header(Foam Settings)]
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.7
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
        }

        Cull Off Lighting Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
                float worldX        : TEXCOORD1;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _WaterColor;
                float4 _FoamColor;
                float4 _NoiseTiling; // Sử dụng biến Vector tùy chỉnh ở đây
                float _WaveSpeed;
                float _WaveIntensity;
                float _WaveFrequency;
                float _FoamThreshold;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                
                // Lấy tọa độ X thế giới để sóng cuộn liên tục
                OUT.worldX = TransformObjectToWorld(IN.positionOS.xyz).x;
                
                return OUT;
            }

            vector<float, 3> frag(Varyings IN) : SV_Target
            {
                // 1. Tạo hiệu ứng nhấp nhô (Sine Wave Displacement)
                float timeTime = _Time.y * _WaveSpeed;
                float waveDistortion = sin(IN.uv.y * _WaveFrequency + timeTime + IN.worldX) * _WaveIntensity;
                
                float2 distortedUV = IN.uv;
                distortedUV.x += waveDistortion;

                // 2. Chạy Noise Texture bằng biến _NoiseTiling tự chế
                float2 noiseUV = distortedUV * _NoiseTiling.xy + _NoiseTiling.zw;
                noiseUV.x -= timeTime * 0.5; // Cuộn noise theo phương ngang
                
                half noiseVal = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

                // 3. Phân tách màu Nước và màu Bọt sóng
                half isFoam = step(_FoamThreshold, noiseVal + waveDistortion);
                half4 finalColor = lerp(_WaterColor, _FoamColor, isFoam);

                // Áp dụng Alpha của Sprite gốc
                half spriteAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                finalColor.a = spriteAlpha * IN.color.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}