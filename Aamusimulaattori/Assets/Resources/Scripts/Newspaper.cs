using UnityEngine;

namespace Aamusimulaattori
{
    public class Newspaper
    {
        static readonly GameObject _newspaperPrefab;

        public static GameObject Show(string title)
        {
            return Show(title, 128);
        }

        public static GameObject Show(string title, int fontSize)
        {
            var newspaper = Object.Instantiate(_newspaperPrefab);
            var script = newspaper.GetComponent<NewspaperBehaviour>();
            script.SetTitle(title, fontSize);
            return newspaper;
        }

        static Newspaper()
        {
            _newspaperPrefab = Resources.Load<GameObject>("Content/Special/Newspaper/Newspaper");
        }
    }
}
