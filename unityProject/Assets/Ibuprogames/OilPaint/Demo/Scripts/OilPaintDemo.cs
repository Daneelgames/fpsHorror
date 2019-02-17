///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames <hello@ibuprogames.com>. All rights reserved.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
// IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Ibuprogames.OilPaintAsset;

/// <summary>
/// Oil Paint demo.
/// </summary>
public sealed class OilPaintDemo : MonoBehaviour
{
  [SerializeField]
  private bool guiShow = true;

  private OilPaint oilPaint;

  private bool menuOpen = false;
  private bool enableCompare = true;

  private const float guiMargen = 10.0f;
  private const float guiWidth = 200.0f;

  private float updateInterval = 0.5f;
  private float accum = 0.0f;
  private int frames = 0;
  private float timeleft;
  private float fps = 0.0f;

  private GUIStyle menuStyle;
  private GUIStyle boxStyle;

  private readonly string[] intensitiesStrings = { @"Low", @"Medium", @"High", @"Custom" };

  private void OnEnable()
  {
    Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
    Camera selectedCamera = null;

    for (int i = 0; i < cameras.Length; ++i)
    {
      if (cameras[i].enabled == true)
      {
        selectedCamera = cameras[i];

        break;
      }
    }

    if (selectedCamera != null)
    {
      oilPaint = selectedCamera.gameObject.GetComponent<OilPaint>();
      if (oilPaint == null)
        oilPaint = selectedCamera.gameObject.AddComponent<OilPaint>();

      if (enableCompare == true)
        Shader.EnableKeyword(@"OILPAINT_DEMO");
      else
        Shader.DisableKeyword(@"OILPAINT_DEMO");
    }
    else
      Debug.LogWarning(@"No camera found.");

    this.enabled = oilPaint != null;
  }

  private void OnDestroy()
  {
    Shader.DisableKeyword(@"OILPAINT_DEMO");
  }

  private void Update()
  {
    timeleft -= Time.deltaTime;
    accum += Time.timeScale / Time.deltaTime;
    frames++;

    if (timeleft <= 0.0f)
    {
      fps = accum / frames;
      timeleft = updateInterval;
      accum = 0.0f;
      frames = 0;
    }

    if (Input.GetKeyUp(KeyCode.Tab) == true)
      guiShow = !guiShow;

#if !UNITY_WEBGL
    if (Input.GetKeyDown(KeyCode.Escape))
      Application.Quit();
#endif
  }

  private void OnGUI()
  {
#if UNITY_ANDROID || UNITY_IPHONE
    GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1280.0f, (float)Screen.height / 720.0f, 1.0f));
#endif
    if (oilPaint == null)
      return;

    if (menuStyle == null)
    {
      menuStyle = new GUIStyle(GUI.skin.textArea);
      menuStyle.alignment = TextAnchor.MiddleCenter;
      menuStyle.fontSize = 14;
    }

    if (boxStyle == null)
    {
      boxStyle = new GUIStyle(GUI.skin.box);
      boxStyle.normal.background = MakeTex(2, 2, new Color(0.5f, 0.5f, 0.5f, 0.5f));
      boxStyle.focused.textColor = Color.red;
    }

    if (guiShow == false)
      return;

    GUILayout.BeginHorizontal(boxStyle, GUILayout.Width(Screen.width));
    {
      GUILayout.Space(guiMargen);

      if (GUILayout.Button("MENU", menuStyle, GUILayout.Width(80.0f)) == true)
        menuOpen = !menuOpen;

      GUILayout.FlexibleSpace();

      GUILayout.Label(@"OIL PAINT", menuStyle, GUILayout.Width(200.0f));

      GUILayout.FlexibleSpace();

      if (GUILayout.Button(@"MUTE", menuStyle) == true)
        AudioListener.volume = 1.0f - AudioListener.volume;

      GUILayout.Space(guiMargen);

      if (fps < 24.0f)
        GUI.contentColor = Color.yellow;
      else if (fps < 15.0f)
        GUI.contentColor = Color.red;
      else
        GUI.contentColor = Color.green;

      GUILayout.Label(fps.ToString(@"000"), menuStyle, GUILayout.Width(40.0f));

      GUI.contentColor = Color.white;

      GUILayout.Space(guiMargen);
    }
    GUILayout.EndHorizontal();

    if (menuOpen == true)
      MenuGUI();
  }

  private void MenuGUI()
  {
    GUILayout.BeginVertical(boxStyle, GUILayout.Width(guiWidth), GUILayout.ExpandHeight(true));
    {
      GUILayout.Space(guiMargen);

      // Common.
      GUILayout.BeginVertical(boxStyle);
      {
        GUILayout.BeginHorizontal();
        {
          GUILayout.Label(@" Strength", GUILayout.Width(70));
          oilPaint.Strength = GUILayout.HorizontalSlider(oilPaint.Strength, 0.0f, 1.0f);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        {
          GUILayout.Label(@" Intensity", GUILayout.Width(70));
          oilPaint.Intensity = (OilPaintIntensities)GUILayout.Toolbar((int)oilPaint.Intensity, intensitiesStrings);
        }
        GUILayout.EndHorizontal();

        if (oilPaint.Intensity == OilPaintIntensities.Custom)
        {
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label(@" Custom", GUILayout.Width(70));
            oilPaint.CustomIntensity = (int)GUILayout.HorizontalSlider(oilPaint.CustomIntensity, 0.0f, 5.0f);
          }
          GUILayout.EndHorizontal();
        }

        enableCompare = GUILayout.Toggle(enableCompare, @" Compare");
        if (enableCompare == true)
          Shader.EnableKeyword(@"OILPAINT_DEMO");
        else
          Shader.DisableKeyword(@"OILPAINT_DEMO");
      }
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();

      // Options.
      GUILayout.BeginVertical(boxStyle);
      {
        GUILayout.Label(@"TAB - Hide/Show gui.");

        GUILayout.BeginHorizontal(boxStyle);
        {
          if (GUILayout.Button(@"Open Web") == true)
            Application.OpenURL(@"http://www.ibuprogames.com/2015/05/04/oil-paint-image-effect/");

          if (GUILayout.Button(@"Reset all values") == true)
            oilPaint.ResetDefaultValues();

#if !UNITY_WEBGL
          if (GUILayout.Button(@"Quit") == true)
            Application.Quit();
#endif
        }
        GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
    }
    GUILayout.EndVertical();
  }

  private Texture2D MakeTex(int width, int height, Color col)
  {
    Color[] pix = new Color[width * height];
    for (int i = 0; i < pix.Length; ++i)
      pix[i] = col;

    Texture2D result = new Texture2D(width, height);
    result.SetPixels(pix);
    result.Apply();

    return result;
  }
}
