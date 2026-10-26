using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cronometro : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TMP_Text textoTiempo;         // "Texto Tiempo"
    [SerializeField] private TMP_Text textoBotonPrincipal; // "Text (TMP)" dentro de BotonPrincipal
    [SerializeField] private TMP_Text textoVueltas;        // "TextoVueltas"

    [Header("Botón principal")]
    [SerializeField] private Image imagenBotonPrincipal;   // el propio objeto "BotonPrincipal"
    [SerializeField] private Color colorEmpezar = new Color32(196, 181, 253, 255); // lila
    [SerializeField] private Color colorDetener = new Color32(255, 107, 107, 255); // rojo

    [Header("Colores de las vueltas")]
    [SerializeField] private Color colorMasRapida = new Color32(74, 222, 128, 255); // verde
    [SerializeField] private Color colorMasLenta  = new Color32(255, 107, 107, 255); // rojo
    [SerializeField] private Color colorNormal    = Color.white;

    [Header("Ajustes")]
    [SerializeField] private int vueltasVisibles = 3;      // cuántas vueltas se muestran a la vez

    private float tiempo;              // segundos acumulados
    private bool corriendo;
    private float tiempoUltimaVuelta;  // para calcular la diferencia entre vueltas
    private readonly List<float> parciales = new List<float>(); // duración de cada vuelta
    private readonly List<float> totales = new List<float>();   // tiempo total en cada vuelta

    private void Start()
    {
        Reestablecer();
    }

    private void Update()
    {
        if (!corriendo) return;

        tiempo += Time.deltaTime;
        ActualizarTexto();
    }

    // ---------- Métodos para los botones ----------

    // BotonPrincipal: Empezar / Detener / Continuar
    public void AlternarCronometro()
    {
        corriendo = !corriendo;
        textoBotonPrincipal.text = corriendo ? "Detener" : "Continuar";
        imagenBotonPrincipal.color = corriendo ? colorDetener : colorEmpezar;
    }

    // BotonVuelta: solo registra vuelta si está corriendo
    public void RegistrarVuelta()
    {
        if (!corriendo) return;

        parciales.Add(tiempo - tiempoUltimaVuelta);
        totales.Add(tiempo);
        tiempoUltimaVuelta = tiempo;

        ActualizarVueltas();
    }

    // BotonReestablecer: todo a cero
    public void Reestablecer()
    {
        corriendo = false;
        tiempo = 0f;
        tiempoUltimaVuelta = 0f;
        parciales.Clear();
        totales.Clear();

        textoBotonPrincipal.text = "Empezar";
        imagenBotonPrincipal.color = colorEmpezar;
        ActualizarTexto();
        ActualizarVueltas();
    }

    // ---------- Utilidades ----------

    private void ActualizarTexto()
    {
        textoTiempo.text = Formatear(tiempo);
    }

    private void ActualizarVueltas()
    {
        // Buscar la vuelta más rápida y la más lenta entre todas las vueltas
        int indiceRapida = -1, indiceLenta = -1;
        if (parciales.Count >= 2)
        {
            indiceRapida = 0;
            indiceLenta = 0;
            for (int i = 1; i < parciales.Count; i++)
            {
                if (parciales[i] < parciales[indiceRapida]) indiceRapida = i;
                if (parciales[i] > parciales[indiceLenta]) indiceLenta = i;
            }
            // Si todas duran lo mismo, no se destaca ninguna
            if (parciales[indiceRapida] == parciales[indiceLenta])
            {
                indiceRapida = -1;
                indiceLenta = -1;
            }
        }

        var sb = new StringBuilder();
        // La más reciente arriba, y solo las últimas "vueltasVisibles"
        int limite = Mathf.Max(0, parciales.Count - vueltasVisibles);
        for (int i = parciales.Count - 1; i >= limite; i--)
        {
            Color color = colorNormal;
            if (i == indiceRapida) color = colorMasRapida;
            else if (i == indiceLenta) color = colorMasLenta;

            string linea = $"Vuelta {i + 1}   {Formatear(parciales[i])}   ({Formatear(totales[i])})";
            sb.AppendLine($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{linea}</color>");
        }

        textoVueltas.text = sb.ToString();
    }

    private string Formatear(float segundos)
    {
        int minutos = (int)(segundos / 60f);
        int seg = (int)(segundos % 60f);
        int centesimas = (int)((segundos * 100f) % 100f);
        return $"{minutos:00}:{seg:00}.{centesimas:00}";
    }
}
