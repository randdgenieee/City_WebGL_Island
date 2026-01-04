Shader "Engine/GridOverlay"
{
    Properties
    {
      _MainTex("Tile Texture", 2D) = "white" {}
      _ColorsTex("Colors Texture", 2D) = "white" {}
      _GridU("Grid Size U", float) = 0
      _GridV("Grid Size V", float) = 0
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
          Fog
          {
            Mode  Off
          }
          Blend SrcAlpha OneMinusSrcAlpha
          // m_ProgramMask = 6
          CGPROGRAM
          //#pragma target 4.0

          #pragma vertex vert
          #pragma fragment frag

          #include "UnityCG.cginc"


          #define CODE_BLOCK_VERTEX
          //uniform float4x4 unity_ObjectToWorld;
          //uniform float4x4 unity_MatrixVP;
          uniform float _GridU;
          uniform float _GridV;
          uniform sampler2D _MainTex;
          uniform sampler2D _ColorsTex;
          struct appdata_t
          {
              float4 vertex :POSITION0;
              float2 texcoord :TEXCOORD0;
          };

          struct OUT_Data_Vert
          {
              float2 texcoord :TEXCOORD0;
              float4 vertex :SV_POSITION;
          };

          struct v2f
          {
              float2 texcoord :TEXCOORD0;
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
              return out_v;
          }

          #define CODE_BLOCK_FRAGMENT
          float2 u_xlat0_d;
          float4 u_xlat10_0;
          float4 u_xlat10_1;
          float2 u_xlat2;
          float u_xlat3;
          float2 u_xlat6;
          OUT_Data_Frag frag(v2f in_f)
          {
              OUT_Data_Frag out_f;
              u_xlat0_d.xy = (in_f.texcoord.xy * float2(_GridU, _GridV));
              u_xlat0_d.xy = floor(u_xlat0_d.xy);
              u_xlat6.xy = ((in_f.texcoord.xy * float2(_GridU, _GridV)) + (-u_xlat0_d.xy));
              u_xlat0_d.xy = (u_xlat0_d.xy + float2(0.5, 0.5));
              u_xlat0_d.xy = (u_xlat0_d.xy / float2(_GridU, _GridV));
              u_xlat10_1 = tex2D(_ColorsTex, u_xlat0_d.xy);
              u_xlat0_d.x = ((-u_xlat6.y) + u_xlat6.x);
              u_xlat3 = (u_xlat6.y + u_xlat6.x);
              u_xlat2.y = (u_xlat3 * 0.5);
              u_xlat2.x = ((u_xlat0_d.x * 0.5) + 0.5);
              u_xlat10_0 = tex2D(_MainTex, u_xlat2.xy);
              out_f.color = (u_xlat10_1 * u_xlat10_0);
              return out_f;
          }


          ENDCG

        } // end phase
      }
          FallBack Off
}
