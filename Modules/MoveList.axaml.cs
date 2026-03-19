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
    public MoveListBox() {
        InitializeComponent();
    }

    public void AddMove(MoveEntry entry)
    {
        MoveListBoxControl.Items.Insert(0, entry);
        MoveListBoxControl.SelectedIndex = 0;
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