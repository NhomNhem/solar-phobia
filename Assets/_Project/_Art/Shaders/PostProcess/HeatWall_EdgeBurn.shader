Shader "SolarPhobia/PostProcess/HeatWall_EdgeBurn"
{
    Properties
    {
        [HideInInspector] _MainTex ("Source", 2D) = "white" {}
        _BurnColor ("Burn Color", Color) = (1, 0.3, 0, 0.5)
        _BurnEdge ("Burn Edge Start", Range(0, 1)) = 0.4
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "HeatWallEdgeBurn"
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
                float4 _BurnColor;
                float _BurnEdge;
            CBUFFER_END

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            half4 frag (Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Calculate distance from center (vignette effect)
                float2 centerUV = input.uv * 2.0 - 1.0;
                float dist = length(centerUV);
                
                // Edge burn intensity driven by global DayPressure
                float edgeMask = smoothstep(_BurnEdge, 1.0, dist);
                float burnIntensity = edgeMask * _SP_DayPressure01;
                
                // Mix in the burn color
                col.rgb = lerp(col.rgb, _BurnColor.rgb, burnIntensity * _BurnColor.a);
                
                // Optional: Add a subtle flicker or noise to the burn
                float flicker = hash(float2(_Time.y, _Time.y)); // Use a hash for flicker
                col.rgb += _BurnColor.rgb * burnIntensity * flicker * 0.1;
                
                return col;
            }
            
            ENDHLSL
        }
    }
}
