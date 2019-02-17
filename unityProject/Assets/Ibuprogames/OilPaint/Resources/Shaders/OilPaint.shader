///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/// <summary>
/// Oil paint shader.
/// </summary>
Shader "Hidden/OilPaint"
{
  Properties
  {
    _MainTex ("Texture", 2D) = "white" {}
  }

  CGINCLUDE
  #include "UnityCG.cginc"

  half _Strength;
  int _Radius;
  int _RadiusDual;
  half _DepthThreshold;

  UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
  float4 _MainTex_ST;
  float4 _MainTex_TexelSize;

  UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
  sampler2D _RTT;

#if MODE_DISTANCE
  sampler2D _DistanceTex;
#endif

#ifdef COLOR_CONTROLS
  half _Brightness;
  half _Contrast;
  half _Gamma;
  half _Hue;
  half _Saturation;
#endif

  #include "OilPaintCG.cginc"
#if MODE_LOW
  #include "OilPaintLowCG.cginc"
#elif MODE_MEDIUM
  #include "OilPaintNormalCG.cginc"
#elif MODE_HIGH
  #include "OilPaintHighCG.cginc"
#else
  #include "OilPaintCustomCG.cginc"
#endif
  
  inline half4 frag(v2f i) : SV_Target
  {
    UNITY_SETUP_INSTANCE_ID(i);

    half3 pixel = SampleMainTexture(i.texcoord);
    half3 final = pixel;

#ifdef MODE_SCREEN
    final = OilPaint(i.texcoord, _Radius);

#elif MODE_LAYER
    float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord));
    
    float depthMask = tex2D(_RTT, i.texcoord).a;

    if (depthMask - depth < _DepthThreshold)
      final = OilPaint(i.texcoord, _Radius);
#elif MODE_DISTANCE
    final = OilPaint(i.texcoord, _Radius);

    float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord));

    float curve = tex2D(_DistanceTex, half2(0.5, depth));

    final = lerp(pixel, final, curve);
#else
    float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.texcoord));

    float depthMask = tex2D(_RTT, i.texcoord).a;

    if (depthMask - depth < _DepthThreshold)
      final = OilPaint(i.texcoord, _Radius);
    else
      final = OilPaint(i.texcoord, _RadiusDual);
#endif

#ifdef COLOR_CONTROLS
    final = ColorAdjust(final, _Brightness, _Contrast, _Gamma, _Hue, _Saturation);
#endif

    final = lerp(pixel, final, _Strength);

#ifdef OILPAINT_DEMO
    final = PixelDemo(pixel, final, i.texcoord);
#endif

    return half4(final, 1.0);
  }
  ENDCG

  SubShader
  {
    Cull Off
    ZWrite Off
    ZTest Always

    // Pass 0: Effect.
    Pass
    {
      Name "Pass OilPaint"

      CGPROGRAM
      #pragma target 3.0
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash

      #pragma multi_compile ___ COLOR_CONTROLS
      #pragma multi_compile ___ MODE_SCREEN MODE_LAYER MODE_DUALLAYER MODE_DISTANCE
      #pragma multi_compile ___ MODE_LOW MODE_MEDIUM MODE_HIGH MODE_CUSTOM
      #pragma multi_compile ___ OILPAINT_DEMO

      #pragma vertex vert
      #pragma fragment frag
      ENDCG
    }
  }
  
  FallBack off
}
