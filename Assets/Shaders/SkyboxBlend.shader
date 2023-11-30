Shader "Custom/PanoramaSkyboxBlend"
{
    Properties
    {
        _MainTex("Current Skybox (Panorama)", 2D) = "white" {}
        _NextTex("Next Skybox (Panorama)", 2D) = "white" {}
        _Blend("Blend Factor", Range(0, 1)) = 0.5
    }

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                };

                struct v2f
                {
                    float3 texcoord : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _NextTex;
                float _Blend;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = v.vertex.xyz;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half3 viewDir = normalize(i.texcoord);
                    half2 uv = float2(atan2(viewDir.z, viewDir.x) / (2.0 * UNITY_PI) + 0.5, viewDir.y * 0.5 + 0.5);
                    half4 col1 = tex2D(_MainTex, uv);
                    half4 col2 = tex2D(_NextTex, uv);
                    return lerp(col1, col2, _Blend);
                }
                ENDCG
            }
        }
}
