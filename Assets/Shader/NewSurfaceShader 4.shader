Shader "Particles/Additive-Transparent-Tint-Clip"
{
    Properties
    {
      _Color("Main Color", Color) = (1,1,1,0.5)
      _MainTex("Base (RGB)", 2D) = "white" {}
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
          "IGNOREPROJECTOR" = "true"
          "PreviewType" = "Plane"
          "QUEUE" = "Transparent"
          "RenderType" = "Transparent"
        }
        Pass // ind: 1, name: 
        {
          Tags
          {
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
          Blend SrcAlpha One
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
          uniform float4 _Color;
          uniform sampler2D _MainTex;
          struct appdata_t
          {
              float4 vertex :POSITION0;
              float2 texcoord :TEXCOORD0;
              float4 color :COLOR0;
          };

          struct OUT_Data_Vert
          {
              float2 texcoord :TEXCOORD0;
              float4 color :COLOR0;
              float4 vertex :SV_POSITION;
          };

          struct v2f
          {
              float2 texcoord :TEXCOORD0;
              float4 color :COLOR0;
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
              out_v.texcoord.xy = in_v.texcoord.xy;
              u_xlat0 = (in_v.color * _Color);
              out_v.color = u_xlat0;
              return out_v;
          }

          #define CODE_BLOCK_FRAGMENT
          float4 u_xlat16_0;
          float4 u_xlat10_0;
          OUT_Data_Frag frag(v2f in_f)
          {
              OUT_Data_Frag out_f;
              u_xlat10_0 = tex2D(_MainTex, in_f.texcoord.xy);
              u_xlat16_0 = (u_xlat10_0 * in_f.color);
              out_f.color = u_xlat16_0;
              return out_f;
          }


          ENDCG

        } // end phase
      }
          FallBack Off
}
