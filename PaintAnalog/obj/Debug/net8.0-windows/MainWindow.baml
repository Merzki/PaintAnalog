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
 Controlf�6!    �� Command 65   �� *UndoCommand+ �� CommandParameter 6U   ��  �� ElementName$ PaintCanvas���� 5      $	 Y��6   $
 Controlf�6!    65   �� *RedoCommand+ 6U   �� $ PaintCanvas���� 5      $	 S��6   $
 Controlf�6!    65   �� *ConfirmChangesCommand+ 6_   �� $ PaintCanvas��.+�5   
   `� $	��Top=�6    `� 
Background
 �����.�5      c� $	��100��6   
�������.@�5      �� $��File & Canvas��6   $��5q�66   .��5      ��  �� Orientation$ 
Horizontal=�6!   .+�5       �� -   5         ��Name$ 
FileButton��5    !   $
��File��65   #��[  6D   
 � Tag 6p   �� $ PaintCanvas�� � ContextMenu 5!      �� 5"   "    6.   �� *PlacementTarget.DataContext+ �� RelativeSource �� *Self+.@�5#   &   m� $
��Save��6/    m� Command 5$      �� *SaveCanvasCommand+ m� CommandParameter 5%      �� *PlacementTarget.Tag+ ��  �� AncestorType ContextMenum� 5&   &   $
��Open��6/    5'      �� *OpenImageCommand+ 5(      �� *PlacementTarget.Tag+ ��  ContextMenu5)   #   5+      �� 5,      $��Clear��6!   #��[  6�   ��61   �� *ClearCanvasCommand+��6X   �� $ PaintCanvas��6�   �� 5-      -   5        $��Resize Canvas��5-   !   #��[  6N   5.      5/      �� 51      $��Undo / Redo & Confirm��6   $��5q�6>   .��52      �� $ 
Horizontal=�6!   .+�53      �� $
��Undo��6!   #��[  6�   ��60   �� *UndoCommand+��6P   �� $ PaintCanvas��6�   �� 54      $
��Redo��6!   #��[  6�   ��60   �� *RedoCommand+��6P   �� $ PaintCanvas��6�   �� 55      $��Confirm Changes��6!   #��[  6�   ��6;   �� *ConfirmChangesCommand+��6e   �� $ PaintCanvas��6�   56      57      �� 59      $��Tools��6   $��5q�6*   .��5:      �� $ 
Horizontal=�6!   .+�5;      �� -   5        $��Tools��5;   !   #��[  6I   �� 5<      $��Insert Image��6!   #��[  6�   ��68   �� *InsertImageCommand+��6_   �� $ PaintCanvas��6�   �� 5=      $��
Enter text��6!   #��[  6�   ��66   �� *InsertTextCommand+��6\   �� $ PaintCanvas��6�   �� 5>      -   5        $ SettingsButton��5>   !   $��Brush Settings��69   #��[  6j   5?      5@      �� 5B      $��Text Settings��6   $��5q�62   .��5C      �� $ 
Horizontal=�6   $��Center=�6*   .+�5D   
   �� $��Size:��6   $	��5,0q�6!   $��Center=�6.   �� 5E   
   $��50��6   $��0,0,5,0q�6   $��Center=�6.   ��5F      �� *
TextSize+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=��� 5G   
   $��px��6   $��0,0,10,0q�6   $��Center=�60   �� 5H   
   $	��130��6   $��25��6    �� SelectedItem 6+   �� *SelectedFontFamily+��5I      -  'System.Windows.Data.CompositeCollection  5J      *�5K      Segoe UI)�6+   *�5L      	Arial)�6(   *�5M      Times New Roman)�62   *�5N      Comic Sans MS)�60   5O      5Q      d� 5R   
   #��[ 6    � ToolTip$ Bold (Ctrl+B)��6_    d� 	IsChecked 6B   �� *IsBold+.��5S      �� $��B��6   $
��Bold"�6!   5T      d� 5U   
   #��[ 6   $ Italic (Ctrl+I)��6a    6B   �� *
IsItalic+.��5V      �� $��I��6   $��Italic$�6!   5W      d� 5X   
   #��[ 6   $ Underline (Ctrl+U)��6d    6B   �� *IsUnderline+.��5Y      �� $��U��6   $��	Underline{�6!   5Z      5[      5\      �� 5^      $��Color��6   $��5q�6*   .��5_      �� $��Choose color��6   #��[  6[   ��64   �� *ChooseColorCommand+6[   5`      5a      5b      �� 5c   
   -   5         ��Name$ CanvasScrollViewer��5c      $
��Auto=�5d      $
��Auto=�5e      
�������.��5g      � .+�5h      �� -   5         ��Name$ PaintCanvas��5h      
�������$
��1200��6A   $	��800��6N   $
��Left=�5i      $	��Top=�6,   $
}�True��6D    � LayoutTransform 5n      �� 5o      -   5        CanvasScaleTransform��5o   )   5q      �� 5r      -   5        $��CursorOverlayCanvas��5r       J� IsHitTestVisible$ False��64   5s      5t      5u      5v      