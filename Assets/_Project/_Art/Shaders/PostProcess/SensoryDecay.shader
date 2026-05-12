Shader "SolarPhobia/PostProcess/SensoryDecay"
{
    Properties
    {
        [HideInInspector] _MainTex ("Source", 2D) = "white" {}
        _AberrationIntensity ("Aberration Max Intensity", Range(0, 0.1)) = 0.02
        _StaticIntensity ("Static Max Intensity", Range(0, 0.2)) = 0.05
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "SensoryDecay"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/_Project/_Art/Shaders/SolarPhobiaGlobals.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float _AberrationIntensity;
                float _StaticIntensity;
            CBUFFER_END

            // Simple hash for noise
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float decay = _SP_SensoryDecay01;
                
                // 1. Chromatic Aberration
                // Offset increases towards the edges and with decay
                float2 distFromCenter = input.uv - 0.5;
                float2 offset = distFromCenter * _AberrationIntensity * decay;
                
                half r = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv - offset).r;
                half g = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).g;
                half b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv + offset).b;
                
                half4 col = half4(r, g, b, 1.0);
                
                // 2. Static Noise
                float n = hash(input.uv + _Time.y);
                float noiseAmt = _StaticIntensity * decay;
                col.rgb = lerp(col.rgb, half3(n, n, n), noiseAmt);
                
                // 3. Subtle Desaturation as decay increases
                half gray = dot(col.rgb, half3(0.299, 0.587, 0.114));
                col.rgb = lerp(col.rgb, half3(gray, gray, gray), decay * 0.3);
                
                return col;
            }
            ENDHLSL
        }
    }
}
