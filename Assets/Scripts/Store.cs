using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Store : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMap()
    {
        SceneManager.LoadScene("Map");
    }

    public void ShowDeck()
    {
        foreach(CardData cardData in DeckManager.instance.initialDeck)
        {
            Debug.Log(cardData);
        }
            
    }
}
