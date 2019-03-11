using UnityEngine;
using System.Collections;

public class ClenchHand : MonoBehaviour 
{
	// 0 = relaxed, 1 = clenched
	public GameObject[] handStyle;
    private int handPosIndex;

    // Set to relaxed
    void Start()
    {
		//Todo: Dev Stuff
		//if (this.gameObject.transform.childCount > 2)
		//{
		//	TextMesh debugLevel = this.transform.GetChild(2).GetComponent<TextMesh>();
		//	debugLevel.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex.ToString();
		//}
		
		for (int x = 1; x < handStyle.Length; x++)
			handStyle[x].SetActive(false);
		
		handPosIndex = 0;
    }
    
    /// <summary>
    /// Set hand type (Relaxed or Clenched)
    /// </summary>
    /// <param name="index">0 = Relaxed, 1 = Clenched, 2 = Point, 3 = Gun</param>
    public void SetHand(int index)
    {
        for(int x = 0; x < handStyle.Length; x++)
        {
            handStyle[x].SetActive(false);
        }
        handStyle[index].SetActive(true);
        handPosIndex = index;
    }

    /// <summary>
    /// Gets what index the hand is at
    /// </summary>
    /// <returns>0 = Relaxed, 1 = Clenched, 2 = Point</returns>
    public int GetHand()
    {
        return handPosIndex;
    }

}
