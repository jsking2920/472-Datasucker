Shader "Custom/ShinySparkles"
{
	Properties
	{
		[Header(Base)]
		_Mask("Mask (R=Main G=Spark B=Spec)", 2D) = "white" {}
		_MainTex("MainTex (RGB)", 2D) = "black" {}

		[Space]
		[Header(Main Gradient)]
		_MainGradient("Main Gradient", 2D) = "white" {}
		_Offset("Main Gradient Offset", Range(-1,1)) = 0
		_Brightness("Brightness", Range(0,5)) = 1
		_NormalStrength("Normal Strength", Range(0,1)) = 0.1
		_Stretch("Main Gradient Stretch", Range(0,5)) = 0.5

		[Space]
		[Header(Noisy Normal)]
		_Normal("Noisy Normal", 2D) = "white" {}
		_ScaleMain("Noisy Normal Scale", Range(0,10)) = 8

		[Space]
		[Header(Sparks)]
		_SparksGradient("Sparks Gradient", 2D) = "white" {}
		_Sparks("Sparkles Texture (RGB)", 2D) = "black" {}
		_SparkleAmount("SparkleAmount", Range(0,10)) = 1
		_CutoffSparks("Cutoff Sparks", Range(0,1)) = 0.5
		_ScaleSparks("Sparks Scale", Range(0,10)) = 5
		_StretchSparks("Sparks Stretch", Range(0,10)) = 3

		[Space]
		[Header(Fake Specular)]
		_SpecSize("Specular Size", Range(0,1)) = 0.9
		_SpecSoftness("Spec Softness", Range(0,1)) = 0




	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex,_Normal,_Sparks, _SparksGradient, _MainGradient, _Mask;

			struct Input
			{
				float2 uv_MainTex;
				float3 worldNormal; INTERNAL_DATA
				float3 viewDir;
				float3 lightDir;
				float3 worldPos;
			};

			float _Stretch;

			float _Offset;
			float _Brightness;
			float _NormalStrength;

			float _SpecSize;
			float _ScaleSparks;
			float _SpecSoftness;
			float _StretchSparks;
			float _CutoffSparks;

			float _ScaleMain;
			float _SparkleAmount;

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.lightDir = WorldSpaceLightDir(mul(unity_ObjectToWorld, v.vertex)); // get the worldspace lighting direction
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 mask = tex2D(_Mask, IN.uv_MainTex);
				fixed4 mainTexture = tex2D(_MainTex, IN.uv_MainTex);

				// fake specular basing on normal and light direction
				half d = dot(o.Normal, (IN.lightDir + IN.viewDir) * 0.5f);
				float specular = (smoothstep(_SpecSize, _SpecSize + _SpecSoftness, d));
				// specular only on the blue channel of the mask
				specular *= mask.b;

				// noisy normal texture
				fixed4 n = tex2D(_Normal, IN.uv_MainTex * _ScaleMain);

				// blend between standard normal and normal from texture
				float3 noiseNormal = BlendNormals(o.Normal, UnpackNormal(n));
				float3 blendedNormal = lerp(o.Normal, noiseNormal, _NormalStrength);

				// fresnel based on blended normals and the view direction
				half f = 1 - dot(blendedNormal, IN.viewDir) + _Offset;

				// fresnel gradient
				fixed4 fGradient = tex2D(_MainGradient,float2(f * _Stretch,f * _Stretch));
				//only on the red channel of the mask
				fGradient *= mask.r;

				// Sparkles noise tex
				float3 sparkles = tex2D(_Sparks, IN.uv_MainTex * _ScaleSparks);
				// multiply with world normals
				sparkles = (IN.worldNormal * sparkles)* 0.5;

				// sparkled fresnel
				half sparkleFres = dot(IN.viewDir, sparkles);
				sparkleFres = pow(saturate(sparkleFres), _SparkleAmount);

				// project the second gradient over the sparkles
				fixed3 SparkGradient = tex2D(_SparksGradient,(float2(sparkleFres, sparkleFres)*  _StretchSparks));

				// basic fresnel to mix with the sparkle tex
				half Fres = dot(IN.viewDir, IN.worldNormal);

				// step through the sparkles
				fixed cutoff = step(_CutoffSparks,saturate(Fres + sparkleFres)* 0.5);
				SparkGradient *= cutoff;
				// sparkles only on the green channel of the mask
				SparkGradient *= mask.g;

				// start with main gradient and brightness
				o.Albedo = fGradient.rgb * _Brightness;

				// albedo to not show up where the specs are to avoid color blending
				o.Albedo *= (1 - cutoff);

				// add in the maintexture
				o.Albedo += mainTexture;

				// the sparkles glow, if you don't want this, just add it to the albedo instead
				o.Emission = SparkGradient;

				// add in the fake specular on the blue part of the mask, add to albedo if you want it to glow
				o.Emission += specular;

				// alpha
				o.Alpha = 1;
			}
			ENDCG
		}
			FallBack "Diffuse"
}