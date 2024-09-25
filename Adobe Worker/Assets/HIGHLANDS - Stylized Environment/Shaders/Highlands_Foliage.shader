// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Highlands Foliage"
{
	Properties
	{
		[Header(Maps)][Space(5)]_Albedo("Albedo", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_SmoothnessTexture("Smoothness", 2D) = "white" {}
		[Enum(Back,2,Front,1,Double Sided,0)][Header(Settings)][Space(5)]_CullMode("Cull Mode", Float) = 0
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.35
		[Space(12)]_MainColor("Main Color", Color) = (1,1,1,0)
		_NormalScale("Normal Scale", Float) = 1
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[KeywordEnum(Smoothness_R,Smoothness_Alpha,Albedo_Alpha)] _SmoothnessSource("Smoothness Source", Float) = 0
		[Header(Second Color)][Space(5)][Toggle(_COLOR2ENABLE_ON)] _Color2Enable("Enable", Float) = 0
		_SecondColor("Second Color", Color) = (0,0,0,0)
		[KeywordEnum(World_Position,Vertex_Position,UV)] _SecondColorOverlayType("Overlay Method", Float) = 1
		_SecondColorOffset("Offset", Float) = 0.1
		_SecondColorFade("Fade", Range( -1 , 1)) = 0.5
		_WorldScale("World Scale", Float) = 1
		[Header(Wind)][Space(5)][Toggle(_ENABLEWIND_ON)] _EnableWind("Enable", Float) = 1
		_WindForce("Force", Range( 0 , 1)) = 0.3
		_WindWavesScale("Waves Scale", Range( 0 , 1)) = 0.25
		_WindSpeed("Speed", Range( 0 , 1)) = 0.5
		[Toggle(_LOCKBYVCOORDINATES_ON)] _LockbyVcoordinates("Lock by V coordinates", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull [_CullMode]
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _ENABLEWIND_ON
		#pragma shader_feature_local _LOCKBYVCOORDINATES_ON
		#pragma shader_feature_local _COLOR2ENABLE_ON
		#pragma shader_feature_local _SECONDCOLOROVERLAYTYPE_WORLD_POSITION _SECONDCOLOROVERLAYTYPE_VERTEX_POSITION _SECONDCOLOROVERLAYTYPE_UV
		#pragma shader_feature_local _SMOOTHNESSSOURCE_SMOOTHNESS_R _SMOOTHNESSSOURCE_SMOOTHNESS_ALPHA _SMOOTHNESSSOURCE_ALBEDO_ALPHA
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows nolightmap  nodynlightmap nodirlightmap dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _CullMode;
		uniform float _WindSpeed;
		uniform float RAYGlobalWindSpeed;
		uniform float _WindWavesScale;
		uniform float RAYGlobalWavesScale;
		uniform float _WindForce;
		uniform float RAYGlobalWindForce;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _NormalScale;
		uniform float4 _MainColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _SecondColor;
		uniform float _WorldScale;
		uniform float _SecondColorOffset;
		uniform float _SecondColorFade;
		uniform sampler2D _SmoothnessTexture;
		uniform float4 _SmoothnessTexture_ST;
		uniform float _Smoothness;
		uniform float _AlphaCutoff;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime34 = _Time.y * ( ( _WindSpeed * 5 ) * RAYGlobalWindSpeed );
			float simplePerlin3D35 = snoise( ( ase_worldPos + mulTime34 )*( ( 1.0 - _WindWavesScale ) * RAYGlobalWavesScale ) );
			float temp_output_231_0 = ( simplePerlin3D35 * 0.01 );
			#ifdef _LOCKBYVCOORDINATES_ON
				float staticSwitch376 = ( temp_output_231_0 * pow( v.texcoord.xy.y , 2.0 ) );
			#else
				float staticSwitch376 = temp_output_231_0;
			#endif
			#ifdef _ENABLEWIND_ON
				float staticSwitch341 = ( staticSwitch376 * v.color.r * ( ( _WindForce * 30 ) * RAYGlobalWindForce ) );
			#else
				float staticSwitch341 = 0.0;
			#endif
			float Wind191 = staticSwitch341;
			float3 temp_cast_0 = (Wind191).xxx;
			v.vertex.xyz += temp_cast_0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 Normal888 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale );
			o.Normal = Normal888;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 tex2DNode1 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_10_0 = ( _MainColor * tex2DNode1 );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_worldPos = i.worldPos;
			float simplePerlin3D742 = snoise( ase_worldPos*_WorldScale );
			simplePerlin3D742 = simplePerlin3D742*0.5 + 0.5;
			#if defined(_SECONDCOLOROVERLAYTYPE_WORLD_POSITION)
				float staticSwitch360 = simplePerlin3D742;
			#elif defined(_SECONDCOLOROVERLAYTYPE_VERTEX_POSITION)
				float staticSwitch360 = ase_vertex3Pos.y;
			#elif defined(_SECONDCOLOROVERLAYTYPE_UV)
				float staticSwitch360 = i.uv_texcoord.y;
			#else
				float staticSwitch360 = ase_vertex3Pos.y;
			#endif
			float SecondColorMask335 = saturate( ( ( staticSwitch360 + _SecondColorOffset ) * ( _SecondColorFade * 3 ) ) );
			float4 lerpResult332 = lerp( temp_output_10_0 , ( _SecondColor * tex2D( _Albedo, uv_Albedo ) ) , SecondColorMask335);
			#ifdef _COLOR2ENABLE_ON
				float4 staticSwitch340 = lerpResult332;
			#else
				float4 staticSwitch340 = temp_output_10_0;
			#endif
			float4 Albedo259 = staticSwitch340;
			o.Albedo = Albedo259.rgb;
			float2 uv_SmoothnessTexture = i.uv_texcoord * _SmoothnessTexture_ST.xy + _SmoothnessTexture_ST.zw;
			float4 tex2DNode708 = tex2D( _SmoothnessTexture, uv_SmoothnessTexture );
			float AlbedoAlpha263 = tex2DNode1.a;
			#if defined(_SMOOTHNESSSOURCE_SMOOTHNESS_R)
				float staticSwitch894 = tex2DNode708.r;
			#elif defined(_SMOOTHNESSSOURCE_SMOOTHNESS_ALPHA)
				float staticSwitch894 = tex2DNode708.a;
			#elif defined(_SMOOTHNESSSOURCE_ALBEDO_ALPHA)
				float staticSwitch894 = AlbedoAlpha263;
			#else
				float staticSwitch894 = tex2DNode708.r;
			#endif
			float Smoothness734 = saturate( ( staticSwitch894 * _Smoothness ) );
			o.Smoothness = Smoothness734;
			o.Alpha = 1;
			clip( AlbedoAlpha263 - _AlphaCutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;892;-3674.855,-681.5496;Inherit;False;847;322;;3;887;886;888;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;262;-5618.563,-1242.181;Inherit;False;1869.362;892.9884;;12;259;263;340;332;337;367;10;1;247;3;366;368;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;336;-5620.879,612.4994;Inherit;False;1770.958;670.6239;;13;335;334;382;391;248;875;310;360;743;745;871;742;361;Second Color Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-5622.815,-252.3361;Inherit;False;3200.724;795.0045;;26;191;775;779;778;341;376;190;56;36;358;188;769;776;345;803;780;35;357;356;359;777;231;182;34;228;344;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;733;-3784.611,617.8207;Inherit;False;1364.669;448.8141;;7;734;893;709;681;894;895;708;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleNode;344;-5289.184,6.64209;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;228;-4929.309,-158.5817;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;34;-4929.502,10.40768;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;182;-4683.235,-78.35852;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleNode;231;-4152.314,16.77568;Inherit;False;0.01;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;777;-5108.283,41.22115;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;359;-3953.194,134.5189;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;356;-4186.744,326.789;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;357;-4450.891,249.002;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;35;-4470.391,12.51909;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0.4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;780;-4657.216,230.0881;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;366;-5268.594,-590.7482;Inherit;True;Property;_TextureSample0;Texture Sample 0;18;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-5269.427,-981.5227;Inherit;True;Property;_LeavesTexture;Leaves Texture;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-4864.979,-1081.864;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;367;-4868.165,-688.4203;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;337;-4885.684,-858.1598;Inherit;False;335;SecondColorMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;332;-4598.519,-901.5045;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-3973.252,-1088.417;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;803;-4849.192,182.7877;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;345;-3680.442,297.8635;Inherit;False;30;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;776;-3501.454,326.7644;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;382;-4455.726,1032.776;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;335;-4097.272,1028.304;Inherit;False;SecondColorMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;769;-3528.97,143.2147;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;804;-2007.425,-256.8053;Inherit;False;259;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;806;-2021.146,-74.343;Inherit;False;734;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;889;-2007.435,-166.0276;Inherit;False;888;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;334;-4273.803,1032.669;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;745;-5533.308,707.1544;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ScaleNode;391;-4693.357,1143.171;Inherit;False;3;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;871;-5271.993,905.782;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;742;-5281.976,782.8204;Inherit;False;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;361;-5305.102,1074.611;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;875;-4643.873,954.1948;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-4896.567,-940.4174;Inherit;False;AlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;895;-3612.388,908.9882;Inherit;False;263;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;709;-2980.644,876.9644;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;893;-2808.773,877.2254;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;188;-3279.869,17.92714;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;886;-3413.692,-596.0408;Inherit;True;Property;_Normal;Normal;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;888;-3056.323,-595.2518;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;734;-2642.021,871.8897;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;887;-3615.854,-548.5499;Inherit;False;Property;_NormalScale;Normal Scale;6;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;708;-3720.643,700.9639;Inherit;True;Property;_SmoothnessTexture;Smoothness;2;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;681;-3311.093,953.3989;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;894;-3363.21,808.6749;Inherit;False;Property;_SmoothnessSource;Smoothness Source;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;3;Smoothness_R;Smoothness_Alpha;Albedo_Alpha;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;360;-5022.12,884.7486;Inherit;False;Property;_SecondColorOverlayType;Overlay Method;11;0;Create;False;0;0;0;False;0;False;0;1;1;True;;KeywordEnum;3;World_Position;Vertex_Position;UV;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;358;-4372.295,379.1421;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-5587.182,-0.7965025;Inherit;False;Property;_WindSpeed;Speed;18;0;Create;False;0;0;0;False;0;False;0.5;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-3967.36,292.0682;Inherit;False;Property;_WindForce;Force;16;0;Create;False;0;0;0;False;0;False;0.3;0.27;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;368;-5550.615,-785.1342;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;2;Header(Maps);Space(5);False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.ColorNode;247;-5184.974,-777.2593;Inherit;False;Property;_SecondColor;Second Color;10;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.09481568,0.6156863,0.3505847,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-5187.692,-1171.078;Inherit;False;Property;_MainColor;Main Color;5;0;Create;True;0;0;0;False;1;Space(12);False;1,1,1,0;0.263263,0.4790527,0.574,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;340;-4267.539,-1087.899;Inherit;False;Property;_Color2Enable;Enable;9;0;Create;False;0;0;0;False;2;Header(Second Color);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;743;-5517.619,885.2426;Inherit;False;Property;_WorldScale;World Scale;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;248;-4987.999,1138.4;Inherit;False;Property;_SecondColorFade;Fade;13;0;Create;False;0;0;0;False;0;False;0.5;0.41;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;310;-4867.37,1035.208;Inherit;False;Property;_SecondColorOffset;Offset;12;0;Create;False;0;0;0;False;0;False;0.1;-0.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;190;-5137.319,177.8795;Inherit;False;Property;_WindWavesScale;Waves Scale;17;0;Create;False;0;0;0;False;0;False;0.25;0.47;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;376;-3764.635,11.86549;Inherit;False;Property;_LockbyVcoordinates;Lock by V coordinates;19;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;774;-2023.518,21.1148;Inherit;False;263;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;236;-2005.875,114.236;Inherit;False;191;Wind;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-2109.145,300.3564;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;4;0;Create;False;0;0;0;False;0;False;0.35;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;891;-1986.421,204.0967;Inherit;False;Property;_CullMode;Cull Mode;3;1;[Enum];Create;True;0;3;Back;2;Front;1;Double Sided;0;0;True;2;Header(Settings);Space(5);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;151;-1761.91,-176.9145;Float;False;True;-1;2;;0;0;Standard;Raygeas/Highlands Foliage;False;False;False;False;False;False;True;True;True;False;False;False;True;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;3;False;;1;False;;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;5;False;;10;False;;4;1;False;;1;False;;1;False;;1;False;;0;False;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;_CullMode;-1;0;True;_AlphaCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.StaticSwitch;341;-3091.946,-12.17157;Inherit;False;Property;_EnableWind;Enable;15;0;Create;False;0;0;0;False;2;Header(Wind);Space(5);False;0;1;1;True;;Toggle;2;;;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;778;-5363.274,100.4682;Float;False;Global;RAYGlobalWindSpeed;RAYGlobalWindSpeed;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;779;-4924.216,305.0882;Float;False;Global;RAYGlobalWavesScale;RAYGlobalWavesScale;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;775;-3752.041,392.7138;Float;False;Global;RAYGlobalWindForce;RAYGlobalWindForce;23;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;191;-2801.433,-10.98094;Inherit;False;Wind;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
WireConnection;344;0;36;0
WireConnection;34;0;777;0
WireConnection;182;0;228;0
WireConnection;182;1;34;0
WireConnection;231;0;35;0
WireConnection;777;0;344;0
WireConnection;777;1;778;0
WireConnection;359;0;231;0
WireConnection;359;1;356;0
WireConnection;356;0;357;2
WireConnection;356;1;358;0
WireConnection;35;0;182;0
WireConnection;35;1;780;0
WireConnection;780;0;803;0
WireConnection;780;1;779;0
WireConnection;366;0;368;0
WireConnection;1;0;368;0
WireConnection;10;0;3;0
WireConnection;10;1;1;0
WireConnection;367;0;247;0
WireConnection;367;1;366;0
WireConnection;332;0;10;0
WireConnection;332;1;367;0
WireConnection;332;2;337;0
WireConnection;259;0;340;0
WireConnection;803;0;190;0
WireConnection;345;0;56;0
WireConnection;776;0;345;0
WireConnection;776;1;775;0
WireConnection;382;0;875;0
WireConnection;382;1;391;0
WireConnection;335;0;334;0
WireConnection;334;0;382;0
WireConnection;391;0;248;0
WireConnection;742;0;745;0
WireConnection;742;1;743;0
WireConnection;875;0;360;0
WireConnection;875;1;310;0
WireConnection;263;0;1;4
WireConnection;709;0;894;0
WireConnection;709;1;681;0
WireConnection;893;0;709;0
WireConnection;188;0;376;0
WireConnection;188;1;769;1
WireConnection;188;2;776;0
WireConnection;886;5;887;0
WireConnection;888;0;886;0
WireConnection;734;0;893;0
WireConnection;894;1;708;1
WireConnection;894;0;708;4
WireConnection;894;2;895;0
WireConnection;360;1;742;0
WireConnection;360;0;871;2
WireConnection;360;2;361;2
WireConnection;340;1;10;0
WireConnection;340;0;332;0
WireConnection;376;1;231;0
WireConnection;376;0;359;0
WireConnection;151;0;804;0
WireConnection;151;1;889;0
WireConnection;151;4;806;0
WireConnection;151;10;774;0
WireConnection;151;11;236;0
WireConnection;341;0;188;0
WireConnection;191;0;341;0
ASEEND*/
//CHKSM=B68CFE902ABFB6867A64DF42416B2A3F43660E78