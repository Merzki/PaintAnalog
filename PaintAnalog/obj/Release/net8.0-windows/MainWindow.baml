   M S B A M L   `   `   `  ���� F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ViewModelsPaintAnalog.ViewModels  ?$clr-namespace:PaintAnalog.ConvertersPaintAnalog.Converters      PaintAnalog.MainWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     3vm$clr-namespace:PaintAnalog.ViewModels     5conv$clr-namespace:PaintAnalog.Converters     -   $	��800��$
��1200��  � WindowState$  	Maximized=� � WindowStyle$ SingleBorderWindow=� � Title �� *Title+ � DataContext *   $PaintAnalog.ViewModels.MainViewModel   � 	Resources 7   1PaintAnalog.Converters.ColorToVisibilityConverter   ColorToVisibilityConverter  ToggleButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName  TextBoxStyle J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   
BlurRadius   Color � RowDefinitions �� Padding��%b  &	        &	      &	     '�� �      �� " d�.���  � ��$ 40����  � ��$ 30����  � ��$ 5q���  � �� �	#FFEEEEEE��  � ��
 ���� ��  � ��$
 Bold"���  � �� �� " d�.R��� $	 border��#��z��$
 6��$��0q�._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ��� $ border�� � �� �	#FFDDEEFFP�  �@d�	IsChecked$
 TrueY�. ��� $ border�� � �� �	#FFAACCEEP�  � {�$ False��. ��� $ border�� � �� �	#FFF0F0F0��  � �� �	#FFAAAAAA�� " ��.���  � ��$	 5,0q���  � ��$	 5,2q���  � ��
 �������  � �� �	#FFCCCCCC��  � ��$ 1q���  � ��$ Center=��� " ��.���  � ��$ 0q���  � ��$ 5q���  � ��$ 5q���  � ��
 �������  � �� �� " ��.R��� #��z��$
 4��$��0q�   $ 1Q�$ 4Q�$ 	#22000000��._��  �� $
��Auto ��� $��* �.+��� $��0��#��z��$
 4,4,0,0��$ 5,0,5,0q�._��� $��Header��$
��True��$��Center=��� $��1��
�������$
 0,0,4,4��# z��._��� .��]�  J� InputBindings �� 
 �� Key$ Z�� �� 	Modifiers$ Controlf� �� Command �� *UndoCommand+ �� CommandParameter ��  �� ElementName$ PaintCanvas���� $ C��$ Controlf� �� *CopyLastImageCommand+ �� $ PaintCanvas���� $ V��$ Controlf� �� *PasteImageCommand+ �� $ PaintCanvas���� $ Y��$ Controlf� �� *RedoCommand+ �� $ PaintCanvas���� $ S��$ Controlf� �� *ConfirmChangesCommand+ �� $ PaintCanvas��.+�`� $	��Top=� `� 
Background
 �����.�c� $	��100��
�������.@��� $��File & Canvas��$��5q�.����  �� Orientation$ 
Horizontal=�.+��� -    ��Name$ 
FileButton��$
��File��  ToolBarButtonStyle#��[ 
 � Tag �� $ PaintCanvas�� � ContextMenu ��  �� *PlacementTarget.DataContext+  �� RelativeSource  �� *Self+.@�m� $
��Save��! m� Command! �� *SaveCanvasCommand+" m� CommandParameter" �� *PlacementTarget.Tag+  �� # �� AncestorType# ContextMenum� $
��Open��! �� *OpenImageCommand+" �� *PlacementTarget.Tag+  �� # ContextMenu�� $��Clear��#��[ ���� *ClearCanvasCommand+���� $ PaintCanvas���� -   $��Resize Canvas��#��[ �� $��Undo / Redo & Confirm��$��5q�.���� $ 
Horizontal=�.+��� $
��Undo��#��[ ���� *UndoCommand+���� $ PaintCanvas���� $
��Redo��#��[ ���� *RedoCommand+���� $ PaintCanvas���� $��Confirm Changes��#��[ ���� *ConfirmChangesCommand+���� $ PaintCanvas���� $��Tools��$��5q�.���� $ 
Horizontal=�.+��� -   $��Tools��#��[ �� $��Insert Image��#��[ ���� *InsertImageCommand+���� $ PaintCanvas���� $��
Enter text��#��[ ���� *InsertTextCommand+���� $ PaintCanvas���� -   $ SettingsButton��$��Brush Settings��#��[ �� $��Text Settings��$��5q�.���� $ 
Horizontal=�$��Center=�.+��� $��Size:��$	��5,0q�$��Center=��� -   $��50��#��[ ���� *
TextSize+$ �� Mode$$ TwoWay=�% �� UpdateSourceTrigger$% PropertyChanged=�  TextSizeConverter& �� 	Converter#& [ �� $��px��$��0,0,10,0q�$��Center=��� $	��130��$��25��  ComboBoxStyle#��[ ' �� SelectedItem' �� *SelectedFontFamily+��-  'System.Windows.Data.CompositeCollection  *�Segoe UI)�*�	Arial)�*�Times New Roman)�*�Comic Sans MS)�d� -   ( d�Name$( 
BoldButton��#��[ ) � ToolTip$) Bold (Ctrl+B)��* d� 	IsChecked* �� *IsBold+$$ TwoWay=�.���� $��B��$
��Bold"�d� -   $( ItalicButton��#��[ $) Italic (Ctrl+I)��* �� *
IsItalic+$$ TwoWay=�.���� $��I��$��Italic$�d� -	   $( UnderlineButton��#��[ $) Underline (Ctrl+U)��* �� *IsUnderline+$$ TwoWay=�.���� $��U��$��	Underline{��� $��Palete��$��5q�$��5q�.���� ���� *PaletteColors+���� .R�A� + A� Columns$+ 4������ .R�� .+�A� -
   $��20��$��20��
�������$��1��$��3q����� A� $��26��$��26��$��2������   �� # ItemsControl, �� Path$, DataContext.SelectedColor�y�`� - `� 	Converter#- [  .1��� $, DataContext.SelectedColor�  �� # ItemsControl�� �� $��Color��$��5q�.���� $��Choose color��#��[ ���� *ChooseColorCommand+�� -   . ��Name$. CanvasScrollViewer��$
��Auto=�$
��Auto=�
�������.��� .+��� -   / ��Name$/ PaintCanvas��
�������$
��1200��$	��800��$
��Left=�$	��Top=�$
}�True��$
|�True��0 � LayoutTransform0 �� -   CanvasScaleTransform���� -   $��CursorOverlayCanvas��1 J� IsHitTestVisible$1 False��