﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3528 $</version>
// </file>

using System.Windows;
using System;
using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// A tool that creates a component.
	/// </summary>
	public class CreateComponentTool : ITool
	{
		readonly Type componentType;
        MoveLogic moveLogic;
		IChangeGroup changeGroup;
		Point createPoint;
		
		/// <summary>
		/// Creates a new CreateComponentTool instance.
		/// </summary>
		public CreateComponentTool(Type componentType)
		{
			if (componentType == null)
				throw new ArgumentNullException("componentType");
			this.componentType = componentType;
		}
		
		/// <summary>
		/// Gets the type of the component to be created.
		/// </summary>
		public Type ComponentType {
			get { return componentType; }
		}
		
		public Cursor Cursor {
			get { return Cursors.Cross; }
		}
		
		public void Activate(IDesignPanel designPanel)
		{
			designPanel.MouseDown += OnMouseDown;
			//designPanel.DragEnter += designPanel_DragOver;
            designPanel.DragOver += designPanel_DragOver;
            designPanel.Drop += designPanel_Drop;
            designPanel.DragLeave += designPanel_DragLeave;
		}
		
		public void Deactivate(IDesignPanel designPanel)
		{
			designPanel.MouseDown -= OnMouseDown;
			//designPanel.DragEnter -= designPanel_DragOver;
            designPanel.DragOver -= designPanel_DragOver;
            designPanel.Drop -= designPanel_Drop;
            designPanel.DragLeave -= designPanel_DragLeave;
		}

        void designPanel_DragOver(object sender, DragEventArgs e)
        {
			try {
				IDesignPanel designPanel = (IDesignPanel)sender;
				e.Effects = DragDropEffects.Copy;
				e.Handled = true;
				Point p = e.GetPosition(designPanel);

				if (moveLogic == null) {
					if (e.Data.GetData(typeof(CreateComponentTool)) != this) return;
					// TODO: dropLayer in designPanel
					designPanel.IsAdornerLayerHitTestVisible = false;
					DesignPanelHitTestResult result = designPanel.HitTest(p, false, true);
					
					if (result.ModelHit != null) {
						designPanel.Focus();
						DesignItem createdItem = CreateItem(designPanel.Context);
						if (AddItemWithDefaultSize(result.ModelHit, createdItem, e.GetPosition(result.ModelHit.View))) {
							moveLogic = new MoveLogic(createdItem);
							createPoint = p;
						} else {
							changeGroup.Abort();
						}
					}
				} else if ((moveLogic.ClickedOn.View as FrameworkElement).IsLoaded) {
					if (moveLogic.Operation == null) {
					    moveLogic.Start(createPoint);
					} else {
					    moveLogic.Move(p);
					}
				}
			} catch (Exception x) {
				DragDropExceptionHandler.HandleException(x);
			}
        }

        void designPanel_Drop(object sender, DragEventArgs e)
        {
			try {
				if (moveLogic != null) {
					moveLogic.Stop();
					if (moveLogic.ClickedOn.Context.ToolService.CurrentTool is CreateComponentTool) {
						moveLogic.ClickedOn.Context.ToolService.CurrentTool = 
							moveLogic.ClickedOn.Context.ToolService.PointerTool;
					}
					moveLogic.DesignPanel.IsAdornerLayerHitTestVisible = true;
					moveLogic = null;
					changeGroup.Commit();
				}
			} catch (Exception x) {
				DragDropExceptionHandler.HandleException(x);
			}
        }

        void designPanel_DragLeave(object sender, DragEventArgs e)
        {
			try {
				if (moveLogic != null) {
					moveLogic.Cancel();
					moveLogic.ClickedOn.Context.SelectionService.Select(null);
					moveLogic.DesignPanel.IsAdornerLayerHitTestVisible = true;
					moveLogic = null;
					changeGroup.Abort();

				}
			} catch (Exception x) {
				DragDropExceptionHandler.HandleException(x);
			}
        }
		
		/// <summary>
		/// Is called to create the item used by the CreateComponentTool.
		/// </summary>
		protected virtual DesignItem CreateItem(DesignContext context)
		{
			object newInstance = context.ExtensionManager.CreateInstanceWithCustomInstanceFactory(componentType, null);
			DesignItem item = context.ModelService.CreateItem(newInstance);
			changeGroup = item.OpenGroup("Drop Control");
			context.ExtensionManager.ApplyDefaultInitializers(item);
			return item;
		}
		
		internal static bool AddItemWithDefaultSize(DesignItem container, DesignItem createdItem, Point position)
		{
			var size = ModelTools.GetDefaultSize(createdItem);
			PlacementOperation operation = PlacementOperation.TryStartInsertNewComponents(
				container,
				new DesignItem[] { createdItem },
				new Rect[] { new Rect(position, size) },
				PlacementType.AddItem
			);
			if (operation != null) {
				container.Context.SelectionService.Select(new DesignItem[] { createdItem });
				operation.Commit();
				return true;
			} else {
				return false;
			}
		}
		
		void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left)) {
				e.Handled = true;
				IDesignPanel designPanel = (IDesignPanel)sender;
				DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true);
				if (result.ModelHit != null) {
					IPlacementBehavior behavior = result.ModelHit.GetBehavior<IPlacementBehavior>();
					if (behavior != null) {
						DesignItem createdItem = CreateItem(designPanel.Context);
						
						new CreateComponentMouseGesture(result.ModelHit, createdItem).Start(designPanel, e);
					}
				}
			}
		}
	}
	
	sealed class CreateComponentMouseGesture : ClickOrDragMouseGesture
	{
		DesignItem createdItem;
		PlacementOperation operation;
		DesignItem container;
		
		public CreateComponentMouseGesture(DesignItem clickedOn, DesignItem createdItem)
		{
			this.container = clickedOn;
			this.createdItem = createdItem;
			this.positionRelativeTo = clickedOn.View;
		}
		
//		GrayOutDesignerExceptActiveArea grayOut;
//		SelectionFrame frame;
//		AdornerPanel adornerPanel;
		
		Rect GetStartToEndRect(MouseEventArgs e)
		{
			Point endPoint = e.GetPosition(positionRelativeTo);
			return new Rect(
				Math.Min(startPoint.X, endPoint.X),
				Math.Min(startPoint.Y, endPoint.Y),
				Math.Abs(startPoint.X - endPoint.X),
				Math.Abs(startPoint.Y - endPoint.Y)
			);
		}
		
		protected override void OnDragStarted(MouseEventArgs e)
		{
			operation = PlacementOperation.TryStartInsertNewComponents(container,
				new DesignItem[] { createdItem },
				new Rect[] { GetStartToEndRect(e) },
				PlacementType.Resize);
			if (operation != null) {
				context.SelectionService.Select(new DesignItem[] { createdItem });
			}
		}
		
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			base.OnMouseMove(sender, e);
			if (operation != null) {
				foreach (PlacementInformation info in operation.PlacedItems) {
					info.Bounds = GetStartToEndRect(e);
					operation.CurrentContainerBehavior.SetPosition(info);
				}
			}
		}
		
		protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (hasDragStarted) {
				if (operation != null) {
					operation.Commit();
					operation = null;
				}
			} else {
				CreateComponentTool.AddItemWithDefaultSize(container, createdItem, e.GetPosition(positionRelativeTo));
			}
			base.OnMouseUp(sender, e);
		}
		
		protected override void OnStopped()
		{
			if (operation != null) {
				operation.Abort();
				operation = null;
			}
			if (context.ToolService.CurrentTool is CreateComponentTool) {
				context.ToolService.CurrentTool = context.ToolService.PointerTool;
			}
			base.OnStopped();
		}
	}
}
