Shader "Innovattic/Sprite/Simple Greyscale"
{
    Properties
    {
      [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
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
      Pass // ind: 1, name: 
      {
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
        uniform sampler2D _MainTex;
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
        float4 u_xlat0_d;
        OUT_Data_Frag frag(v2f in_f)
        {
            OUT_Data_Frag out_f;
            u_xlat0_d = tex2D(_MainTex, in_f.texcoord.xy).xwyz;
            u_xlat0_d.x = dot(u_xlat0_d.xzw, float3(0.209999993, 0.720000029, 0.0700000003));
            out_f.color = u_xlat0_d.xxxy;
            return out_f;
        }


        ENDCG

      } // end phase
    }
        FallBack Off
}
