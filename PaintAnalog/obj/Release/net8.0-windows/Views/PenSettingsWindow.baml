   M S B A M L   `   `   `  ����   PaintAnalog)    #PaintAnalog.Views.PenSettingsWindow   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Pen Settings��$	��300��$	��400��.���� $��10q�.+��� $��Pen Thickness��$
��Bold"�$��0,0,0,5q��� -    ��Name$ ThicknessSlider��   CustomSliderStyle#��[   � Minimum$ 1Q� � Maximum$ 50Q� �� TickFrequency$ 1Q� �� IsSnapToTickEnabled$
 True��$��0,0,0,10q� � Value �� *SelectedThickness+�� $��	Pen Shape��$
��Bold"�$��0,10,0,5q��� -    ��Name$ ShapeComboBox��  ComboBoxStyle#��[  �� SelectedValuePath$	 Tag��$��0,0,0,10q�	 �� SelectedValue	 �� *SelectedShape+.@��� $��Polyline��

 � Tag$
 Polyline���� $��	Rectangle��$
 	Rectangle���� $��Triangle��$
 Triangle���� $��Ellipse��$
 Ellipse���� -   $��Apply��  ToolBarButtonStyle#��[ 