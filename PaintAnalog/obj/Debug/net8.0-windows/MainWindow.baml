   M S B A M L   `   `   `  ����F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ViewModelsPaintAnalog.ViewModels  5   	   ?$clr-namespace:PaintAnalog.ConvertersPaintAnalog.Converters  5   	       PaintAnalog.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   3vm$clr-namespace:PaintAnalog.ViewModels     5   	   5conv$clr-namespace:PaintAnalog.Converters     5   	   -   5        $	��800��5   !   $
��1200��6.     � WindowState$  	Maximized=�5   	    � WindowStyle$ SingleBorderWindow=�5	   	    � Title 5   	   �� *Title+ � DataContext 5
      *   $PaintAnalog.ViewModels.MainViewModel  5   
    � 	Resources 5      7   1PaintAnalog.Converters.ColorToVisibilityConverter   ColorToVisibilityConverter  ToggleButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName  TextBoxStyle J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   
BlurRadius   Color � RowDefinitions �� Padding��%�  &	        &	      &	 �    '�� �      5   
   6*   �� 5   
   " d�6*   .�5      ��  � ��$ 40��6&   �� 5       � ��$ 30��6'   �� 5       � ��$ 5q�6'   �� 5       � �� �	#FFEEEEEE6+   �� 5       � ��
 ���� 6,   �� 5       � ��$
 Bold"�6+   �� 5       � �� 5      �� 5      " d�6&   .R�5      �� $	 border��6!   #��z��5      $
 6��5      $��0q�5      ._�5      �� $��Center=�6/   $��Center=�5   '   5        5!      P� 5"       �@J�IsMouseOver$
 True��6=   . �5#   "   �� $ border��6)    � �� �	#FFDDEEFF6S   5$      P� 5%       �@d�	IsChecked$
 TrueY�6;   . �5&   "   �� $ border��6)    � �� �	#FFAACCEE6S   5'      P� 5(       � {�$ False��6;   . �5)   "   �� $ border��6)    � �� �	#FFF0F0F06S   �� 5*   "    � �� �	#FFAAAAAA6?   5+      5-      5/      50      �� 52   
   " ��6%   .�53      ��  � ��$	 5,0q�6'   �� 54       � ��$	 5,2q�6(   �� 55       � ��
 �����6+   �� 56       � �� �	#FFCCCCCC6,   �� 57       � ��$ 1q�60   �� 58       � ��$ Center=�69   59      �� 5;   
   " ��6   .�5<      ��  � ��$ 0q�60   �� 5=       � ��$ 5q�6(   �� 5>       � ��$ 5q�6'   �� 5?       � ��
 �����6+   �� 5@       � �� 5A      �� 5B      " ��6&   .R�5C      �� #��z��6!   $
 4��5D      $��0q�5E       5F        5G   "   $ 1Q�63   $ 4Q�6C   $ 	#22000000��6R   ._�5I      �  5J   "   �� 5K   &   $
��Auto �64   �� 5L   &   $��* �64   .+�5N   "   �� $��0��6)   #��z��5O   %   $
 4,4,0,0��5P   %   $ 5,0,5,0q�5Q   %   ._�5R   &   �� $��Header��67   $
��True��5S   3   $��Center=�5T   3   5U   #   �� 5V   "   $��1��6)   
�������$
 0,0,4,4��5X   %   # z��5Y   %   ._�5Z   &   �� 5[   #   5\      5]      5^      5`      5a      5c      .��5d      ]�  J� InputBindings 5e   
   �� 5f      
 �� Key$ Z��6    �� 	Modifiers$ Controlf�6!    �� Command 65   �� *UndoCommand+ �� CommandParameter 6U   ��  �� ElementName$ PaintCanvas���� 5g      $ C��6   $ Controlf�6!    65   �� *CopyLastImageCommand+ 6^   �� $ PaintCanvas���� 5h      $ V��6   $ Controlf�6!    65   �� *PasteImageCommand+ 6[   �� $ PaintCanvas���� 5i      $ Y��6   $ Controlf�6!    65   �� *RedoCommand+ 6U   �� $ PaintCanvas���� 5j      $ S��6   $ Controlf�6!    65   �� *ConfirmChangesCommand+ 6_   �� $ PaintCanvas��.+�5l   
   `� $	��Top=�6    `� 
Background
 �����.�5m      c� $	��100��6   
�������.@�5n      �� $��File & Canvas��6   $��5q�66   .��5o      ��  �� Orientation$ 
Horizontal=�6!   .+�5p      �� -   5         ��Name$ 
FileButton��5p   !   $
��File��65     ToolBarButtonStyle#��[ 6D   
 � Tag 6p   �� $ PaintCanvas�� � ContextMenu 5q      �� 5r   "    6.   �� *PlacementTarget.DataContext+  �� RelativeSource  �� *Self+.@�5s   &   m� $
��Save��6/   ! m� Command! 5t      �� *SaveCanvasCommand+" m� CommandParameter" 5u      �� *PlacementTarget.Tag+  �� # �� AncestorType# ContextMenum� 5v   &   $
��Open��6/   ! 5w      �� *OpenImageCommand+" 5x      �� *PlacementTarget.Tag+  �� # ContextMenu5y   #   5{      �� 5|      $��Clear��6!   #��[ 6�   ��61   �� *ClearCanvasCommand+��6X   �� $ PaintCanvas��6�   �� 5}      -   5        $��Resize Canvas��5}   !   #��[ 6N   5~      5      �� 5�      $��Undo / Redo & Confirm��6   $��5q�6>   .��5�      �� $ 
Horizontal=�6!   .+�5�      �� $
��Undo��6!   #��[ 6�   ��60   �� *UndoCommand+��6P   �� $ PaintCanvas��6�   �� 5�      $
��Redo��6!   #��[ 6�   ��60   �� *RedoCommand+��6P   �� $ PaintCanvas��6�   �� 5�      $��Confirm Changes��6!   #��[ 6�   ��6;   �� *ConfirmChangesCommand+��6e   �� $ PaintCanvas��6�   5�      5�      �� 5�      $��Tools��6   $��5q�6*   .��5�      �� $ 
Horizontal=�6!   .+�5�      �� -   5        $��Tools��5�   !   #��[ 6I   �� 5�      $��Insert Image��6!   #��[ 6�   ��68   �� *InsertImageCommand+��6_   �� $ PaintCanvas��6�   �� 5�      $��
Enter text��6!   #��[ 6�   ��66   �� *InsertTextCommand+��6\   �� $ PaintCanvas��6�   �� 5�      -   5        $ SettingsButton��5�   !   $��Brush Settings��69   #��[ 6j   5�      5�      �� 5�      $��Text Settings��6   $��5q�62   .��5�      �� $ 
Horizontal=�6   $��Center=�6*   .+�5�   
   �� $��Size:��6   $	��5,0q�6!   $��Center=�6.   �� 5�   
   -   5        $��50��5�      #��[ 5�   
   ��5�   
   �� *
TextSize+$ �� Mode$$ TwoWay=�% �� UpdateSourceTrigger$% PropertyChanged=�  TextSizeConverter& �� 	Converter#& [ 5�   
   �� 5�   
   $��px��6   $��0,0,10,0q�6   $��Center=�60   �� 5�   
   $	��130��6   $��25��6     ComboBoxStyle#��[ 5�      ' �� SelectedItem' 5�      �� *SelectedFontFamily+��5�      -  'System.Windows.Data.CompositeCollection  5�      *�5�      Segoe UI)�6+   *�5�      	Arial)�6(   *�5�      Times New Roman)�62   *�5�      Comic Sans MS)�60   5�      5�      d� 5�      -   5        ( d�Name$( 
BoldButton��5�      #��[ 5�      ) � ToolTip$) Bold (Ctrl+B)��5�      * d� 	IsChecked* 5�      �� *IsBold+$$ TwoWay=�.��5�      �� $��B��6(   $
��Bold"�61   5�      d� 5�      -   5        $( ItalicButton��5�   '   #��[ 5�      $) Italic (Ctrl+I)��5�      * 5�      �� *
IsItalic+$$ TwoWay=�.��5�      �� $��I��6(   $��Italic$�61   5�      d� 5�      -	   5        $( UnderlineButton��5�   '   #��[ 5�      $) Underline (Ctrl+U)��5�      * 5�      �� *IsUnderline+$$ TwoWay=�.��5�      �� $��U��6(   $��	Underline{�61   5�      5�      5�      �� 5�      $��Palete��6   $��5q�6+   $��5q�66   .��5�      �� ��6#   �� *PaletteColors+��5�      �� 5�      .R�5�   "   A� + A� Columns$+ 4��6.   5�      ��5�      �� 5�      .R�5�   "   � .+�5�   &   A� -
   5        $��20��5�   .   $��20��69   
�������$��1��5�   (   $��3q�5�      ��5�      �� 5�      A� 5�   &   $��26��6.   $��26��69   $��2��5�      ��5�      ��   �� # ItemsControl, �� Path$, DataContext.SelectedColor�y�5�   *   `� 5�   .   - `� 	Converter#- [  6;   .1�5�   2   �� $, DataContext.SelectedColor�6:     6[   �� # ItemsControl�� 5�   2   5�   /   5�   '   5�   #   5�      5�      5�      �� 5�      $��Color��6   $��5q�6*   .��5�      �� $��Choose color��6   #��[ 6[   ��64   �� *ChooseColorCommand+6[   5�      5�      5�      �� 5�   
   -   5        . ��Name$. CanvasScrollViewer��5�      $
��Auto=�5�      $
��Auto=�5�      
�������.��5�      � .+�5�      �� -   5        / ��Name$/ PaintCanvas��5�      
�������$
��1200��6A   $	��800��6N   $
��Left=�5�      $	��Top=�6,   $
}�True��6D   $
|�True��6X   0 � LayoutTransform0 5�      �� 5�      -   5        CanvasScaleTransform��5�   )   5�      �� 5�      -   5        $��CursorOverlayCanvas��5�      1 J� IsHitTestVisible$1 False��64   5�      5�      5�      5�      