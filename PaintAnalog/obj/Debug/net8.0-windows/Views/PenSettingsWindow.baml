   M S B A M L   `   `   `  ����  PaintAnalog)    #PaintAnalog.Views.PenSettingsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Pen Settings��5   	   $	��300��6   $	��400��6+   .��5      �� $��10q�6   .+�5   
   �� $��Pen Thickness��6   $
��Bold"�6)   $��0,0,0,5q�6;   �� 5   
   -   5         ��Name$ ThicknessSlider��5         CustomSliderStyle#��[  5	       � Minimum$ 1Q�5
       � Maximum$ 50Q�6    �� TickFrequency$ 1Q�5       �� IsSnapToTickEnabled$
 True��5      $��0,0,0,10q�5       � Value 5      �� *SelectedThickness+5      �� 5   
   $��	Pen Shape��6   $
��Bold"�6%   $��0,10,0,5q�67   �� 5   
   -   5         ��Name$ ShapeComboBox��5        ComboBoxStyle#��[ 5       �� SelectedValuePath$	 Tag��5      $��0,0,0,10q�5      	 �� SelectedValue	 5      �� *SelectedShape+.@�5      �� $��Polyline��6   

 � Tag$
 Polyline��6.   �� 5      $��	Rectangle��6   $
 	Rectangle��6/   �� 5      $��Triangle��6   $
 Triangle��6.   �� 5      $��Ellipse��6   $
 Ellipse��6-   5      �� 5   
   -   5        $��Apply��5        ToolBarButtonStyle#��[ 6;   5      5      