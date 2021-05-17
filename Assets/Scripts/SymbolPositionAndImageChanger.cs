using UnityEngine;
using UnityEngine.UI;

public class SymbolPositionAndImageChanger : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private GameManagement gameManager;
    private RectTransform symbol;
    private Sprite symbolSprite;
    private float symbolHeigth;
    public bool isMutable;
    #endregion

    void Start()
    {
        symbol = GetComponent<RectTransform>();
        symbolSprite = GetComponent<Image>().sprite;
        symbolHeigth = symbol.rect.height;
    }
    void Update()
    {
        CheckSymbolPosition();
    }
    void ChangeSymbolPosition()
    {
        var offset = symbol.position.y + symbolHeigth * 4;
        print(offset);
        var newPos = new Vector3(symbol.position.x, offset, symbol.position.z);
        symbol.position = newPos;
        if (isMutable) ChangeSprite();
    }

    private void ChangeSprite()
    {
        var newSymbol = symbol.GetComponentInParent<ReelInfo>().isStopping ?
            gameManager.GetFinalScreenSymbol(symbol) : gameManager.GetRandomSymbol();
        symbolSprite = newSymbol.SymbolImage;
        GetComponent<Image>().sprite = symbolSprite;
    }

    public void MakeSymbolMutable(bool isMutable)
    {
        this.isMutable = isMutable;
    }

    private void CheckSymbolPosition()
    {
        if (symbol.position.y <= -300)
        {
            ChangeSymbolPosition();
        }
    }
}
