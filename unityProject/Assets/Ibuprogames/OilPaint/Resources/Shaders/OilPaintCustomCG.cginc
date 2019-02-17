///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifndef OILPAINT_CUSTOM_CG
#define OILPAINT_CUSTOM_CG

inline half3 OilPaint(half2 uv, int radius)
{
  half3 m[4] =
  {
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0}
  };

  half3 s[4] =
  {
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0},
    {0.0, 0.0, 0.0}
  };

  half3 c;
  int u, v, i, j;

  for (i = -radius; i <= 0; ++i)
    for (j = -radius; j <= 0; ++j)
    {
      c = SampleMainTextureLod(uv + half2(j, i) * _MainTex_TexelSize.xy);
      m[0] += c;
      s[0] += c * c;
    }

  for (i = -radius; i <= 0; ++i)
    for (j = 0; j <= radius; ++j)
    {
      c = SampleMainTextureLod(uv + half2(j, i) * _MainTex_TexelSize.xy);
      m[1] += c;
      s[1] += c * c;
    }

  for (i = 0; i <= radius; ++i)
    for (j = 0; j <= radius; ++j)
    {
      c = SampleMainTextureLod(uv + half2(j, i) * _MainTex_TexelSize.xy);
      m[2] += c;
      s[2] += c * c;
    }

  for (i = 0; i <= radius; ++i)
    for (j = -radius; j <= 0; ++j)
    {
      c = SampleMainTextureLod(uv + half2(j, i) * _MainTex_TexelSize.xy);
      m[3] += c;
      s[3] += c * c;
    }

  half3 final = half3(0.0, 0.0, 0.0);
  half minSigma2 = 1e+2;

  half n = half((radius + 1) * (radius + 1));

  m[0] /= n;
  s[0] = abs(s[0] / n - m[0] * m[0]);

  half sigma2 = s[0].r + s[0].g + s[0].b;
  if (sigma2 < minSigma2)
  {
    minSigma2 = sigma2;
    final = m[0];
  }

  m[1] /= n;
  s[1] = abs(s[1] / n - m[1] * m[1]);

  sigma2 = s[1].r + s[1].g + s[1].b;
  if (sigma2 < minSigma2)
  {
    minSigma2 = sigma2;
    final = m[1];
  }

  m[2] /= n;
  s[2] = abs(s[2] / n - m[2] * m[2]);

  sigma2 = s[2].r + s[2].g + s[2].b;
  if (sigma2 < minSigma2)
  {
    minSigma2 = sigma2;
    final = m[2];
  }

  m[3] /= n;
  s[3] = abs(s[3] / n - m[3] * m[3]);

  sigma2 = s[3].r + s[3].g + s[3].b;
  if (sigma2 < minSigma2)
  {
    minSigma2 = sigma2;
    final = m[3];
  }

  return final;
}

#endif