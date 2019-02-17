///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;

namespace Ibuprogames
{
  namespace OilPaintAsset
  {
    /// <summary>
    /// Oil Paint inspector.
    /// </summary>
    [CustomEditor(typeof(OilPaint))]
    public sealed class OilPaintEditor : Editor
    {
      #region Private data.
      private OilPaint baseTarget;

      private string displayAdvancedSettingsKey;
      private string displayColorKey;

      private bool displayAdvancedSettings;
      private bool displayColor = true;
      #endregion

      #region Private functions.
      private void OnEnable()
      {
        string productID = this.GetType().ToString().Replace(@"Editor", string.Empty);

        displayAdvancedSettingsKey = string.Format("{0}.displayAdvancedSettings", productID);
        displayColorKey = string.Format("{0}.displayColor", productID);

        displayAdvancedSettings = EditorPrefs.GetInt(displayAdvancedSettingsKey, 0) == 1;
        displayColor = EditorPrefs.GetInt(displayColorKey, 1) == 1;

        baseTarget = this.target as OilPaint;
      }

      /// <summary>
      /// OnInspectorGUI.
      /// </summary>
      public override void OnInspectorGUI()
      {
        EditorHelper.Reset(0, 0.0f, 100.0f);

        Undo.RecordObject(baseTarget, baseTarget.GetType().Name);

        /////////////////////////////////////////////////
        // Controls.
        /////////////////////////////////////////////////

        EditorHelper.BeginVertical();
        {
          EditorHelper.Separator();

          baseTarget.Strength = EditorHelper.Slider(@"Strength", "The strength of the effect.\nFrom 0.0 (no effect) to 1.0 (full effect).", baseTarget.Strength, 0.0f, 1.0f, 1.0f);

          baseTarget.Mode = (OilPaintModes)EditorHelper.EnumPopup(@"Mode", @"Effect mode (Screen, Layer, DualLayer or Distance).", baseTarget.Mode, OilPaintModes.Screen);

          if (baseTarget.Mode == OilPaintModes.Layer || baseTarget.Mode == OilPaintModes.DualLayer)
          {
            EditorHelper.IndentLevel++;

            baseTarget.Layer = EditorHelper.LayerMask(@"Layer mask", baseTarget.Layer, LayerMask.NameToLayer(@"Everything"));

            EditorHelper.IndentLevel--;
          }
          else if (baseTarget.Mode == OilPaintModes.Distance)
            baseTarget.DistanceCurve = EditorHelper.Curve(@"    Curve", baseTarget.DistanceCurve);

          if (baseTarget.Mode == OilPaintModes.DualLayer)
          {
            EditorHelper.Label(@"Intensity");

            baseTarget.Intensity = OilPaintIntensities.Custom;
          }
          else
            baseTarget.Intensity = (OilPaintIntensities)EditorHelper.EnumPopup(@"Intensity", @"Intensity of the effect (Low, Normal, High or Custom).", baseTarget.Intensity, OilPaintIntensities.Medium);

          if (baseTarget.Intensity == OilPaintIntensities.Custom)
          {
            EditorHelper.IndentLevel++;

            baseTarget.CustomIntensity = EditorHelper.IntSlider(@"Custom", @"Custom intensity, used in Custom Intensity [0, 10]. Default 4.", baseTarget.CustomIntensity, 0, 10, 4);

            if (baseTarget.Mode == OilPaintModes.DualLayer)
              baseTarget.CustomIntensityDual = EditorHelper.IntSlider(@"Custom dual", @"Custom intensity on the second layer, used in DualLayer mode [0, 10]. Default 4.", baseTarget.CustomIntensityDual, 0, 10, 4);

            EditorHelper.IndentLevel--;
          }

          /////////////////////////////////////////////////
          // Color.
          /////////////////////////////////////////////////

          EditorHelper.Separator();

          baseTarget.EnableColorControls = EditorHelper.Header(ref displayColor, baseTarget.EnableColorControls, @"Color");
          if (displayColor == true)
          {
            EditorHelper.Enabled = baseTarget.EnableColorControls;

            EditorGUI.indentLevel++;

            baseTarget.Brightness = EditorHelper.Slider(@"Brightness", "Brightness [-1.0, 1.0]. Default 0.", baseTarget.Brightness, -1.0f, 1.0f, 0.0f);

            baseTarget.Contrast = EditorHelper.Slider(@"Contrast", "Contrast [-1.0, 1.0]. Default 0.", baseTarget.Contrast, -1.0f, 1.0f, 0.0f);

            baseTarget.Gamma = EditorHelper.Slider(@"Gamma", "Gamma [0.1, 10.0]. Default 1.", baseTarget.Gamma, 0.01f, 10.0f, 1.0f);

            baseTarget.Hue = EditorHelper.Slider(@"Hue", "The color wheel [0.0, 1.0]. Default 0.", baseTarget.Hue, 0.0f, 1.0f, 0.0f);

            baseTarget.Saturation = EditorHelper.Slider(@"Saturation", "Intensity of a colors [0.0, 1.0]. Default 1.", baseTarget.Saturation, 0.0f, 1.0f, 1.0f);

            EditorGUI.indentLevel--;

            EditorHelper.Enabled = true;
          }

          /////////////////////////////////////////////////
          // Advanced settings.
          /////////////////////////////////////////////////

          EditorHelper.Separator();

          displayAdvancedSettings = EditorHelper.Foldout(displayAdvancedSettings, @"Advanced settings");
          if (displayAdvancedSettings == true)
          {
            EditorHelper.IndentLevel++;

            baseTarget.DepthThreshold = EditorHelper.Slider(@"Depth threshold", @"Accuracy of depth texture.", baseTarget.DepthThreshold, 0.0f, 0.1f, 0.04f);

            EditorHelper.IndentLevel--;
          }

          /////////////////////////////////////////////////
          // Misc.
          /////////////////////////////////////////////////

          EditorHelper.Separator();

          if (baseTarget.Intensity == OilPaintIntensities.Custom && (baseTarget.CustomIntensity > 6 || baseTarget.CustomIntensityDual > 6))
            EditorGUILayout.HelpBox(@"Values above 6 are very expensive.", MessageType.Warning);

          EditorGUILayout.HelpBox(@"Oil paint effect based on Anisotropic Kuwahara filter.", MessageType.Info);

          EditorHelper.Separator();

          EditorHelper.BeginHorizontal();
          {
            if (GUILayout.Button(new GUIContent(@"[doc]", @"Online documentation"), GUI.skin.label) == true)
              Application.OpenURL(@"http://www.ibuprogames.com/2015/05/04/oil-paint-image-effect/");

            EditorHelper.FlexibleSpace();

            if (EditorHelper.Button(@"Reset") == true)
              baseTarget.ResetDefaultValues();
          }
          EditorHelper.EndHorizontal();
        }
        EditorHelper.EndVertical();

        EditorHelper.Separator();

        if (EditorHelper.Changed == true)
        {
          EditorPrefs.SetInt(displayAdvancedSettingsKey, displayAdvancedSettings == true ? 1 : 0);
          EditorPrefs.SetInt(displayColorKey, displayColor == true ? 1 : 0);

          EditorHelper.SetDirty(target);
        }
      }
      #endregion
    }
  }
}
