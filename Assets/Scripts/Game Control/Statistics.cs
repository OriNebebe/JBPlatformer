using UnityEngine;

public class Statistics : MonoBehaviour
{
    public static int[] stats;
    void Start()
    {
        ResetStatistics();
        //0 = enemieskilled
        //1 = times jumped
        //2 = stars collected
        //3 = coinscollected
        //4 = timeshealed
    }
    // metoda resetu statystyk jest wywo�ywana na pocz�tku aplikacji i na �mierci gracza
    public static void ResetStatistics()
    {
        stats = new int[5];
    }
}
