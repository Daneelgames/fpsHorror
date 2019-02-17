///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifndef OILPAINT_NORMAL_CG
#define OILPAINT_NORMAL_CG

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
  int u, v;

  #define SAMPLE(u, v, n) \
    c = SampleMainTexture(uv + half2(u, v) * _MainTex_TexelSize.xy); \
    m[n] += c; \
    s[n] += c * c;

  SAMPLE(-4,-4, 0) SAMPLE(-4,-3, 0) SAMPLE(-4,-2, 0) SAMPLE(-4,-1, 0) SAMPLE(-4, 0, 0)
  SAMPLE(-3,-4, 0) SAMPLE(-3,-3, 0) SAMPLE(-3,-2, 0) SAMPLE(-3,-1, 0) SAMPLE(-3, 0, 0)
  SAMPLE(-2,-4, 0) SAMPLE(-2,-3, 0) SAMPLE(-2,-2, 0) SAMPLE(-2,-1, 0) SAMPLE(-2, 0, 0)
  SAMPLE(-1,-4, 0) SAMPLE(-1,-3, 0) SAMPLE(-1,-2, 0) SAMPLE(-1,-1, 0) SAMPLE(-1, 0, 0)
  SAMPLE( 0,-4, 0) SAMPLE( 0,-3, 0) SAMPLE( 0,-2, 0) SAMPLE( 0,-1, 0) SAMPLE( 0, 0, 0)

  SAMPLE(-4, 0, 1) SAMPLE(-4, 1, 1) SAMPLE(-4, 2, 1) SAMPLE(-4, 3, 1) SAMPLE(-4, 4, 1)
  SAMPLE(-3, 0, 1) SAMPLE(-3, 1, 1) SAMPLE(-3, 2, 1) SAMPLE(-3, 3, 1) SAMPLE(-3, 4, 1)
  SAMPLE(-2, 0, 1) SAMPLE(-2, 1, 1) SAMPLE(-2, 2, 1) SAMPLE(-2, 3, 1) SAMPLE(-2, 4, 1)
  SAMPLE(-1, 0, 1) SAMPLE(-1, 1, 1) SAMPLE(-1, 2, 1) SAMPLE(-1, 3, 1) SAMPLE(-1, 4, 1)
  SAMPLE( 0, 0, 1) SAMPLE( 0, 1, 1) SAMPLE( 0, 2, 1) SAMPLE( 0, 3, 1) SAMPLE( 0, 4, 1)

  SAMPLE(0, 0, 2) SAMPLE(0, 1, 2) SAMPLE(0, 2, 2) SAMPLE(0, 3, 2) SAMPLE(0, 4, 2)
  SAMPLE(1, 0, 2) SAMPLE(1, 1, 2) SAMPLE(1, 2, 2) SAMPLE(1, 3, 2) SAMPLE(1, 4, 2)
  SAMPLE(2, 0, 2) SAMPLE(2, 1, 2) SAMPLE(2, 2, 2) SAMPLE(2, 3, 2) SAMPLE(2, 4, 2)
  SAMPLE(3, 0, 2) SAMPLE(3, 1, 2) SAMPLE(3, 2, 2) SAMPLE(3, 3, 2) SAMPLE(3, 4, 2)
  SAMPLE(4, 0, 2) SAMPLE(4, 1, 2) SAMPLE(4, 2, 2) SAMPLE(4, 3, 2) SAMPLE(4, 4, 2)

  SAMPLE(0,-4, 3) SAMPLE(0,-3, 3) SAMPLE(0,-2, 3) SAMPLE(0,-1, 3) SAMPLE(0, 0, 3)
  SAMPLE(1,-4, 3) SAMPLE(1,-3, 3) SAMPLE(1,-2, 3) SAMPLE(1,-1, 3) SAMPLE(1, 0, 3)
  SAMPLE(2,-4, 3) SAMPLE(2,-3, 3) SAMPLE(2,-2, 3) SAMPLE(2,-1, 3) SAMPLE(2, 0, 3)
  SAMPLE(3,-4, 3) SAMPLE(3,-3, 3) SAMPLE(3,-2, 3) SAMPLE(3,-1, 3) SAMPLE(3, 0, 3)
  SAMPLE(4,-4, 3) SAMPLE(4,-3, 3) SAMPLE(4,-2, 3) SAMPLE(4,-1, 3) SAMPLE(4, 0, 3)

  #undef SAMPLE

  half3 final = half3(0.0, 0.0, 0.0);
  half minSigma2 = 1e+2;

  const half n = 25.0; // (4 + 1)^2

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