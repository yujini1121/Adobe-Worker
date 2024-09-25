// Upgrade NOTE: upgraded instancing buffer 'RaygeasHighlandsSurface' to new syntax.

// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Raygeas/Highlands Surface"
{
	Properties
	{
		[Header(Maps)][Space(10)]_Albedo("Albedo", 2D) = "white" {}
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_MetallicSmoothness("Metallic/Smoothness", 2D) = "white" {}
		[HDR]_Emission("Emission", 2D) = "white" {}
		[Header(Coverage Maps)][Space(10)]_CoverageAlbedo("Coverage Albedo", 2D) = "white" {}
		[Normal]_CoverageNormal("Coverage Normal", 2D) = "bump" {}
		_CoverageSmoothnessTexture("Coverage Smoothness", 2D) = "white" {}
		_CoverageMask("Coverage Mask", 2D) = "white" {}
		[Enum(Back,2,Front,1,Double Sided,0)][Header(Settings)][Space(5)]_CullMode("Cull Mode", Float) = 2
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0
		[Space(12)]_Color("Main Color", Color) = (1,1,1,0)
		[HDR]_EmissionColor("Emission", Color) = (0,0,0,1)
		_NormalScale("Normal Scale", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_SurfaceSmoothness("Smoothness", Range( 0 , 1)) = 0
		[KeywordEnum(Metallic_Alpha,Albedo_Alpha,Metallic_R)] _SmoothnessSource("Smoothness Source", Float) = 0
		[Header(Coverage)][Space(5)][Toggle(_COVERAGEON_ON)] _Coverageon("Enable", Float) = 0
		_CoverageColor("Coverage Color", Color) = (1,1,1,0)
		_CoverageNormalScale("Normal Scale", Float) = 1
		_CoverageSmoothness("Smoothness", Range( 0 , 1)) = 0
		[KeywordEnum(Smoothness_R,Smoothness_Alpha,Albedo_Alpha)] _CoverageSmoothnessSource("Smoothness Source", Float) = 0
		[Header(Coverage Overlay)][Space(5)][KeywordEnum(World_Normal,Vertex_Position)] _OverlayMethod("Overlay Method", Float) = 0
		_CoverageLevel("Level", Float) = 0
		_CoverageFade("Fade", Float) = 1
		_MaskContrast("Mask Contrast", Float) = 0
		[Toggle(_BLENDNORMALS_ON)] _BlendNormals("Blend Normals", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature_local _COVERAGEON_ON
		#pragma shader_feature_local _BLENDNORMALS_ON
		#pragma shader_feature_local _OVERLAYMETHOD_WORLD_NORMAL _OVERLAYMETHOD_VERTEX_POSITION
		#pragma multi_compile_local _SMOOTHNESSSOURCE_METALLIC_ALPHA _SMOOTHNESSSOURCE_ALBEDO_ALPHA _SMOOTHNESSSOURCE_METALLIC_R
		#pragma multi_compile_local _COVERAGESMOOTHNESSSOURCE_SMOOTHNESS_R _COVERAGESMOOTHNESSSOURCE_SMOOTHNESS_ALPHA _COVERAGESMOOTHNESSSOURCE_ALBEDO_ALPHA
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform float _CullMode;
		uniform sampler2D _Normal;
		uniform sampler2D _CoverageNormal;
		uniform float _CoverageLevel;
		uniform float _MaskContrast;
		uniform sampler2D _CoverageMask;
		uniform float _CoverageFade;
		uniform float4 _Color;
		uniform sampler2D _Albedo;
		uniform float4 _CoverageColor;
		uniform sampler2D _CoverageAlbedo;
		uniform sampler2D _Emission;
		uniform float4 _EmissionColor;
		uniform float _Metallic;
		uniform sampler2D _MetallicSmoothness;
		uniform float _SurfaceSmoothness;
		uniform sampler2D _CoverageSmoothnessTexture;
		uniform float _CoverageSmoothness;
		uniform float _AlphaCutoff;

		UNITY_INSTANCING_BUFFER_START(RaygeasHighlandsSurface)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Normal_ST)
#define _Normal_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _CoverageNormal_ST)
#define _CoverageNormal_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _CoverageMask_ST)
#define _CoverageMask_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _Albedo_ST)
#define _Albedo_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _CoverageAlbedo_ST)
#define _CoverageAlbedo_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _Emission_ST)
#define _Emission_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _MetallicSmoothness_ST)
#define _MetallicSmoothness_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _CoverageSmoothnessTexture_ST)
#define _CoverageSmoothnessTexture_ST_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float, _NormalScale)
#define _NormalScale_arr RaygeasHighlandsSurface
			UNITY_DEFINE_INSTANCED_PROP(float, _CoverageNormalScale)
#define _CoverageNormalScale_arr RaygeasHighlandsSurface
		UNITY_INSTANCING_BUFFER_END(RaygeasHighlandsSurface)


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _Normal_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Normal_ST_arr, _Normal_ST);
			float2 uv_Normal = i.uv_texcoord * _Normal_ST_Instance.xy + _Normal_ST_Instance.zw;
			float _NormalScale_Instance = UNITY_ACCESS_INSTANCED_PROP(_NormalScale_arr, _NormalScale);
			float3 tex2DNode6 = UnpackScaleNormal( tex2D( _Normal, uv_Normal ), _NormalScale_Instance );
			float4 _CoverageNormal_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverageNormal_ST_arr, _CoverageNormal_ST);
			float2 uv_CoverageNormal = i.uv_texcoord * _CoverageNormal_ST_Instance.xy + _CoverageNormal_ST_Instance.zw;
			float _CoverageNormalScale_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverageNormalScale_arr, _CoverageNormalScale);
			float3 tex2DNode305 = UnpackScaleNormal( tex2D( _CoverageNormal, uv_CoverageNormal ), _CoverageNormalScale_Instance );
			#ifdef _BLENDNORMALS_ON
				float3 staticSwitch358 = BlendNormals( tex2DNode6 , tex2DNode305 );
			#else
				float3 staticSwitch358 = tex2DNode305;
			#endif
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			#if defined(_OVERLAYMETHOD_WORLD_NORMAL)
				float staticSwitch355 = (WorldNormalVector( i , tex2DNode6 )).y;
			#elif defined(_OVERLAYMETHOD_VERTEX_POSITION)
				float staticSwitch355 = ase_vertex3Pos.y;
			#else
				float staticSwitch355 = (WorldNormalVector( i , tex2DNode6 )).y;
			#endif
			float4 _CoverageMask_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverageMask_ST_arr, _CoverageMask_ST);
			float2 uv_CoverageMask = i.uv_texcoord * _CoverageMask_ST_Instance.xy + _CoverageMask_ST_Instance.zw;
			float4 CoverageMask297 = saturate( ( ( staticSwitch355 + _CoverageLevel ) * saturate( CalculateContrast(_MaskContrast,tex2D( _CoverageMask, uv_CoverageMask )) ) * ( _CoverageFade * 5 ) ) );
			float3 lerpResult309 = lerp( tex2DNode6 , staticSwitch358 , CoverageMask297.rgb);
			#ifdef _COVERAGEON_ON
				float3 staticSwitch308 = lerpResult309;
			#else
				float3 staticSwitch308 = tex2DNode6;
			#endif
			float3 Normal75 = staticSwitch308;
			o.Normal = Normal75;
			float4 _Albedo_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Albedo_ST_arr, _Albedo_ST);
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST_Instance.xy + _Albedo_ST_Instance.zw;
			float4 tex2DNode2 = tex2D( _Albedo, uv_Albedo );
			float4 temp_output_3_0 = ( _Color * tex2DNode2 );
			float4 _CoverageAlbedo_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverageAlbedo_ST_arr, _CoverageAlbedo_ST);
			float2 uv_CoverageAlbedo = i.uv_texcoord * _CoverageAlbedo_ST_Instance.xy + _CoverageAlbedo_ST_Instance.zw;
			float4 tex2DNode299 = tex2D( _CoverageAlbedo, uv_CoverageAlbedo );
			float4 lerpResult302 = lerp( temp_output_3_0 , ( _CoverageColor * tex2DNode299 ) , CoverageMask297);
			#ifdef _COVERAGEON_ON
				float4 staticSwitch304 = lerpResult302;
			#else
				float4 staticSwitch304 = temp_output_3_0;
			#endif
			float4 Albedo19 = staticSwitch304;
			o.Albedo = Albedo19.rgb;
			float4 _Emission_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_Emission_ST_arr, _Emission_ST);
			float2 uv_Emission = i.uv_texcoord * _Emission_ST_Instance.xy + _Emission_ST_Instance.zw;
			float4 Emission259 = ( tex2D( _Emission, uv_Emission ) * _EmissionColor );
			o.Emission = Emission259.rgb;
			float4 _MetallicSmoothness_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MetallicSmoothness_ST_arr, _MetallicSmoothness_ST);
			float2 uv_MetallicSmoothness = i.uv_texcoord * _MetallicSmoothness_ST_Instance.xy + _MetallicSmoothness_ST_Instance.zw;
			float4 tex2DNode239 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
			float temp_output_241_0 = ( _Metallic * tex2DNode239.r );
			float lerpResult335 = lerp( temp_output_241_0 , 0.0 , CoverageMask297.r);
			#ifdef _COVERAGEON_ON
				float staticSwitch340 = lerpResult335;
			#else
				float staticSwitch340 = temp_output_241_0;
			#endif
			float Metallic262 = staticSwitch340;
			o.Metallic = Metallic262;
			float AlbedoAlpha391 = tex2DNode2.a;
			#if defined(_SMOOTHNESSSOURCE_METALLIC_ALPHA)
				float staticSwitch266 = tex2DNode239.a;
			#elif defined(_SMOOTHNESSSOURCE_ALBEDO_ALPHA)
				float staticSwitch266 = AlbedoAlpha391;
			#elif defined(_SMOOTHNESSSOURCE_METALLIC_R)
				float staticSwitch266 = tex2DNode239.r;
			#else
				float staticSwitch266 = tex2DNode239.a;
			#endif
			float temp_output_240_0 = ( staticSwitch266 * _SurfaceSmoothness );
			float4 _CoverageSmoothnessTexture_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_CoverageSmoothnessTexture_ST_arr, _CoverageSmoothnessTexture_ST);
			float2 uv_CoverageSmoothnessTexture = i.uv_texcoord * _CoverageSmoothnessTexture_ST_Instance.xy + _CoverageSmoothnessTexture_ST_Instance.zw;
			float4 tex2DNode367 = tex2D( _CoverageSmoothnessTexture, uv_CoverageSmoothnessTexture );
			#if defined(_COVERAGESMOOTHNESSSOURCE_SMOOTHNESS_R)
				float staticSwitch371 = tex2DNode367.r;
			#elif defined(_COVERAGESMOOTHNESSSOURCE_SMOOTHNESS_ALPHA)
				float staticSwitch371 = tex2DNode367.a;
			#elif defined(_COVERAGESMOOTHNESSSOURCE_ALBEDO_ALPHA)
				float staticSwitch371 = AlbedoAlpha391;
			#else
				float staticSwitch371 = tex2DNode367.r;
			#endif
			float lerpResult333 = lerp( temp_output_240_0 , ( staticSwitch371 * _CoverageSmoothness ) , CoverageMask297.r);
			#ifdef _COVERAGEON_ON
				float staticSwitch339 = lerpResult333;
			#else
				float staticSwitch339 = temp_output_240_0;
			#endif
			float Smoothness263 = staticSwitch339;
			o.Smoothness = Smoothness263;
			o.Alpha = 1;
			clip( AlbedoAlpha391 - _AlphaCutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows dithercrossfade 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;298;-2741.373,-179.2464;Inherit;False;1478.108;797.1818;;14;297;296;360;286;289;288;287;292;390;384;385;283;357;355;Coverage Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;41;-2738.437,-1218.32;Inherit;False;1598.036;946.8154;;12;1;304;2;299;369;19;302;301;303;3;300;391;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;79;-4844.504,-1216.598;Inherit;False;2022.504;945.3936;;23;335;239;367;364;363;371;333;332;370;383;266;54;340;268;334;240;366;365;262;263;339;241;242;Metallic/Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;80;-4844.572,-183.0758;Inherit;False;2022.237;645.2857;;12;175;361;311;312;75;310;309;358;308;306;6;305;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;260;-1177.338,-174.9489;Inherit;False;806.0352;519.8389;;4;243;259;244;245;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendNormalsNode;306;-4108.269,87.18636;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;308;-3362.81,-86.23829;Inherit;False;Property;_Coverageon1;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;358;-3844.724,219.8597;Inherit;False;Property;_BlendNormals;Blend Normals;25;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;309;-3551.188,-44.03468;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;310;-3823.349,51.76005;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;-3106.467,-86.02216;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;312;-3569.003,-76.59734;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;311;-3432.45,-74.56634;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;361;-4707.541,152.4362;Inherit;False;InstancedProperty;_CoverageNormalScale;Normal Scale;18;0;Create;False;0;1;Option1;0;0;False;0;False;1;2.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;175;-4710.08,61.8754;Inherit;False;InstancedProperty;_NormalScale;Normal Scale;12;0;Create;False;0;1;Option1;0;0;False;0;False;1;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;355;-2383.182,10.72481;Inherit;False;Property;_OverlayMethod;Overlay Method;21;0;Create;True;0;0;0;False;2;Header(Coverage Overlay);Space(5);False;0;0;0;True;;KeywordEnum;2;World_Normal;Vertex_Position;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;357;-2652.578,62.55454;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;283;-2658.629,-95.56486;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;385;-2577.496,426.7458;Inherit;False;Property;_MaskContrast;Mask Contrast;24;0;Create;True;0;0;0;False;0;False;0;-0.88;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;384;-2334.496,337.7462;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;390;-2112.353,338.0813;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-4470.766,-44.31013;Inherit;True;Property;_Normal;Normal;1;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;fb4cb7e8c7f603043bb76118c978dd6f;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;305;-4471.074,221.8547;Inherit;True;Property;_CoverageNormal;Coverage Normal;5;1;[Normal];Create;True;0;0;0;False;0;False;-1;None;a050c74b858db0845a90f393385999d1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;292;-2695.304,229.4878;Inherit;True;Property;_CoverageMask;Coverage Mask;7;0;Create;True;0;0;0;False;0;False;-1;None;6a900d677377db64c824e5ff2c2e6994;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;242;-4291.986,-1108.282;Inherit;False;Property;_Metallic;Metallic;13;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;-3957.589,-1053.881;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;339;-3348.867,-802.0609;Inherit;False;Property;_Coverageon2;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;263;-3077.54,-802.058;Inherit;False;Smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;262;-3077.657,-1082.289;Inherit;False;Metallic;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;365;-3583.317,-803.762;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;366;-3453.317,-802.762;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;-3955.969,-771.8701;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;334;-3981.168,-897.821;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;340;-3345.299,-1081.767;Inherit;False;Property;_Coverageon3;Coverage;16;0;Create;False;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;304;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-4291.684,-677.8931;Inherit;False;Property;_SurfaceSmoothness;Smoothness;14;0;Create;False;0;0;0;False;0;False;0;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;266;-4305.265,-815.707;Inherit;False;Property;_SmoothnessSource;Smoothness Source;15;0;Create;True;0;0;0;True;0;False;1;0;0;True;;KeywordEnum;3;Metallic_Alpha;Albedo_Alpha;Metallic_R;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;370;-3953.178,-476.1148;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;332;-4285.304,-390.5108;Inherit;False;Property;_CoverageSmoothness;Smoothness;19;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;333;-3572.283,-773.0189;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;371;-4336.772,-542.7756;Inherit;False;Property;_CoverageSmoothnessSource;Smoothness Source;20;0;Create;False;0;0;0;True;0;False;1;0;0;True;;KeywordEnum;3;Smoothness_R;Smoothness_Alpha;Albedo_Alpha;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;363;-3586.596,-1086.644;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;364;-3458.703,-1085.644;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;367;-4736.882,-637.1857;Inherit;True;Property;_CoverageSmoothnessTexture;Coverage Smoothness;6;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;239;-4737.521,-1058.268;Inherit;True;Property;_MetallicSmoothness;Metallic/Smoothness;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;335;-3574.055,-1053.046;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;287;-2294.013,481.3725;Inherit;False;Property;_CoverageFade;Fade;23;0;Create;False;0;0;0;False;0;False;1;-0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;288;-2122.892,485.4424;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;289;-2045.346,66.38091;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;286;-2230.133,140.7794;Inherit;False;Property;_CoverageLevel;Level;22;0;Create;False;0;0;0;False;0;False;0;-1.41;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;360;-1850.652,313.1478;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;296;-1678.066,312.7967;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;297;-1505.2,307.7139;Inherit;False;CoverageMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;245;-1038.579,119.2047;Inherit;False;Property;_EmissionColor;Emission;11;1;[HDR];Create;False;0;0;0;False;0;False;0,0,0,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-757.6783,12.50504;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;259;-577.8893,8.921542;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;243;-1130.161,-105.8264;Inherit;True;Property;_Emission;Emission;3;1;[HDR];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;300;-2550.118,-708.7945;Inherit;False;Property;_CoverageColor;Coverage Color;17;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.2901961,0.6344857,0.6901961,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-2208.901,-1040.494;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;303;-2284.198,-721.6245;Inherit;False;297;CoverageMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;301;-2211.303,-614.2607;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;302;-1943.549,-831.6386;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1385.717,-1043.523;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;369;-2285.885,-416.1419;Inherit;False;CoverageAlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;299;-2634.996,-512.1555;Inherit;True;Property;_CoverageAlbedo;Coverage Albedo;4;0;Create;True;0;0;0;False;2;Header(Coverage Maps);Space(10);False;-1;None;23e13916fca698c41a77b6bac9959ad5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-2636.866,-940.9748;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;2;Header(Maps);Space(10);False;-1;None;6a900d677377db64c824e5ff2c2e6994;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;304;-1678.449,-1043.714;Inherit;False;Property;_Coverageon;Enable;16;0;Create;False;0;0;0;False;2;Header(Coverage);Space(5);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-2553.478,-1125.226;Inherit;False;Property;_Color;Main Color;10;0;Create;False;0;0;0;False;1;Space(12);False;1,1,1,0;0.668,0.5182759,0.4990805,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;77;-801.0324,-1007.085;Inherit;False;75;Normal;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;264;-800.3423,-826.9345;Inherit;False;262;Metallic;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;261;-800.3494,-917.1233;Inherit;False;259;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-800.5704,-1089.055;Inherit;False;19;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;265;-817.3434,-742.9333;Inherit;False;263;Smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-533.4035,-942.7938;Float;False;True;-1;2;;0;0;Standard;Raygeas/Highlands Surface;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Opaque;;AlphaTest;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;0;15;10;25;False;0.5;True;0;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;True;_CullMode;-1;0;True;_AlphaCutoff;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;391;-2305.294,-845.0972;Inherit;False;AlbedoAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;268;-4572.088,-792.504;Inherit;False;391;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;383;-4629.053,-429.5067;Inherit;False;391;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;392;-819.5696,-652.2864;Inherit;False;391;AlbedoAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;279;-781.6405,-553.1014;Inherit;False;Property;_CullMode;Cull Mode;8;1;[Enum];Create;True;0;3;Back;2;Front;1;Double Sided;0;0;True;2;Header(Settings);Space(5);False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;280;-905.2573,-457.2653;Inherit;False;Property;_AlphaCutoff;Alpha Cutoff;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
WireConnection;306;0;6;0
WireConnection;306;1;305;0
WireConnection;308;1;311;0
WireConnection;308;0;309;0
WireConnection;358;1;305;0
WireConnection;358;0;306;0
WireConnection;309;0;6;0
WireConnection;309;1;358;0
WireConnection;309;2;310;0
WireConnection;75;0;308;0
WireConnection;312;0;6;0
WireConnection;311;0;312;0
WireConnection;355;1;283;2
WireConnection;355;0;357;2
WireConnection;283;0;6;0
WireConnection;384;1;292;0
WireConnection;384;0;385;0
WireConnection;390;0;384;0
WireConnection;6;5;175;0
WireConnection;305;5;361;0
WireConnection;241;0;242;0
WireConnection;241;1;239;1
WireConnection;339;1;366;0
WireConnection;339;0;333;0
WireConnection;263;0;339;0
WireConnection;262;0;340;0
WireConnection;365;0;240;0
WireConnection;366;0;365;0
WireConnection;240;0;266;0
WireConnection;240;1;54;0
WireConnection;340;1;364;0
WireConnection;340;0;335;0
WireConnection;266;1;239;4
WireConnection;266;0;268;0
WireConnection;266;2;239;1
WireConnection;370;0;371;0
WireConnection;370;1;332;0
WireConnection;333;0;240;0
WireConnection;333;1;370;0
WireConnection;333;2;334;0
WireConnection;371;1;367;1
WireConnection;371;0;367;4
WireConnection;371;2;383;0
WireConnection;363;0;241;0
WireConnection;364;0;363;0
WireConnection;335;0;241;0
WireConnection;335;2;334;0
WireConnection;288;0;287;0
WireConnection;289;0;355;0
WireConnection;289;1;286;0
WireConnection;360;0;289;0
WireConnection;360;1;390;0
WireConnection;360;2;288;0
WireConnection;296;0;360;0
WireConnection;297;0;296;0
WireConnection;244;0;243;0
WireConnection;244;1;245;0
WireConnection;259;0;244;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;301;0;300;0
WireConnection;301;1;299;0
WireConnection;302;0;3;0
WireConnection;302;1;301;0
WireConnection;302;2;303;0
WireConnection;19;0;304;0
WireConnection;369;0;299;4
WireConnection;304;1;3;0
WireConnection;304;0;302;0
WireConnection;0;0;20;0
WireConnection;0;1;77;0
WireConnection;0;2;261;0
WireConnection;0;3;264;0
WireConnection;0;4;265;0
WireConnection;0;10;392;0
WireConnection;391;0;2;4
ASEEND*/
//CHKSM=4D04A8F2D36E433CAE893141A4D20DA3FFB4AC2E