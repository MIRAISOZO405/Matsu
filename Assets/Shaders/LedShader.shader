Shader "Unlit/LedShader"
{
    Properties
    {
        _EmissionColor("Emission Color", Color) = (0,0,0,1)
        _EmissionMap("Emission Map", 2D) = "white" {}
        _MainTex("Texture", 2D) = "white" {}
        _PixShape("Pixel Shape Texture", 2D) = "white" {}
        _UV_X("Pixel num x", Range(10,1600)) = 960
        _UV_Y("Pixel num y", Range(10,1600)) = 360
        _Intensity("intensity", float) = 1

            _ColorTemperature("Color Temperature", Range(-0.5, 0.5)) = 0.0
    _Contrast("Contrast", Range(0.5, 2.0)) = 1.0
    _Brightness("Brightness", Range(-0.5, 0.5)) = 0.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex, _PixShape, _EmissionMap;
                float4 _MainTex_ST, _PixShape_ST, _EmissionColor;
                float _UV_X, _UV_Y, _Intensity;

                float _ColorTemperature;
                float _Contrast;
                float _Brightness;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 AdjustColorTemperature(fixed4 color, float temperature)
                {
                    color.r += temperature;
                    color.b -= temperature;
                    return color;
                }

                fixed4 AdjustContrastAndBrightness(fixed4 color, float contrast, float brightness)
                {
                    color.rgb = ((color.rgb - 0.5) * max(contrast, 0)) + 0.5;
                    color.rgb += brightness;
                    return color;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 縦横何個並べるか
                    float2 uv_res = float2(_UV_X, _UV_Y);
                    fixed4 col = tex2D(_MainTex, (floor(i.uv * uv_res) / uv_res + (1 / (uv_res * 2))));

                    // 画素
                    float2 uv = i.uv * uv_res;
                    float4 pix = tex2D(_PixShape, uv);

                    // 発光テクスチャのサンプリング
                    fixed4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, col);

                    // 発光の適用
                    col.rgb += emission.rgb * pix.a;

                    // 色温度とコントラスト・明るさの調整
                    col = AdjustColorTemperature(col, _ColorTemperature);
                    col = AdjustContrastAndBrightness(col, _Contrast, _Brightness);

                    // 発光の適用
                    col.rgb += emission.rgb * pix.a;

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, col);

                    return col * pix * _Intensity;
                }
                ENDCG
            }
        }
}

