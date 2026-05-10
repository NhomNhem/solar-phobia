Shader "SolarPhobia/URP_2D_HeatDistortionWall"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint Color", Color) = (1,1,1,1)

        [Header(Distortion Settings)]
        [NoScaleOffset] _NoiseTex ("Noise Texture (R)", 2D) = "gray" {}
        _NoiseTiling ("Noise Tiling (XY) Offset (ZW)", Vector) = (1, 1, 0, 0)
        _DistortSpeed ("Distort Speed", Range(0.0, 5.0)) = 1.0
        _DistortAmount ("Distort Amount", Range(0.0, 0.1)) = 0.02
        _HeatWaveFrequency ("Heat Wave Frequency", Range(1.0, 50.0)) = 10.0 // Thêm tần số sóng nhiệt

        [Header(Heat Edge)]
        [HDR] _EdgeColor ("Edge Color", Color) = (2, 0.5, 0, 1) // Màu Cam Cháy rực rỡ
        _EdgeWidth ("Edge Width", Range(0.0, 0.5)) = 0.1
        _EdgeIntensity ("Edge Intensity", Range(0.0, 5.0)) = 2.0
        _EdgeBurnFactor ("Edge Burn Factor", Range(0.0, 1.0)) = 0.5 // Thêm hệ số làm xém viền

        [Header(Direction)]
        // 1 cho bức tường bên Trái tiến sang Phải, -1 cho tường bên Phải tiến sang Trái
        _WallDirection ("Wall Direction (1: Left->Right, -1: Right->Left)", Float) = 1.0 
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "RenderPipeline"="UniversalPipeline"
            // Bắt buộc phải vẽ sau khi background và các sprite khác đã vẽ xong
            "DisableBatching"="True" 
        }

        // Tắt culling và depth write vì đây là hiệu ứng 2D đè lên màn hình
        Cull Off
        Lighting Off
        ZWrite Off
        // Sử dụng Alpha Blending tiêu chuẩn
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl" // Quan trọng để lấy ảnh nền

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
                // screenPosition dùng để lấy tọa độ màn hình chuẩn xác khi bóp méo
                float4 screenPosition : TEXCOORD1; 
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _NoiseTiling;
                float _DistortSpeed;
                float _DistortAmount;
                float _HeatWaveFrequency;
                float4 _EdgeColor;
                float _EdgeWidth;
                float _EdgeIntensity;
                float _EdgeBurnFactor;
                float _WallDirection;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                
                // Tính toán tọa độ màn hình (Screen Space UV)
                OUT.screenPosition = ComputeScreenPos(OUT.positionHCS);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Chuẩn hóa tọa độ màn hình
                float2 screenUV = IN.screenPosition.xy / IN.screenPosition.w;

                // 1. Tính toán sóng nhiệt (Heat Distortion) với Sóng Sine
                float timeTime = _Time.y * _DistortSpeed;
                float2 noiseUV = IN.uv * _NoiseTiling.xy + _NoiseTiling.zw;
                noiseUV.y -= timeTime; // Cuộn nhiễu từ dưới lên để giống hơi nóng bốc lên
                
                // Thêm một lớp bóp méo hình sin để mô phỏng "Heat Haze" (bức xạ nhiệt)
                float heatHaze = sin(IN.uv.y * _HeatWaveFrequency + _Time.y * _DistortSpeed * 2.0) * (_DistortAmount * 0.5);
                
                // Lấy giá trị Noise
                float noiseVal = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;
                
                // Điều chỉnh lại Noise từ [0,1] thành [-1,1] để méo đều hai bên
                float2 offset = (noiseVal * 2.0 - 1.0) * _DistortAmount;
                offset.x += heatHaze; // Áp dụng độ méo sin vào trục X
                
                // Cộng offset vào screenUV để bóp méo hình ảnh đằng sau
                float2 distortedScreenUV = screenUV + offset;

                // 2. Lấy hình ảnh của game hiện tại (Bị bóp méo)
                // LƯU Ý: Phải bật "Opaque Texture" trong file URP Asset thì hàm này mới chạy!
                half3 sceneColor = SampleSceneColor(distortedScreenUV);

                // 3. Tính toán Viền Sáng Rực Rỡ (Edge Glow)
                // Dùng trục X của UV gốc (0 đến 1)
                // _WallDirection = 1 (Tường trái): UV.x từ 1 giảm về 0 ở mép phải
                // _WallDirection = -1 (Tường phải): UV.x từ 0 tăng lên 1 ở mép trái
                float edgeGradient = _WallDirection > 0 ? (1.0 - IN.uv.x) : IN.uv.x;
                
                // Tạo viền sắc nét ở mép
                float edgeMask = smoothstep(0.0, _EdgeWidth, edgeGradient);
                
                // Áp dụng "Edge Burn Factor" để làm tối hình ảnh nền trước khi cộng sáng
                // Điều này làm cho màu cam cháy có vẻ thiêu đốt màu thật của ảnh
                sceneColor = lerp(sceneColor, sceneColor * (1.0 - _EdgeBurnFactor), edgeMask);
                
                // 4. Trộn kết quả
                // Vùng bên trong: Chỉ hiện hình ảnh méo mó của scene.
                // Vùng viền mép: Cộng thêm màu Cam Cháy chói lóa.
                // Tính toán phần RGB riêng biệt với half3
                half3 finalRGB = sceneColor + (_EdgeColor.rgb * edgeMask * _EdgeIntensity);

                // Lấy Alpha của Sprite gốc
                half spriteAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv).a;
                
                // Gộp lại thành half4 để trả về
                half4 finalColor = half4(finalRGB, spriteAlpha * IN.color.a);

                return finalColor;
            }
            ENDHLSL
        }
    }
}