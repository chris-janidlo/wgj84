using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using crass;

public class UIStateManager : Singleton<UIStateManager>
{
    public List<BoardPiece> PlayerPieces;
    public GameObject ActionSelectContainer, MoveMenuContainer, ActMenuContainer;
    public Button MoveButton, ActButton, AntagonizeButton, SupportButton, ContextButton;
    public TextMeshProUGUI NameplateText, ContextButtonText, AntagonizeText, SupportText;

    enum state
    {
        none, actionMenu, actMenu, moveMenu
    }

    BoardPiece selectedPiece;
    AttackCategory selectedCategory;
    state currentState;
    List<BoardSpace> movableSpaces;
    List<BoardPiece> attackablePieces;

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Start ()
    {
        foreach (var p in PlayerPieces)
        {
            p.GetComponent<Button3D>().OnClickCallback += clickPiece;
        }

        ContextButton.onClick.AddListener(clickContextButton);

        MoveButton.onClick.AddListener(clickMoveButton);
        ActButton.onClick.AddListener(() => currentState = state.actMenu);

        SupportButton.onClick.AddListener(() => clickAttackButton(AttackCategory.Supporting));
        AntagonizeButton.onClick.AddListener(() => clickAttackButton(AttackCategory.Antagonistic));
    }

    void Update ()
    {
        NameplateText.text = selectedPiece.Name;
        AntagonizeText.text = selectedPiece.NextAntagonism.Dialog[0];
        SupportText.text = selectedPiece.NextSupport.Dialog[0];
        ContextButtonText.text = (currentState == state.none) ? "End Turn" : "Back";

        MoveButton.interactable = !selectedPiece.HasMoved;
        ActButton.interactable = !selectedPiece.HasAttacked;

        ActionSelectContainer.SetActive(currentState == state.actionMenu);
        MoveMenuContainer.SetActive(currentState == state.moveMenu);
        ActMenuContainer.SetActive(currentState == state.actMenu);
    }

    void clickPiece (object sender, System.EventArgs e)
    {
        selectedPiece = ((Button3D) sender).GetComponent<BoardPiece>();
        currentState = state.actionMenu;
    }

    void clickMoveButton ()
    {
        currentState = state.moveMenu;

        movableSpaces = BoardManager.Instance.GetCircle
        (
            selectedPiece.Space.Position,
            selectedPiece.Speed,
            s => s.CurrentPiece == null
        );

        foreach (BoardSpace s in movableSpaces)
        {
            var button = s.gameObject.AddComponent<GroundMoveButton>();
            button.OnClickCallback += clickMovableGround;
        }
    }

    void clickMovableGround (object sender, System.EventArgs e)
    {
        var s = ((GroundMoveButton) sender).GetComponent<BoardSpace>();
        BoardManager.Instance.DoMove(selectedPiece, s);
        clickContextButton();
        clearGroundButtons();
    }

    void clearGroundButtons ()
    {
        foreach (BoardSpace s in movableSpaces)
        {
            var button = s.GetComponent<GroundMoveButton>();
            button.OnClickCallback -= clickMovableGround;
            Destroy(button);
        }
    }

    void clickContextButton ()
    {
        switch (currentState)
        {
            case state.none:
                // end turn
                break;

            case state.actionMenu:
                currentState = state.none;
                break;

            case state.actMenu:
                currentState = state.actionMenu;
                clearGroundButtons();
                break;
            
            case state.moveMenu:
                currentState = state.actionMenu;

                break;
        }
    }

    void clickAttackButton (AttackCategory category)
    {
        selectedCategory = category;

        attackablePieces = BoardManager.Instance.GetCircle
        (
            selectedPiece.Space.Position,
            selectedPiece.PeekAttack(category).Range,
            s => s.CurrentPiece != null && s.CurrentPiece.Team == Team.AI
        )
        .Select(s => s.CurrentPiece).ToList();

        foreach (var p in attackablePieces)
        {
            var button = p.gameObject.AddComponent<AttackEnemyButton>();
            button.OnClickCallback += clickAttackablePiece;
        }
    }

    void clickAttackablePiece (object sender, System.EventArgs e)
    {
        var enemy = ((AttackEnemyButton) sender).GetComponent<BoardPiece>();
        BoardManager.Instance.DoAttack(selectedCategory, enemy, selectedPiece);
        clickContextButton();
        clearAttackButtons();
    }

    void clearAttackButtons ()
    {
        foreach (BoardPiece p in attackablePieces)
        {
            var button = p.GetComponent<AttackEnemyButton>();
            button.OnClickCallback -= clickAttackablePiece;
            Destroy(button);
        }
    }
}
