using Godot;
using System;
using System.ComponentModel;
using TeroftheMagic;

public partial class BlockMouseInput : Control {

    	public override void _GuiInput(InputEvent @event) {
		base._GuiInput(@event);
        if (@event is InputEventMouseButton) {
            GetViewport().SetInputAsHandled();
        }
	}
}