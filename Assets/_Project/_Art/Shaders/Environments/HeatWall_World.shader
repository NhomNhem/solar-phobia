Shader "SolarPhobia/Environment/HeatWall_World"
{
    Properties
    {
        _MainTex ("Distortion Map", 2D) = "white" {}
        [HDR] _Color ("Base Color", Color) = (1, 0.2, 0, 1)
        _NoiseScale ("Noise Scale", Float) = 5.0
        _NoiseSpeed ("Noise Speed", Float) = 1.0
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.5
        _EdgeIntensity ("Edge Intensity", Range(0, 1)) = 0.3
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "Unlit"
            Blend SrcAlpha One
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
                float3 positionWS : TEXCOORD1;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _NoiseScale;
                float _NoiseSpeed;
                float _DistortionStrength;
                float _EdgeIntensity;
            CBUFFER_END

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.positionWS = worldPos;
                output.positionCS = TransformWorldToHClip(worldPos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float pressure = saturate(_SP_DayPressure01);

                float2 noiseUV = input.positionWS.xy * _NoiseScale + _Time.y * _NoiseSpeed;
                float noise = hash(noiseUV);
                float2 distortionOffset = (noise - 0.5) * _DistortionStrength * pressure;

                float2 uv = input.uv + distortionOffset;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;

                float2 centerUV = input.uv * 2.0 - 1.0;
                float edgeMask = saturate(length(centerUV) * _EdgeIntensity);
                col.rgb += _Color.rgb * edgeMask * pressure;

                col.a *= pressure;

                return col;
            }
            ENDHLSL
        }
    }
}
