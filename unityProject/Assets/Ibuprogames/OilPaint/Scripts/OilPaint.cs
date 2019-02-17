///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;

using UnityEngine;

namespace Ibuprogames
{
  namespace OilPaintAsset
  {
    /// <summary>
    /// Oil paint. Based on Anisotropic Kuwahara Filtering (http://www.kyprianidis.com/p/gpupro).
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Ibuprogames/Oil Paint")]
    public sealed class OilPaint : MonoBehaviour
    {
      #region Properties.
      /// <summary>
      /// Strength of the effect [0, 1]. Default 1.
      /// </summary>
      public float Strength
      {
        get { return strength; }
        set { strength = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// Select the effect mode (Screen, Layer, DualLayer or Distance).
      /// </summary>
      public OilPaintModes Mode
      {
        get { return mode; }
        set
        {
          if (value != mode)
          {
            mode = value;

            this.GetComponent<Camera>().depthTextureMode = UseDepthMode() == true ? DepthTextureMode.None : DepthTextureMode.Depth;

            if (UseDepthMode() == true)
            {
              if (mode == OilPaintModes.DualLayer)
                intensity = OilPaintIntensities.Custom;

              CreateDepthCamera();
            }
            else
              DestroyDepthCamera();
          }
        }
      }

      /// <summary>
      /// The layer to which the effect affects. Used in Layer and DualLayer modes. Default 'Everything'.
      /// </summary>
      public LayerMask Layer
      {
        get { return layer; }
        set
        {
          layer = value;

          if (renderDepth != null)
            renderDepth.layer = layer;
        }
      }

      /// <summary>
      /// Intensity of the effect (Low, Normal, High or Custom).
      /// </summary>
      public OilPaintIntensities Intensity
      {
        get { return intensity; }
        set { intensity = value; }
      }

      /// <summary>
      /// Custom intensity, used in Custom Intensity [0, 10]. Default 4.
      /// Values above 6 are very expensive.
      /// </summary>
      public int CustomIntensity
      {
        get { return customIntensity; }
        set { customIntensity = value; }
      }

      /// <summary>
      /// Custom intensity on the second layer, used in DualLayer mode [0, 10]. Default 4.
      /// Values above 6 are very expensive.
      /// </summary>
      public int CustomIntensityDual
      {
        get { return customIntensityDual; }
        set { customIntensityDual = value; }
      }

      /// <summary>
      /// Effect force curve.
      /// </summary>
      public AnimationCurve DistanceCurve
      {
        get { return distanceCurve; }
        set
        {
          distanceCurve = value;

          UpdateDistanceCurveTexture();
        }
      }

      /// <summary>
      /// Enable color controls.
      /// </summary>
      public bool EnableColorControls
      {
        get { return enableColorControls; }
        set { enableColorControls = value; }
      }

      /// <summary>
      /// Brightness [-1.0, 1.0]. Default 0.
      /// </summary>
      public float Brightness
      {
        get { return brightness; }
        set { brightness = Mathf.Clamp(value, -1.0f, 1.0f); }
      }

      /// <summary>
      /// Contrast [-1.0, 1.0]. Default 0.
      /// </summary>
      public float Contrast
      {
        get { return contrast; }
        set { contrast = Mathf.Clamp(value, -1.0f, 1.0f); }
      }

      /// <summary>
      /// Gamma [0.1, 10.0]. Default 1.
      /// </summary>
      public float Gamma
      {
        get { return gamma; }
        set { gamma = Mathf.Clamp(value, 0.1f, 10.0f); }
      }

      /// <summary>
      /// The color wheel [0.0, 1.0]. Default 0.
      /// </summary>
      public float Hue
      {
        get { return hue; }
        set { hue = Mathf.Clamp01(value); }
      }

      /// <summary>
      /// Intensity of a colors [0.0, 1.0]. Default 1.
      /// </summary>
      public float Saturation
      {
        get { return saturation; }
        set { saturation = Mathf.Clamp(value, 0.0f, 1.0f); }
      }

      /// <summary>
      /// Accuracy of depth texture [0.0, 0.1]. Used in Layer and DualLayer. Default 0.04.
      /// </summary>
      public float DepthThreshold
      {
        get { return depthThreshold; }
        set { depthThreshold = Mathf.Clamp01(value); }
      }
      #endregion

      #region Private data.
      [SerializeField]
      private float strength = 1.0f;

      [SerializeField]
      private int customIntensity = 4;

      [SerializeField]
      private int customIntensityDual = 4;

      [SerializeField]
      private OilPaintIntensities intensity;

      [SerializeField]
      private OilPaintModes mode;

      [SerializeField]
      private LayerMask layer = -1;

      [SerializeField]
      private AnimationCurve distanceCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f, 0.0f, 0.0f), new Keyframe(1.0f, 0.0f, 0.0f, 0.0f));

      [SerializeField]
      private bool enableColorControls = false;

      [SerializeField]
      private float brightness = 0.0f;

      [SerializeField]
      private float contrast = 0.0f;

      [SerializeField]
      private float gamma = 1.0f;

      [SerializeField]
      private float hue = 0.0f;

      [SerializeField]
      private float saturation = 1.0f;

      [SerializeField]
      private float depthThreshold = 0.04f;

      [SerializeField]
      private RenderDepth renderDepth;

      private Material material;

      private Shader shader;

      private Texture2D distanceTexture;

      private static readonly string variableStrength = @"_Strength";
      private static readonly string variableRadius = @"_Radius";
      private static readonly string variableRadiusDual = @"_RadiusDual";
      private static readonly string variableBrightness = @"_Brightness";
      private static readonly string variableContrast = @"_Contrast";
      private static readonly string variableGamma = @"_Gamma";
      private static readonly string variableHue = @"_Hue";
      private static readonly string variableSaturation = @"_Saturation";
      private static readonly string variableDepthThreshold = @"_DepthThreshold";
      private static readonly string variableRenderToTexture = @"_RTT";
      private static readonly string variableDistanceTexture = @"_DistanceTex";

      private static readonly string keywordModeScreen = @"MODE_SCREEN";
      private static readonly string keywordModeLayer = @"MODE_LAYER";
      private static readonly string keywordModeDualLayer = @"MODE_DUALLAYER";
      private static readonly string keywordModeDistance = @"MODE_DISTANCE";

      private static readonly string keywordModeLow = @"MODE_LOW";
      private static readonly string keywordModeMedium = @"MODE_MEDIUM";
      private static readonly string keywordModeHigh = @"MODE_HIGH";
      private static readonly string keywordModeCustom = @"MODE_CUSTOM";

      private static readonly string keywordColorControls = @"COLOR_CONTROLS";
      #endregion

      #region Public functions.
      /// <summary>
      /// Reset to default values.
      /// </summary>
      public void ResetDefaultValues()
      {
        strength = 1.0f;

        customIntensity = 4;
        customIntensityDual = 4;

        distanceCurve = new AnimationCurve(new Keyframe(0.0f, 1.0f, 0.0f, 0.0f), new Keyframe(1.0f, 0.0f, 0.0f, 0.0f));

        brightness = 0.0f;
        contrast = 0.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        depthThreshold = 0.04f;

        layer = LayerMask.NameToLayer(@"Everything");
      }
      #endregion

      #region Private functions.
      private Material Material
      {
        get
        {
          if (material == null && shader != null)
          {
            string materialName = this.GetType().Name;

            material = new Material(shader);
            if (material != null)
            {
              material.name = materialName;
              material.hideFlags = HideFlags.HideAndDontSave;
            }
            else
            {
              Debug.LogErrorFormat("[Ibuprogames.OilPaint] '{0}' material null. Please contact with 'hello@ibuprogames.com' and send the log file.", materialName);

              this.enabled = false;
            }
          }

          return material;
        }
      }

      private bool UseDepthMode()
      {
        return mode == OilPaintModes.Layer || mode == OilPaintModes.DualLayer || mode == OilPaintModes.Distance;
      }

      private void CreateDepthCamera()
      {
        if (renderDepth == null)
        {
          GameObject go = new GameObject(@"OilPaintDepthCamera", typeof(Camera));
          go.hideFlags = HideFlags.HideAndDontSave;
          go.transform.parent = this.transform;
          go.transform.localPosition = Vector3.zero;
          go.transform.localRotation = Quaternion.identity;
          go.transform.localScale = Vector3.one;

          renderDepth = go.AddComponent<RenderDepth>();
          renderDepth.layer = layer;
        }
      }

      private void DestroyDepthCamera()
      {
        if (renderDepth != null)
        {
          GameObject obj = renderDepth.gameObject;
          renderDepth = null;

          DestroyImmediate(obj);
        }
      }

      private void ClearMaterial()
      {
        if (material != null)
#if UNITY_EDITOR
          DestroyImmediate(material);
#else
				  Destroy(material);
#endif
      }

      private bool LoadShader()
      {
        bool success = true;
        string shaderName = @"Shaders/OilPaint";

        if (SystemInfo.supportsImageEffects == true)
        {
          shader = Resources.Load<Shader>(shaderName);
          if (shader != null)
          {
            if (shader.isSupported == false)
            {
              Debug.LogWarningFormat("[Ibuprogames.OilPaint] '{0}' not supported. Please contact with 'hello@ibuprogames.com' and send the log file.", shaderName);

              success = false;
            }
            else
              ClearMaterial();
          }
          else
          {
            Debug.LogWarningFormat("[Ibuprogames.OilPaint] Shader '{0}' not found.", shaderName);

            success = false;
          }
        }
        else
        {
          Debug.LogWarning(@"[Ibuprogames.OilPaint] Hardware not support Image Effects.");

          success = false;
        }

        return success;
      }

      private void UpdateDistanceCurveTexture()
      {
        if (distanceTexture == null)
          distanceTexture = new Texture2D(512, 4);

        float step = 1.0f / (float)distanceTexture.width;
        for (int i = 0; i < distanceTexture.width; ++i)
          distanceTexture.SetPixel(i, 0, Color.white * distanceCurve.Evaluate((float)i * step));

        distanceTexture.Apply();
      }

      /// <summary>
      /// Called on the frame when a script is enabled just before any of the Update methods is called the first time.
      /// </summary>
      private void Start()
      {
        this.enabled = LoadShader();
      }

      /// <summary>
      /// Called when the object becomes enabled and active.
      /// </summary>
      private void OnEnable()
      {
        if (UseDepthMode() == true && renderDepth == null)
          CreateDepthCamera();

        Camera cam = this.GetComponent<Camera>();
        cam.depthTextureMode = UseDepthMode() == true ? DepthTextureMode.None : DepthTextureMode.Depth;
      }

      /// <summary>
      /// When the MonoBehaviour will be destroyed.
      /// </summary>
      private void OnDestroy()
      {
        ClearMaterial();
      }

      /// <summary>
      /// Called after all rendering is complete to render image.
      /// </summary>
      private void OnRenderImage(RenderTexture source, RenderTexture destination)
      {
        if (Material != null)
        {
          material.shaderKeywords = null;

          material.SetFloat(variableStrength, strength);

          switch (mode)
          {
            case OilPaintModes.Screen: material.EnableKeyword(keywordModeScreen); break;
            case OilPaintModes.Layer:
            case OilPaintModes.DualLayer:
            {
              material.EnableKeyword(mode == OilPaintModes.Layer ? keywordModeLayer : keywordModeDualLayer);

              if (renderDepth != null)
                material.SetTexture(variableRenderToTexture, renderDepth.renderTexture);

              material.SetFloat(variableDepthThreshold, depthThreshold);
            }
            break;
            case OilPaintModes.Distance:
              material.EnableKeyword(keywordModeDistance);

              if (distanceTexture == null)
                UpdateDistanceCurveTexture();

              material.SetTexture(variableDistanceTexture, distanceTexture);
            break;
          }

          switch (intensity)
          {
            case OilPaintIntensities.Low:     material.EnableKeyword(keywordModeLow); break;
            case OilPaintIntensities.Medium:  material.EnableKeyword(keywordModeMedium); break;
            case OilPaintIntensities.High:    material.EnableKeyword(keywordModeHigh); break;
            case OilPaintIntensities.Custom:
            {
              material.EnableKeyword(keywordModeCustom);
              material.SetInt(variableRadius, customIntensity);

              if (mode == OilPaintModes.DualLayer)
                material.SetInt(variableRadiusDual, customIntensityDual);
            }
            break;
          }

          if (enableColorControls == true && strength > 0.0f)
          {
            material.EnableKeyword(keywordColorControls);

            material.SetFloat(variableBrightness, brightness);
            material.SetFloat(variableContrast, contrast);
            material.SetFloat(variableGamma, 1.0f / gamma);
            material.SetFloat(variableHue, hue);
            material.SetFloat(variableSaturation, saturation);
          }          

          Graphics.Blit(source, destination, material);
        }
        else
          Graphics.Blit(source, destination);
      }
      #endregion
    }
  }
}
