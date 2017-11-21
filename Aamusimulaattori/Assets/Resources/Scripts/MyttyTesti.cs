using UnityEngine;
using Aamusimulaattori;

public class MyttyTesti : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            Newspaper.Show("Housut räjähti teltassa");
            //Newspaper.Show("Varusmies heitti tuolin ikkunasta");
        }
    }
}
