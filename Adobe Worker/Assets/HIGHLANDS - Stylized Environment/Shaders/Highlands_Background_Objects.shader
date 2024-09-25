// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Highlands Background Objects"
{
	Properties
	{
		[Header(Maps)][Space(5)]_Albedo("Albedo", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		[Header(Settings)][Space(5)]_CustomCutoff("Alpha Cutoff", Range( 0 , 1)) = 0
		[HDR]_Color("Color", Color) = (0.8,0.8,0.8,0)
		_NormalScale("Normal Scale", Float) = 1
		[Header(Fog Settings)][Space(5)][Toggle(_FOGON_ON)] _FogOn("Fog On", Float) = 1
		_FogColor("Fog Color", Color) = (1,1,1,0)
		_Fade("Fade", Float) = 1
		_Height("Height", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _FOGON_ON
		#pragma surface surf Standard keepalpha noshadow exclude_path:deferred nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalScale;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Color;
		uniform float4 _FogColor;
		uniform float _Height;
		uniform float _Fade;
		uniform float _CustomCutoff;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo );
			o.Albedo = ( tex2DNode2 * _Color ).rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#ifdef _FOGON_ON
				float4 staticSwitch35 = ( _FogColor * saturate( ( ( ( ase_vertex3Pos.y * 0.1 ) + _Height ) * _Fade ) ) );
			#else
				float4 staticSwitch35 = float4( 0,0,0,0 );
			#endif
			o.Emission = staticSwitch35.rgb;
			o.Alpha = 1;
			clip( tex2DNode2.a - _CustomCutoff );
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.RangedFloatNode;34;-862.2328,73.42168;Inherit;False;Property;_NormalScale;Normal Scale;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;26;-1815.643,392.2545;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;33;-1614.996,441.2723;Inherit;False;0.1;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1603.996,551.2723;Inherit;False;Property;_Height;Height;8;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-1400.996,487.2723;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1406.996,636.2722;Inherit;False;Property;_Fade;Fade;7;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1177.994,545.2723;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;30;-996.2874,545.4656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-758.6368,428.9618;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-498.1984,-340.2841;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-101.3884,7.333805;Float;False;True;-1;2;;0;0;Standard;Raygeas/Highlands Background Objects;False;False;False;False;False;False;True;True;True;True;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;False;0;True;Opaque;;AlphaTest;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;False;0;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;True;_CustomCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.SamplerNode;3;-658.2183,25.97813;Inherit;True;Property;_Normal;Normal;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;35;-550.2927,400.484;Inherit;False;Property;_FogOn;Fog On;5;0;Create;True;0;0;0;False;2;Header(Fog Settings);Space(5);False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-120.1099,476.4875;Inherit;False;Property;_CustomCutoff;Alpha Cutoff;2;0;Create;False;0;0;0;False;2;Header(Settings);Space(5);False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;38;-815.8279,-235.3334;Inherit;False;Property;_Color;Color;3;1;[HDR];Create;True;0;0;0;False;0;False;0.8,0.8,0.8,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;-1062.751,328.064;Inherit;False;Property;_FogColor;Fog Color;6;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-902.2438,-454.4573;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;2;Header(Maps);Space(5);False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;33;0;26;2
WireConnection;31;0;33;0
WireConnection;31;1;32;0
WireConnection;27;0;31;0
WireConnection;27;1;28;0
WireConnection;30;0;27;0
WireConnection;29;0;11;0
WireConnection;29;1;30;0
WireConnection;37;0;2;0
WireConnection;37;1;38;0
WireConnection;0;0;37;0
WireConnection;0;1;3;0
WireConnection;0;2;35;0
WireConnection;0;10;2;4
WireConnection;3;5;34;0
WireConnection;35;0;29;0
ASEEND*/
//CHKSM=437EA63273977B6D74D216C0FA14756F1C2F71AE