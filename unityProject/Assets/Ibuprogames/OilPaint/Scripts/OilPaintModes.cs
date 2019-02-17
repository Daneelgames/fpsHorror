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
    /// Effect modes.
    /// </summary>
    public enum OilPaintModes
    {
      /// <summary>
      /// It affects the entire screen.
      /// </summary>
      Screen,

      /// <summary>
      /// It only affects objects in a layer.
      /// </summary>
      Layer,

      /// <summary>
      /// It only affects objects in two layers.
      /// </summary>
      DualLayer,

      /// <summary>
      /// Depth modulates the strength of the effect.
      /// </summary>
      Distance,
    }
  }
}
