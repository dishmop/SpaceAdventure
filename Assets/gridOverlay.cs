using UnityEngine;
using System.Collections;

public class gridOverlay : MonoBehaviour
{

    public GameObject plane;

    public bool showMain = true;
    public bool showSub = false;

    public int gridSizeX;
    public int gridSizeY;

    public float largeStep;

    public float startX;
    public float startY;
    
    private Material lineMaterial;

    private Color mainColor = new Color(0f, 1f, 0f, 0.5f);


    void CreateLineMaterial()
    {

        if (!lineMaterial)
        {
            lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    void OnPostRender()
    {
        CreateLineMaterial();
        // set the current material
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);


        if (showMain)
        {
            GL.Color(mainColor);

            //draw vertical
            for (float i = 0; i <= gridSizeX; i += largeStep)
            {
                GL.Vertex3(startX + i, startY, 0);
                GL.Vertex3(startX + i, startY + gridSizeY, 0);
            }

            //draw horizontal
            for (float i = 0; i <= gridSizeY; i += largeStep)
            {
                GL.Vertex3(startX, startY + i, 0);
                GL.Vertex3(startX + gridSizeX, startY + i, 0);
            }
        }


        GL.End();
    }
}