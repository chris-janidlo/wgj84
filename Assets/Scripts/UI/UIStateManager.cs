using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using crass;

public class UIStateManager : Singleton<UIStateManager>
{
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
    Dictionary<BoardSpaceVis, Color> spaceColorMemory;

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
        spaceColorMemory = new Dictionary<BoardSpaceVis, Color>();

        foreach (var p in BoardManager.Instance.Pieces)
        {
            if (p.Properties.Team == Team.Player)
            {
                p.Clickable = true;
                p.OnClickCallback += clickPlayerPiece;
            }
            else
            {
                p.Clickable = false;
                p.OnClickCallback += clickAIPiece;
            }
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

    void clickPlayerPiece (object sender, System.EventArgs e)
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
            spaceColorMemory[s] = s.GetComponent<Renderer>().material.color;
            var button = s.gameObject.AddComponent<GroundMoveButton>();
            button.OnClickCallback += clickMovableGround;
        }
    }

    void clickMovableGround (object sender, System.EventArgs e)
    {
        var s = ((GroundMoveButton) sender).GetComponent<BoardSpaceVis>();
        BoardManager.Instance.DoMove(selectedPiece, s);
        currentState = state.actionMenu;
    }

    void clearGroundButtons ()
    {
        foreach (BoardSpaceVis s in movableSpaces)
        {
            var button = s.GetComponent<GroundMoveButton>();
            button.OnClickCallback -= clickMovableGround;
            Destroy(button);
            s.GetComponent<Renderer>().material.color = spaceColorMemory[s];
        }

        movableSpaces = null;
        spaceColorMemory.Clear();
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
            p.Clickable = true;
        }
    }

    void clickAIPiece (object sender, System.EventArgs e)
    {
        var enemy = (BoardPieceVis) sender;
        BoardManager.Instance.DoAttack(selectedCategory, enemy, selectedPiece);
        currentState = state.actionMenu;
    }

    void clearAttackButtons ()
    {
        foreach (BoardPieceVis p in attackablePieces)
        {
            p.Clickable = false;
        }

        attackablePieces = null;
    }
}
