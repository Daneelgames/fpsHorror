///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Ibuprogames
{
  namespace OilPaintAsset
  {
    /// <summary>
    /// Intensities of the effect.
    /// </summary>
    public enum OilPaintIntensities
    {
      /// <summary>
      /// Intensity 2.
      /// </summary>
      Low,

      /// <summary>
      /// Intensity 4.
      /// </summary>
      Medium,

      /// <summary>
      /// Intensity 6.
      /// </summary>
      High,

      /// <summary>
      /// Custom quality [0, 10]. Default 4.
      /// Values above 6 are very expensive.
      /// </summary>
      Custom,
    }
  }
}
