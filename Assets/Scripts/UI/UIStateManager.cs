using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using crass;

public class UIStateManager : Singleton<UIStateManager>
{
    public Color MovableNormal, MovableHighlighted, MovablePressed;
    public GameObject ActionSelectContainer, MoveMenuContainer, ActMenuContainer;
    public Button MoveButton, ActButton, AntagonizeButton, SupportButton, ContextButton;
    public TextMeshProUGUI NameplateText, ContextButtonText, AntagonizeText, SupportText;

    enum state
    {
        none, actionMenu, actMenu, moveMenu
    }

    BoardPieceVis selectedPiece;
    AttackCategory selectedCategory;
    List<BoardSpaceVis> movableSpaces;
    List<BoardPieceVis> attackablePieces;

    state _currentState;
    state currentState
    {
        get => _currentState;
        set
        {
            if (attackablePieces != null) clearAttackButtons();
            if (movableSpaces != null) clearGroundButtons();
            _currentState = value;
        }
    }

    void Awake ()
    {
        SingletonSetInstance(this, true);
    }

    void Start ()
    {
        foreach (var p in BoardManager.Instance.Pieces)
        {
            bool isPlayer = p.Properties.Team == Team.Player;
            p.Interactable = isPlayer;
            p.OnClickCallback += isPlayer ? (System.Action<Button3D>) clickPlayerPiece : clickAIPiece;
        }

        foreach (var s in BoardManager.Instance.Spaces)
        {
            s.Interactable = false;
            s.OnClickCallback += clickMovableGround;
        }

        ContextButton.onClick.AddListener(clickContextButton);

        MoveButton.onClick.AddListener(clickMoveButton);
        ActButton.onClick.AddListener(() => currentState = state.actMenu);

        SupportButton.onClick.AddListener(() => clickAttackButton(AttackCategory.Supporting));
        AntagonizeButton.onClick.AddListener(() => clickAttackButton(AttackCategory.Antagonistic));
    }

    void Update ()
    {
        NameplateText.text = selectedPiece?.Properties.Name ?? "<nothing selected>";
        if (currentState == state.actMenu)
        {
            AntagonizeText.text = selectedPiece.Properties.NextAntagonism.DialogPreview;
            SupportText.text = selectedPiece.Properties.NextSupport.DialogPreview;
        }
        ContextButtonText.text = (currentState == state.none) ? "End Turn" : "Back";

        if (currentState == state.actionMenu)
        {
            MoveButton.interactable = !selectedPiece.Properties.HasMoved;
            ActButton.interactable = !selectedPiece.Properties.HasAttacked;
        }

        ActionSelectContainer.SetActive(currentState == state.actionMenu);
        MoveMenuContainer.SetActive(currentState == state.moveMenu);
        ActMenuContainer.SetActive(currentState == state.actMenu);
    }

    void clickPlayerPiece (Button3D sender)
    {
        selectedPiece = (BoardPieceVis) sender;
        currentState = state.actionMenu;
    }

    void clickMoveButton ()
    {
        currentState = state.moveMenu;

        movableSpaces = BoardManager.Instance.GetMoveableSpaceVis(selectedPiece);

        foreach (BoardSpaceVis s in movableSpaces)
        {
            s.Interactable = true;
        }
    }

    void clickMovableGround (Button3D sender)
    {
        BoardSpaceVis s = (BoardSpaceVis) sender;
        BoardManager.Instance.DoMove(selectedPiece, s);
        currentState = state.actionMenu;
    }

    void clearGroundButtons ()
    {
        foreach (BoardSpaceVis s in movableSpaces)
        {
            s.Interactable = false;
        }

        movableSpaces = null;
    }

    void clickContextButton ()
    {
        switch (currentState)
        {
            case state.none:
                // end turn
                foreach (var p in BoardManager.Instance.Pieces)
                {
                    p.Properties.HasAttacked = false;
                    p.Properties.HasMoved = false;
                }
                break;

            case state.actionMenu:
                currentState = state.none;
                selectedPiece = null;
                break;

            case state.actMenu:
            case state.moveMenu:
                currentState = state.actionMenu;
                break;
        }
    }

    void clickAttackButton (AttackCategory category)
    {
        selectedCategory = category;

        attackablePieces = BoardManager.Instance.GetAttackablePieceVis(selectedPiece, selectedCategory);

        foreach (var p in attackablePieces)
        {
            p.Interactable = true;
        }
    }

    void clickAIPiece (Button3D sender)
    {
        var enemy = (BoardPieceVis) sender;
        BoardManager.Instance.DoAttack(selectedCategory, enemy, selectedPiece);
        currentState = state.actionMenu;
    }

    void clearAttackButtons ()
    {
        foreach (BoardPieceVis p in attackablePieces)
        {
            p.Interactable = false;
        }

        attackablePieces = null;
    }
}
