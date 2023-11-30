Shader "Custom/FadeShader"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Fade("Fade", Range(0,1)) = 0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" }
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _Fade;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    half4 col = tex2D(_MainTex, i.uv);
                    col.a *= _Fade;
                    return col;
                }
                ENDCG
            }
        }
}
