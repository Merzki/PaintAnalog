   M S B A M L   `   `   `  ����F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ViewModelsPaintAnalog.ViewModels  5   	       PaintAnalog.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3vm$clr-namespace:PaintAnalog.ViewModels     5   	   $	��800��5   !   $
��1200��6.     � WindowState$  	Maximized=�5   	    � WindowStyle$ SingleBorderWindow=�5   	    � Title 5   	   �� *Title+ � DataContext 5      *   $PaintAnalog.ViewModels.MainViewModel  5	   
   .��5      ]�  J� InputBindings 5   
   �� 5      
 �� Key$ Z��6    �� 	Modifiers$ Controlf�6!    �� Command 65   �� *UndoCommand+ �� CommandParameter 6U   �� 	 �� ElementName$	 PaintCanvas���� 5      $ Y��6   $ Controlf�6!    65   �� *RedoCommand+ 6U   �� $	 PaintCanvas��.+�5   
   c� $	��Top=�6   .@�5      �� -   5        $��Tools��5      6%   �� 5      $��Clear��6   ��6%   �� *ClearCanvasCommand+��6L   �� $	 PaintCanvas���� 5      $
��Save��6   ��6$   �� *SaveCanvasCommand+��6J   �� $	 PaintCanvas���� 5      $��Choose color��6   ��6,   �� *ChooseColorCommand+�� 5      $��
Enter text��6   ��6*   �� *InsertTextCommand+��6P   �� $	 PaintCanvas���� 5      $��Insert Image��6   ��6,   �� *InsertImageCommand+��6S   �� $	 PaintCanvas���� 5      $��Confirm Changes��6   ��6/   �� *ConfirmChangesCommand+��6Y   �� $	 PaintCanvas���� 5      $
��Undo��6   ��6$   �� *UndoCommand+��6D   �� $	 PaintCanvas���� 5      $
��Redo��6   ��6$   �� *RedoCommand+��6D   �� $	 PaintCanvas���� 5      -   5        
 ��Name$
 SettingsButton��5      $��Brush Settings��6-   6F   �� 5      -   5        $��Resize Canvas��5      6-   �� 5       �� Orientation$ 
Horizontal=�6   $��Center=�62   .+�5      �� $��
Text Size:��6   $��5,0,5,0q�6.   $��Center=�6?   �� 5       � Minimum$ 8Q�6    � Maximum$ 96Q�6%   $	��150��6r    � Value 62   �� *
TextSize+ �� UpdateSourceTrigger$ PropertyChanged=�6r   �� 5      $��5,0,10,0q�66   $��Center=�6H   ��6   �� *
TextSize+6H   �� 5       $	��150��6    �� SelectedItem 6'   �� *SelectedFontFamily+��5!      -  'System.Windows.Data.CompositeCollection  5"      *�5#      Segoe UI)�63   *�5$      	Arial)�60   *�5%      Times New Roman)�6:   *�5&      Comic Sans MS)�68   5'      5)      5*      �� 5+      $ 
Horizontal=�6   $��Center=�62   $��10,0,0,0q�6M   .+�5,      d� $��40��6   $��30��6*    � ToolTip$ Bold (Ctrl+B)��6S    d� 	IsChecked 66   �� *IsBold+.��5-      �� $��B��6    $
��Bold"�6)   $��Center=�6;   $��Center=�6V   5.      d� 5/      $��40��6   $��30��6*   $ Italic (Ctrl+I)��6U    66   �� *
IsItalic+.��50      �� $��I��6    $��Italic$�6)   $��Center=�6<   $��Center=�6W   51      d� 52      $��40��6   $��30��6*   $ Underline (Ctrl+U)��6X    66   �� *IsUnderline+.��53      �� $��U��6    $��	Underline{�6)   $��Center=�6E   $��Center=�6`   54      55      56      �� 57   
   -   5         ��Name$ CanvasScrollViewer��57      $
��Auto=�58      $
��Auto=�59      
�������.��5;      � .+�5<      �� -   5         ��Name$ PaintCanvas��5<      
�������$
��1200��6A   $	��800��6N   $
��Left=�5=      $	��Top=�6,   $
}�True��6D    � LayoutTransform 5B      �� 5C      -   5        CanvasScaleTransform��5C   )   5E      �� 5F      -   5        $��CursorOverlayCanvas��5F       J� IsHitTestVisible$ False��64   5G      5H      5I      5J      