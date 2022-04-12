Shader "Custom/Slot Shader Legacy" {
	Properties{
		_Color("Color", Color) = (0,0,0,1)
		_Mask("Mask", 2D) = "white" {}
		_RingRadius("RingRadius", Range(0,1)) = 0.5
		_Thickness("Thickness", Range(0,1)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
	#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
	#pragma target 3.0

			sampler2D _Mask;

		struct Input {
			float2 uv_Mask;
		};

		fixed4 _Color;
		half _ForegroundCutoff;
		half _RingRadius;
		half _Thickness;

		void surf(Input IN, inout SurfaceOutputStandard o) {
			_RingRadius = 1 - _RingRadius;
			_ForegroundCutoff = _RingRadius + _Thickness;

			fixed x = (-0.5 + IN.uv_Mask.x) * 2;
			fixed y = (-0.5 + IN.uv_Mask.y) * 2;

			fixed radius = 1 - sqrt(x * x + y * y);
			clip(radius - _RingRadius);
			o.Albedo = _Color;
			if (radius > _ForegroundCutoff) {
				clip(-1);
			}
			o.Alpha = 1;
		}
		ENDCG
		}
			FallBack "Diffuse"
}