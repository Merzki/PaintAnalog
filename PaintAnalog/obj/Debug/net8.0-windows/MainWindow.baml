   M S B A M L   `   `   `  ����F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ViewModelsPaintAnalog.ViewModels  5   	       PaintAnalog.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3vm$clr-namespace:PaintAnalog.ViewModels     5   	   $	��800��5   !   $
��1200��6.     � WindowState$  	Maximized=�5   	    � WindowStyle$ SingleBorderWindow=�5   	    � Title 5   	   �� *Title+ � DataContext 5      *   $PaintAnalog.ViewModels.MainViewModel  5	   
    � 	Resources 5         ToolBarButtonStyle �� 
TargetType �� Property �� Value  ToggleButtonStyle��%  &	        &	 m     �� 5   
   " ��6+   .�5      ��  � ��$ 5q�6'   �� 5       � ��$
 15,5q�6(   5      �� 5   
   " d�6*   .�5      ��  � ��$ 40��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   5      5      .��5      ]�  J� InputBindings 5   
   �� 5      
	 �� Key$	 Z��6   
 �� 	Modifiers$
 Controlf�6!    �� Command 65   �� *UndoCommand+ �� CommandParameter 6U   ��  �� ElementName$ PaintCanvas���� 5      $	 V��6   $
 Controlf�6!    65   �� *PasteImageCommand+ 6[   �� $ PaintCanvas���� 5      $	 Y��6   $
 Controlf�6!    65   �� *RedoCommand+ 6U   �� $ PaintCanvas���� 5      $	 S��6   $
 Controlf�6!    65   �� *ConfirmChangesCommand+ 6_   �� $ PaintCanvas��.+�5   
   `� $	��Top=�6    `� 
Background
 �����.�5      c� $	��100��6   
�������.@�5      �� $��File & Canvas��6   $��5q�66   .��5       ��  �� Orientation$ 
Horizontal=�6!   .+�5!      �� -   5         ��Name$ 
FileButton��5!   !   $
��File��65   #��[  6D   
 � Tag 6p   �� $ PaintCanvas�� � ContextMenu 5"      �� 5#   "    6.   �� *PlacementTarget.DataContext+ �� RelativeSource �� *Self+.@�5$   &   m� $
��Save��6/    m� Command 5%      �� *SaveCanvasCommand+ m� CommandParameter 5&      �� *PlacementTarget.Tag+ ��  �� AncestorType ContextMenum� 5'   &   $
��Open��6/    5(      �� *OpenImageCommand+ 5)      �� *PlacementTarget.Tag+ ��  ContextMenu5*   #   5,      �� 5-      $��Clear��6!   #��[  6�   ��61   �� *ClearCanvasCommand+��6X   �� $ PaintCanvas��6�   �� 5.      -   5        $��Resize Canvas��5.   !   #��[  6N   5/      50      �� 52      $��Undo / Redo & Confirm��6   $��5q�6>   .��53      �� $ 
Horizontal=�6!   .+�54      �� $
��Undo��6!   #��[  6�   ��60   �� *UndoCommand+��6P   �� $ PaintCanvas��6�   �� 55      $
��Redo��6!   #��[  6�   ��60   �� *RedoCommand+��6P   �� $ PaintCanvas��6�   �� 56      $��Confirm Changes��6!   #��[  6�   ��6;   �� *ConfirmChangesCommand+��6e   �� $ PaintCanvas��6�   57      58      �� 5:      $��Tools��6   $��5q�6*   .��5;      �� $ 
Horizontal=�6!   .+�5<      �� -   5        $��Tools��5<   !   #��[  6I   �� 5=      $��Insert Image��6!   #��[  6�   ��68   �� *InsertImageCommand+��6_   �� $ PaintCanvas��6�   �� 5>      $��
Enter text��6!   #��[  6�   ��66   �� *InsertTextCommand+��6\   �� $ PaintCanvas��6�   �� 5?      -   5        $ SettingsButton��5?   !   $��Brush Settings��69   #��[  6j   5@      5A      �� 5C      $��Text Settings��6   $��5q�62   .��5D      �� $ 
Horizontal=�6   $��Center=�6*   .+�5E   
   �� $��Size:��6   $	��5,0q�6!   $��Center=�6.   �� 5F   
   $��50��6   $��0,0,5,0q�6   $��Center=�6.   ��5G      �� *
TextSize+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=��� 5H   
   $��px��6   $��0,0,10,0q�6   $��Center=�60   �� 5I   
   $	��130��6   $��25��6    �� SelectedItem 6+   �� *SelectedFontFamily+��5J      -  'System.Windows.Data.CompositeCollection  5K      *�5L      Segoe UI)�6+   *�5M      	Arial)�6(   *�5N      Times New Roman)�62   *�5O      Comic Sans MS)�60   5P      5R      d� 5S   
   #��[ 6    � ToolTip$ Bold (Ctrl+B)��6_    d� 	IsChecked 6B   �� *IsBold+.��5T      �� $��B��6   $
��Bold"�6!   5U      d� 5V   
   #��[ 6   $ Italic (Ctrl+I)��6a    6B   �� *
IsItalic+.��5W      �� $��I��6   $��Italic$�6!   5X      d� 5Y   
   #��[ 6   $ Underline (Ctrl+U)��6d    6B   �� *IsUnderline+.��5Z      �� $��U��6   $��	Underline{�6!   5[      5\      5]      �� 5_      $��Color��6   $��5q�6*   .��5`      �� $��Choose color��6   #��[  6[   ��64   �� *ChooseColorCommand+6[   5a      5b      5c      �� 5d   
   -   5         ��Name$ CanvasScrollViewer��5d      $
��Auto=�5e      $
��Auto=�5f      
�������.��5h      � .+�5i      �� -   5         ��Name$ PaintCanvas��5i      
�������$
��1200��6A   $	��800��6N   $
��Left=�5j      $	��Top=�6,   $
}�True��6D    � LayoutTransform 5o      �� 5p      -   5        CanvasScaleTransform��5p   )   5r      �� 5s      -   5        $��CursorOverlayCanvas��5s       J� IsHitTestVisible$ False��64   5t      5u      5v      5w      