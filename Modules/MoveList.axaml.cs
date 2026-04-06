using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Runtime.Serialization;

using System;
using System.Collections.Generic;
using Avalonia.Remote.Protocol.Input;
using System.Data.SqlTypes;
using System.Net;
using System.Linq;
using Avalonia.Data;

namespace Chess.Modules;

public partial class MoveListBox : UserControl
{
    private bool _addingMove = false;

    public event Action<MoveEntry>? EntryPressed;

    public MoveListBox() {
        InitializeComponent();

        MoveListBoxControl.SelectionChanged += (s, e) =>
        {
            if(_addingMove) return;
            if(MoveListBoxControl.SelectedItem is MoveEntry entry)
                EntryPressed?.Invoke(entry);
                // call an action and tell game view that we need to act on this shit
                // act on this shit:
                  // save current game state
                  // fetch game state from MoveList
                  // switch up the pieces for both the logic and the piece renders
                  // turn off interaction
                  // light up the piece that had moved
        };

    }

    public void AddMove(MoveEntry entry)
    {
        _addingMove = true;
        MoveListBoxControl.Items.Insert(0, entry);
        MoveListBoxControl.SelectedIndex = 0;
        _addingMove = false;
    }

    public int getLength()
    {
        int count = MoveListBoxControl.Items.Count;
        return count;
    }

    public MoveEntry? getLastMove()
    {
        return MoveListBoxControl.Items.Count > 0 
            ? MoveListBoxControl.Items[0] as MoveEntry 
            : null;
    }

    public void editMove(int index, MoveEntry e)
    {
        MoveListBoxControl.Items[index] = e;
    }

}