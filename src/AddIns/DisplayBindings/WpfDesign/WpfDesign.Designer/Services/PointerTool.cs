﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2669 $</version>
// </file>

using System;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class PointerTool : ITool
	{
		internal static readonly PointerTool Instance = new PointerTool();
		
		public Cursor Cursor {
			get { return null; }
		}
		
		public void Activate(IDesignPanel designPanel)
		{
			designPanel.MouseDown += OnMouseDown;
		}
		
		public void Deactivate(IDesignPanel designPanel)
		{
			designPanel.MouseDown -= OnMouseDown;
		}
		
		void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			IDesignPanel designPanel = (IDesignPanel)sender;
			DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true);
			if (result.ModelHit != null) {
				IHandlePointerToolMouseDown b = result.ModelHit.GetBehavior<IHandlePointerToolMouseDown>();
				if (b != null) {
					b.HandleSelectionMouseDown(designPanel, e, result);
				}
				if (!e.Handled) {
					if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
						e.Handled = true;
						ISelectionService selectionService = designPanel.Context.SelectionService;
						selectionService.Select(new DesignItem[] { result.ModelHit }, SelectionTypes.Auto);
						if (selectionService.IsSelected(result.ModelHit)) {
							new DragMoveMouseGesture(result.ModelHit, e.ClickCount == 2).Start(designPanel, e);
						}
					}
				}
			}
		}
	}
}
