   M S B A M L   `   `   `  ���� F  BPaintAnalog, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:PaintAnalog.ConvertersPaintAnalog.Converters      PaintAnalog.App   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     ;
converters$clr-namespace:PaintAnalog.Converters       �� 	Resources  .   (PaintAnalog.Converters.TextSizeConverter   TextSizeConverter  ToolBarButtonStyle �� 
TargetType �� Property �� Value �� 
TargetType ��Name �� CornerRadius �� Padding �� Triggers	 P� Property
 P� Value �� 
TargetName  CustomSliderStyle ]�Name �� IsDirectionReversed � Maximum � Minimum � Value ]� DecreaseRepeatButton J� Opacity ]� Thumb J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   ShadowDepth   
BlurRadius   Color ]� IncreaseRepeatButton  ComboBoxStyle � ColumnDefinitions ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate d�Name �� 	ClickMode  d� 	IsChecked! �� Mode" �� RelativeSource# F�Name$ �Name% �� IsDropDownOpen& � AllowsTransparency' �Name( J� SnapsToDevicePixels) � ActualWidth* �� MaxDropDownHeight+ K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation,   DirectionalNavigation- �� Padding��%�	  &	        &	      &	     &	 �      �� " ��.���  � ��$ 5q���  � ��$
 15,5q���  � �� �	#FFDDDDDD��  � ��
 �   ���  � ��$ SemiBold"���  � ��$ 0q���  � �� �� " ��.R��� $ border��#��z��$ 6��# z��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver$

 True��. ��� $ border�� � �� �	#FFB0C4DEP� 	 � ��$

 True��. ��� $ border�� � �� �	#FFA0B0C0P� 	 � {�$
 False��. ��� $ border�� � �� �	#FFEEEEEE��  � �� �	#FFAAAAAA�� " ��.���  � �� �#424242��  � �� �#E0E0E0��  � ��$ 0q���  � ��$ 24����  � �� �� " ��.R�� $��Center=�.+�]� $ 
PART_Track��#��z #��z #��z #��z  �� #��z��$	 0.3Q�$��2��$��Stretch=�$��Center=���Z�  n� $��16��$��16������ " n�.R��� #��z��$ 8��$��16��$��16��$��0q�   $ 0Q�$ 4Q�$ 	#88000000�� �� #��z��$��2��$��Stretch=�$��Center=���Z� �� " ��.���  � ��$	 5,0q���  � ��$ 8,0,28,0q���  � ��
 �������  � �� �	#FFCCCCCC��  � ��$ 1q���  � ��$ Center=���  � ��$ 32����  � �� �� " ��.R�� .+��� $ border��#��z��#��z��#��z��$ 6��._��  �� $��* ��� $
��Auto �.+��� $ ContentSite��$��0��$ False��#��z #��z #��z��$��8,0,0,0q�$��Center=�$
��Left=�d� $ ToggleButton��$��1��$|�False��$ Press=�$��24��$��0,0,4,0q�  �� *IsDropDownOpen+$! TwoWay=�" �� *TemplatedParent+���� " d�.R��� 
������ $��20��$��20��._�F� $# Arrow�����	#FF666666$��Center=�$��Center=�"����        � 	=  	= � z     	� $$ Popup��$��Bottom=�#��z% $
& True��$|�False��$��Slide=�.��� $' DropDown��$
( True��#��z) #��z* .+��� $ DropDownBorder��
����������	#FFCCCCCC$��1q�$ 6��._��� $��4,6,4,6q�$
( True��.���� $
+ True��$, 	Contained=� P� 	 �@J�IsMouseOver$

 True��. ��� $ border�� � �� �	#FFB0C4DEP� 	 �@��IsDropDownOpen$

 True��. ��� $ border�� � �� �	#FFA0B0C0P� 	 � {�$
 False��. ��� $ border�� � �� �	#FFEEEEEE�� $ border�� � �� �	#FFDDDDDD��  � �� �	#FFAAAAAA��  � �� �� .R��� $��14(�$	��4,2q�$	- 4,2q�$��Center=����� ���� ��  � ��$ Disabled=���  � ��$
 Auto=�