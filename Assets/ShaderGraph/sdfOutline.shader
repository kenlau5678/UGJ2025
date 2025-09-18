Shader "Unlit/SDFSpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite (Color)", 2D) = "white" {}
        _MainTex2 ("Sprite (SDF)", 2D) = "white" {}
        _Color ("Sprite Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Thickness ("Outline Thickness (0..0.5)", Range(0,0.5)) = 0.03
        _Feather ("Feather (softness)", Range(0.0,0.1)) = 0.01
        _OutlineOpacity ("Outline Opacity", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                fixed4 col : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MainTex2;
            float4 _MainTex2_ST;
            fixed4 _Color;

            fixed4 _OutlineColor;
            float _Thickness;
            float _Feather;
            float _OutlineOpacity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.col = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Ö÷ sprite ÑÕÉ«
                fixed4 baseCol = tex2D(_MainTex, i.uv) * i.col;

                // SDF alpha
                float sdf = tex2D(_MainTex2, i.uv).a;
                float dist = sdf - 0.5;

                float f = max(_Feather, 0.0001);
                float inside = smoothstep(-f, f, dist);

                float t = _Thickness;
                float outerLo = t - f;
                float outerHi = t + f;
                float outer = smoothstep(outerLo, outerHi, dist);

                float outlineMask = saturate(outer * (1.0 - inside));

                fixed4 outlineCol = _OutlineColor;
                outlineCol.a *= _OutlineOpacity * outlineMask;

                float finalAlpha = max(baseCol.a * inside, outlineCol.a);
                float3 rgb = lerp(baseCol.rgb * inside, outlineCol.rgb, outlineCol.a);

                return fixed4(rgb, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
