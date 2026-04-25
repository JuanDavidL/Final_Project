using UnityEngine;
using System.Collections.Generic;

public class CameraOcclusion : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Transform target;
    [SerializeField] private float transparencia = 0.2f;
    [SerializeField] private float velocidadFade = 8f;
    [SerializeField] private LayerMask oclusionLayer;

    private class ObjetoOculto
    {
        public Renderer renderer;
        public Material[] materialesOriginales;
        public Material[] materialesCopia;
    }

    private Dictionary<Renderer, ObjetoOculto> _objetosOcultos = new Dictionary<Renderer, ObjetoOculto>();
    private List<Renderer> _detectadosEsteFrame = new List<Renderer>();

    void Update()
    {
        if (target == null) return;

        _detectadosEsteFrame.Clear();

        Vector3 direccion = target.position - transform.position;
        float distancia = direccion.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(
            transform.position,
            direccion.normalized,
            distancia,
            oclusionLayer
        );

        // ── Vuelve transparentes los detectados ───────
        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) continue;

            _detectadosEsteFrame.Add(rend);

            if (!_objetosOcultos.ContainsKey(rend))
            {
                // ✅ Crea copias de los materiales originales
                Material[] copias = new Material[rend.sharedMaterials.Length];
                for (int i = 0; i < rend.sharedMaterials.Length; i++)
                    copias[i] = new Material(rend.sharedMaterials[i]);

                var obj = new ObjetoOculto
                {
                    renderer = rend,
                    materialesOriginales = rend.sharedMaterials,
                    materialesCopia = copias
                };

                _objetosOcultos[rend] = obj;
                rend.materials = copias;

                // ✅ Configura cada copia para URP transparente
                foreach (Material mat in copias)
                    ConfigurarURPTransparente(mat);
            }

            // Fade gradual hacia transparencia
            foreach (Material mat in rend.materials)
            {
                Color c = mat.color;
                c.a = Mathf.Lerp(c.a, transparencia, velocidadFade * Time.deltaTime);
                mat.color = c;
            }
        }

        // ── Restaura los que ya no bloquean ──────────
        List<Renderer> aRestaurar = new List<Renderer>();
        foreach (var kvp in _objetosOcultos)
        {
            if (!_detectadosEsteFrame.Contains(kvp.Key))
                aRestaurar.Add(kvp.Key);
        }

        foreach (Renderer rend in aRestaurar)
        {
            // Fade de vuelta a opaco antes de restaurar
            StartCoroutine(RestaurarGradual(rend, _objetosOcultos[rend]));
            _objetosOcultos.Remove(rend);
        }
    }

    private System.Collections.IEnumerator RestaurarGradual(Renderer rend, ObjetoOculto obj)
    {
        // Fade de vuelta a alpha 1
        float timer = 0f;
        float duracion = 0.3f;

        while (timer < duracion)
        {
            if (rend == null) yield break;
            timer += Time.deltaTime;

            foreach (Material mat in rend.materials)
            {
                Color c = mat.color;
                c.a = Mathf.Lerp(transparencia, 1f, timer / duracion);
                mat.color = c;
            }
            yield return null;
        }

        // ✅ Restaura materiales originales
        if (rend != null)
            rend.materials = obj.materialesOriginales;
    }

    // ✅ Configura el material para URP transparente
    private void ConfigurarURPTransparente(Material mat)
    {
        // Surface Type = Transparent (1)
        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_Blend", 0f); // Alpha blend
        mat.SetFloat("_AlphaClip", 0f);

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);

        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.renderQueue = 3000;
    }

    void OnDestroy()
    {
        foreach (var kvp in _objetosOcultos)
        {
            if (kvp.Key != null)
                kvp.Key.materials = kvp.Value.materialesOriginales;
        }
    }
}