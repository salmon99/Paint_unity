Shader "Paint/PaintTraceTexture"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _PaintTex ("Painted Texture", 2D) = "black" {}
        _TrackTex ("Tracking Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _PaintTex;
        sampler2D _TrackTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 main = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 painted = tex2D (_PaintTex, IN.uv_MainTex);
            fixed4 track = tex2D(_TrackTex, IN.uv_MainTex);

            fixed alphaBlend = saturate(painted.a + main.a * (1 - painted.a));
            fixed4 blendResult = lerp(main, painted, painted.a / alphaBlend);
            
            o.Albedo = blendResult.rgb;
            o.Alpha = alphaBlend;
        }
        ENDCG
    }
    FallBack "Diffuse"
}