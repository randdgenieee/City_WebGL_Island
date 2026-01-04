Shader "UI/Clip Silhouette"
{
    Properties
    {
      [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
      _StencilComp("Stencil Comparison", float) = 8
      _Stencil("Stencil ID", float) = 0
      _StencilOp("Stencil Operation", float) = 0
      _StencilWriteMask("Stencil Write Mask", float) = 255
      _StencilReadMask("Stencil Read Mask", float) = 255
      _ColorMask("Color Mask", float) = 15
      [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", float) = 0
    }
        SubShader
      {
        Tags
        {
          "CanUseSpriteAtlas" = "true"
          "IGNOREPROJECTOR" = "true"
          "PreviewType" = "Plane"
          "QUEUE" = "Transparent"
          "RenderType" = "Transparent"
        }
        Pass // ind: 1, name: Default
        {
          Name "Default"
          Tags
          {
            "CanUseSpriteAtlas" = "true"
            "IGNOREPROJECTOR" = "true"
            "PreviewType" = "Plane"
            "QUEUE" = "Transparent"
            "RenderType" = "Transparent"
          }
          ZWrite Off
          Cull Off
          Stencil
          {
            Ref 0
            ReadMask 0
            WriteMask 0
            Pass Keep
            Fail Keep
            ZFail Keep
            PassFront Keep
            FailFront Keep
            ZFailFront Keep
            PassBack Keep
            FailBack Keep
            ZFailBack Keep
          }
          Blend SrcAlpha OneMinusSrcAlpha
          ColorMask 0
          // m_ProgramMask = 6
          CGPROGRAM
          //#pragma target 4.0

          #pragma vertex vert
          #pragma fragment frag

          #include "UnityCG.cginc"


          #define CODE_BLOCK_VERTEX
          //uniform float4x4 unity_ObjectToWorld;
          //uniform float4x4 unity_MatrixVP;
          uniform float4 _ClipRect;
          uniform sampler2D _MainTex;
          struct appdata_t
          {
              float4 vertex :POSITION0;
              float4 color :COLOR0;
              float2 texcoord :TEXCOORD0;
          };

          struct OUT_Data_Vert
          {
              float4 color :COLOR0;
              float2 texcoord :TEXCOORD0;
              float4 texcoord1 :TEXCOORD1;
              float4 vertex :SV_POSITION;
          };

          struct v2f
          {
              float4 color :COLOR0;
              float2 texcoord :TEXCOORD0;
              float4 texcoord1 :TEXCOORD1;
          };

          struct OUT_Data_Frag
          {
              float4 color :SV_Target0;
          };

          float4 u_xlat0;
          float4 u_xlat1;
          OUT_Data_Vert vert(appdata_t in_v)
          {
              OUT_Data_Vert out_v;
              out_v.vertex = UnityObjectToClipPos(in_v.vertex);
              out_v.color = in_v.color;
              out_v.texcoord.xy = in_v.texcoord.xy;
              out_v.texcoord1 = in_v.vertex;
              return out_v;
          }

          #define CODE_BLOCK_FRAGMENT
          float4 u_xlat0_d;
          float4 u_xlatb0;
          float u_xlat16_1;
          float u_xlat10_1;
          OUT_Data_Frag frag(v2f in_f)
          {
              OUT_Data_Frag out_f;
              u_xlatb0.xy = bool4(in_f.texcoord1.xyxx >= _ClipRect.xyxx).xy;
              u_xlatb0.zw = bool4(_ClipRect.zzzw >= in_f.texcoord1.xxxy).zw;
              u_xlat0_d = lerp(float4(0, 0, 0, 0), float4(1, 1, 1, 1), float4(u_xlatb0));
              u_xlat0_d.xy = (u_xlat0_d.zw * u_xlat0_d.xy);
              u_xlat0_d.x = (u_xlat0_d.y * u_xlat0_d.x);
              u_xlat10_1 = tex2D(_MainTex, in_f.texcoord.xy).w;
              u_xlat16_1 = (u_xlat10_1 * in_f.color.w);
              u_xlat0_d.x = (u_xlat0_d.x * u_xlat16_1);
              out_f.color.w = u_xlat0_d.x;
              out_f.color.xyz = in_f.color.xyz;
              return out_f;
          }


          ENDCG

        } // end phase
      }
          FallBack Off
}
