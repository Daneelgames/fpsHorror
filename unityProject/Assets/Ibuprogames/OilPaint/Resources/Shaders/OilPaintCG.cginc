///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifndef OILPAINT_CG
#define OILPAINT_CG

/// <summary>
/// Samples MainTex.
/// </summary>
inline half3 SampleMainTexture(half2 uv)
{
#if defined(UNITY_SINGLE_PASS_STEREO)
  return tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST)).rgb;
#else
  return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv).rgb;
#endif
}

/// <summary>
/// Samples MainTex lod.
/// </summary>
inline half3 SampleMainTextureLod(half2 uv)
{
#if defined(UNITY_SINGLE_PASS_STEREO)
  return tex2Dlod(_MainTex, float4(UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST), 0.0, 0.0)).rgb;
#else
  return tex2Dlod(_MainTex, float4(uv, 0.0, 0.0)).rgb;
#endif
}

/// <summary>
/// RGB -> HSV http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl.
/// </summary>
inline half3 RGB2HSV(half3 c)
{
  const half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  const half Epsilon = 1.0e-10;

  half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
  half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

  half d = q.x - min(q.w, q.y);

  return half3(abs(q.z + (q.w - q.y) / (6.0 * d + Epsilon)), d / (q.x + Epsilon), q.x);
}

/// <summary>
/// HSV -> RGB http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl.
/// </summary>
inline half3 HSV2RGB(half3 c)
{
  const half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);

  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

/// <summary>
/// Color adjust.
/// </summary>
inline half3 ColorAdjust(half3 pixel, half brightness, half contrast, half gamma, half hue, half saturation)
{
  // Brightness.
  pixel += brightness;

  // Contrast.
  pixel = (pixel - 0.5) * ((1.015 * (contrast + 1.0)) / (1.015 - contrast)) + 0.5;

  // Hue & saturation.
  half3 hsv = RGB2HSV(pixel);

  hsv.x += hue;
  hsv.y *= saturation;

  pixel = HSV2RGB(hsv);

  // Gamma.
  pixel = pow(pixel, gamma);

  return pixel;
}

// Do not use ;)
inline half3 PixelDemo(half3 pixel, half3 final, half2 uv)
{
  half separator = (sin(_Time.x * 15.0) * 0.3) + 0.7;
  const half separatorWidth = 0.005;

  if (uv.x > separator)
    final = pixel;
  else if (abs(uv.x - separator) < separatorWidth)
    final = half3(0.9, 0.9, 0.9);

  return final;
}

struct appdata_t
{
  float4 vertex : POSITION;
  half2 texcoord : TEXCOORD0;
  UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
  float4 vertex : SV_POSITION;
  half2 texcoord : TEXCOORD0;
  UNITY_VERTEX_INPUT_INSTANCE_ID
  UNITY_VERTEX_OUTPUT_STEREO    
};

v2f vert(appdata_t v)
{
  v2f o;
  UNITY_SETUP_INSTANCE_ID(v);
  UNITY_TRANSFER_INSTANCE_ID(v, o);
  UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
  o.vertex = UnityObjectToClipPos(v.vertex);
  o.texcoord = v.texcoord;

  return o;
}

#endif
