   M S B A M L   `   `   `  ����F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ViewModelsPaintAnalog.ViewModels  5   	       PaintAnalog.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3vm$clr-namespace:PaintAnalog.ViewModels     5   	   $	��800��5   !   $
��1200��6.     � WindowState$  	Maximized=�5   	    � WindowStyle$ SingleBorderWindow=�5   	    � Title 5   	   �� *Title+ � DataContext 5      *   $PaintAnalog.ViewModels.MainViewModel  5	   
    � 	Resources 5         ToggleButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName  TextBoxStyle��%�  &	        &	 �    �� 5   
   " d�6*   .�5      ��  � ��$ 40��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   �� 5       � �� �	#FFEEEEEE6+   �� 5       � ��
 ���� 6,   �� 5       � ��$
 Bold"�6+   �� 5       � �� 5      �� 5      " d�6&   .R�5      �� $	 border��6!   #��z��5      $
 6��5      $��0q�5      ._�5      �� $��Center=�6/   $��Center=�5   '   5       5      P� 5       �@J�IsMouseOver$
 True��6=   . �5    "   �� $ border��6)    � �� �	#FFDDEEFF6S   5!      P� 5"       �@d�	IsChecked$
 TrueY�6;   . �5#   "   �� $ border��6)    � �� �	#FFAACCEE6S   5$      P� 5%       � {�$ False��6;   . �5&   "   �� $ border��6)    � �� �	#FFF0F0F06S   �� 5'   "    � �� �	#FFAAAAAA6?   5(      5*      5,      5-      �� 5/   
   " ��6%   .�50      ��  � ��$	 5,0q�6'   �� 51       � ��$	 5,2q�6(   �� 52       � ��
 �����6+   �� 53       � �� �	#FFCCCCCC6,   �� 54       � ��$ 1q�60   �� 55       � ��$ Center=�69   56      58      .��59      ]�  J� InputBindings 5:   
   �� 5;      
 �� Key$ Z��6    �� 	Modifiers$ Controlf�6!    �� Command 65   �� *UndoCommand+ �� CommandParameter 6U   ��  �� ElementName$ PaintCanvas���� 5<      $ C��6   $ Controlf�6!    65   �� *CopyLastImageCommand+ 6^   �� $ PaintCanvas���� 5=      $ V��6   $ Controlf�6!    65   �� *PasteImageCommand+ 6[   �� $ PaintCanvas���� 5>      $ Y��6   $ Controlf�6!    65   �� *RedoCommand+ 6U   �� $ PaintCanvas���� 5?      $ S��6   $ Controlf�6!    65   �� *ConfirmChangesCommand+ 6_   �� $ PaintCanvas��.+�5A   
   `� $	��Top=�6    `� 
Background
 �����.�5B      c� $	��100��6   
�������.@�5C      �� $��File & Canvas��6   $��5q�66   .��5D      ��  �� Orientation$ 
Horizontal=�6!   .+�5E      �� -   5         ��Name$ 
FileButton��5E   !   $
��File��65     ToolBarButtonStyle#��[ 6D   
 � Tag 6p   �� $ PaintCanvas�� � ContextMenu 5F      �� 5G   "    6.   �� *PlacementTarget.DataContext+ �� RelativeSource �� *Self+.@�5H   &   m� $
��Save��6/    m� Command 5I      �� *SaveCanvasCommand+ m� CommandParameter 5J      �� *PlacementTarget.Tag+ ��  �� AncestorType ContextMenum� 5K   &   $
��Open��6/    5L      �� *OpenImageCommand+ 5M      �� *PlacementTarget.Tag+ ��  ContextMenu5N   #   5P      �� 5Q      $��Clear��6!   #��[ 6�   ��61   �� *ClearCanvasCommand+��6X   �� $ PaintCanvas��6�   �� 5R      -   5        $��Resize Canvas��5R   !   #��[ 6N   5S      5T      �� 5V      $��Undo / Redo & Confirm��6   $��5q�6>   .��5W      �� $ 
Horizontal=�6!   .+�5X      �� $
��Undo��6!   #��[ 6�   ��60   �� *UndoCommand+��6P   �� $ PaintCanvas��6�   �� 5Y      $
��Redo��6!   #��[ 6�   ��60   �� *RedoCommand+��6P   �� $ PaintCanvas��6�   �� 5Z      $��Confirm Changes��6!   #��[ 6�   ��6;   �� *ConfirmChangesCommand+��6e   �� $ PaintCanvas��6�   5[      5\      �� 5^      $��Tools��6   $��5q�6*   .��5_      �� $ 
Horizontal=�6!   .+�5`      �� -   5        $��Tools��5`   !   #��[ 6I   �� 5a      $��Insert Image��6!   #��[ 6�   ��68   �� *InsertImageCommand+��6_   �� $ PaintCanvas��6�   �� 5b      $��
Enter text��6!   #��[ 6�   ��66   �� *InsertTextCommand+��6\   �� $ PaintCanvas��6�   �� 5c      -   5        $ SettingsButton��5c   !   $��Brush Settings��69   #��[ 6j   5d      5e      �� 5g      $��Text Settings��6   $��5q�62   .��5h      �� $ 
Horizontal=�6   $��Center=�6*   .+�5i   
   �� $��Size:��6   $	��5,0q�6!   $��Center=�6.   �� 5j   
   -   5        $��50��5j      #��[ 5k   
   ��5l   
   �� *
TextSize+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�  TextSizeConverter  �� 	Converter#  [ 5o   
   �� 5p   
   $��px��6   $��0,0,10,0q�6   $��Center=�60   �� 5q   
   $	��130��6   $��25��6     ComboBoxStyle#��[ 5r      ! �� SelectedItem! 5s      �� *SelectedFontFamily+��5t      -  'System.Windows.Data.CompositeCollection  5u      *�5v      Segoe UI)�6+   *�5w      	Arial)�6(   *�5x      Times New Roman)�62   *�5y      Comic Sans MS)�60   5z      5|      d� 5}   
   #��[  6   " � ToolTip$" Bold (Ctrl+B)��6_   # d� 	IsChecked# 6B   �� *IsBold+.��5~      �� $��B��6   $
��Bold"�6!   5      d� 5�   
   #��[  6   $" Italic (Ctrl+I)��6a   # 6B   �� *
IsItalic+.��5�      �� $��I��6   $��Italic$�6!   5�      d� 5�   
   #��[  6   $" Underline (Ctrl+U)��6d   # 6B   �� *IsUnderline+.��5�      �� $��U��6   $��	Underline{�6!   5�      5�      5�      �� 5�      $��Color��6   $��5q�6*   .��5�      �� $��Choose color��6   #��[ 6[   ��64   �� *ChooseColorCommand+6[   5�      5�      5�      �� 5�   
   -   5        $ ��Name$$ CanvasScrollViewer��5�      $
��Auto=�5�      $
��Auto=�5�      
�������.��5�      � .+�5�      �� -   5        % ��Name$% PaintCanvas��5�      
�������$
��1200��6A   $	��800��6N   $
��Left=�5�      $	��Top=�6,   $
}�True��6D   $
|�True��6X   & � LayoutTransform& 5�      �� 5�      -   5        CanvasScaleTransform��5�   )   5�      �� 5�      -	   5        $��CursorOverlayCanvas��5�      ' J� IsHitTestVisible$' False��64   5�      5�      5�      5�      