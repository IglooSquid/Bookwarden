using UnityEngine;
using System.Collections;

public class GridTileClick : MonoBehaviour
{
    public bool enableDebug = true;

    private Renderer tileRenderer;
    private Color originalColor;
    private Coroutine colorResetCoroutine;

    private void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }
    }

    public void OnTileClicked()
    {
        Debug.Log($"{name} was clicked!");
        var props = GetComponent<TileProperties>();
        if (props != null)
        {
            Debug.Log($"{name} contains: {props.occupancy}");
        }

        if (enableDebug && tileRenderer != null)
        {
            tileRenderer.material.color = Color.Lerp(originalColor, Color.cyan, 0.5f);

            if (colorResetCoroutine != null)
                StopCoroutine(colorResetCoroutine);

            colorResetCoroutine = StartCoroutine(ResetColorAfterDelay(0.5f));
        }
    }

    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tileRenderer.material.color = originalColor;
        colorResetCoroutine = null;
    }
}