   M S B A M L   `   `   `  ����^  ZMicrosoft.Samples.Kinect.WpfViewers, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null�^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers#Microsoft.Samples.Kinect.WpfViewers  5   	    KinectSetupDev   KinectSetupDev.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   mmy^clr-namespace:Microsoft.Samples.Kinect.WpfViewers;assembly=Microsoft.Samples.Kinect.WpfViewers     5   	   -   5          � Title$  Web-Browsing Kinect��5   	   $	��600��6%   $	��990��62    � WindowState$ 	Maximized=�5   "   .��5      �� -   5        $��
MainCanvas��5      .+�5   
   � $
��1200��6    �� Left$	 344��6   
 �� Top$	 157��6/   $
��1920��6@   =   7Microsoft.Samples.Kinect.WpfViewers.KinectSensorChooser  5	   
   -   5          Name$ kinectSensorChooser1��5	   D   $	 332��6!   $	 284��63   $	��328��6b   ;   5Microsoft.Samples.Kinect.WpfViewers.KinectColorViewer  5
   
   -   5          Name$ kinectColorViewer1��5
   W   $ 0��6   $ 0��6/   $	��640��6>   $	��480��6J   5   /Microsoft.Samples.Kinect.WpfViewers.ImageViewer   Kinect 5      �� *Kinect+ �� ElementName$ kinectSensorChooser1���� 5   
   -   5         ��Name$ KeyboardButton��5   S   $	 958��6   $	 404��6#   $��Keyboard��64   $��62��6G   $	��130��6k   6w   �� 5   
   -   5        $ UrlVoiceCmd��5   ]   $	 822��6   $	 284��6#   $��Url Voice Commands��64   $��62��6Q   $	��134��6r   6~   �� 5   
   -   5        $ SearchVoiceCmd��5   a   $
 1086��6   $	 284��6$   $��Search Voice Commands��65   $��62��6U   $	��151��6y   6�   5      5      